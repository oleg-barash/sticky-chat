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
open Microsoft.IdentityModel.Tokens
open Microsoft.OpenApi.Models;

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
        services.AddControllers() |> ignore
        services.AddSignalR() |> ignore
        services.AddCors() |> ignore
        services.AddHttpContextAccessor() |> ignore
        services.AddSwaggerGen(fun swaggerGenOptions ->
            let swaggerGenOptions1 = swaggerGenOptions;
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
            swaggerGenOptions1.AddSecurityRequirement(securityRequirement);
        ) |> ignore
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(fun options ->
                options.RequireHttpsMetadata <- false
                options.TokenValidationParameters = TokenValidationParameters(
                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.ISSUER,
                    ValidateAudience = true,
                    ValidAudience = AuthOptions.AUDIENCE,
                    ValidateLifetime = true,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true) |> ignore
                options.Events = JwtBearerEvents(
                        OnMessageReceived = fun context ->
                            let path = context.HttpContext.Request.Path
                            if (path.StartsWithSegments(PathString(hubName))) then
                                context.Token <- context.Request.Query.["access_token"].Item(0)
                            Task.CompletedTask;
                        ) |> ignore
            ) |> ignore
        services.AddSingleton<IUserIdProvider, NameUserIdProvider>() |> ignore
        services.AddSingleton<ChatHub>() |> ignore
        

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        app.UseSwagger() |> ignore
        app.UseSwaggerUI(fun c ->
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sticky API V1") |> ignore) |> ignore 
        app.UseRouting() |> ignore
        app.UseAuthentication() |> ignore
        app.UseAuthorization() |> ignore
        
        app.UseCors(fun (builder) ->
                builder.WithOrigins("http://localhost:8080") |> ignore
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
