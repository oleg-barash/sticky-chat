namespace stickyServer

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.Connections
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.IdentityModel.Logging
open Microsoft.IdentityModel.Tokens
open Microsoft.OpenApi.Models
open stickyServer.Helpers.JwtMiddlware

type NameUserIdProvider () =
    interface IUserIdProvider with 
        member this.GetUserId(connection: HubConnectionContext): string =
            connection.User.Identity.Name

type Startup private () =
    static let hubName: string = "chat-hub" 
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        IdentityModelEventSource.ShowPII <- true
        services.AddSignalR() |> ignore
        services.AddCors() |> ignore
        services.AddAuthorization() |> ignore
        services.AddHttpContextAccessor() |> ignore
        services.AddSwaggerGen(fun swaggerGenOptions ->
            let swaggerGenOptions1 = swaggerGenOptions
            let securityRequirement = OpenApiSecurityRequirement()
            securityRequirement.Add(OpenApiSecurityScheme(
                                        Reference = OpenApiReference(
                                            Id = "Bearer Token",
                                            Type = Nullable<ReferenceType>(ReferenceType.SecurityScheme)))
                                        , Array.empty)
            swaggerGenOptions.AddSecurityDefinition("Bearer Token",
                OpenApiSecurityScheme(
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Description = "Specify the authorization token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http)
                )
            swaggerGenOptions1.AddSecurityRequirement(securityRequirement)
        ) |> ignore
        services.AddSingleton<IUserIdProvider, NameUserIdProvider>() |> ignore
        services.AddSingleton<ChatHub>() |> ignore
        services.AddControllers() |> ignore

        

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        app.UseSwagger() |> ignore
        app.UseSwaggerUI(fun c ->
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sticky API V1") |> ignore) |> ignore 
        app.UseRouting() |> ignore
        app.UseMiddleware<JwtMiddleware>() |> ignore
        app.UseCors(fun (builder) ->
                builder.WithOrigins("http://localhost:8080", "http://35.184.35.166") |> ignore
                builder.AllowAnyMethod() |> ignore
                builder.AllowAnyHeader() |> ignore
                builder.AllowCredentials() |> ignore) |> ignore

        app.UseEndpoints(fun endpoints ->
            endpoints.MapControllers() |> ignore
            endpoints.MapHub<ChatHub>(hubName,
              fun options ->
                  options.Transports = (HttpTransportType.WebSockets ||| HttpTransportType.LongPolling) |> ignore
            ) |> ignore
        ) |> ignore

    member val Configuration : IConfiguration = null with get, set
