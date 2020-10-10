namespace stickyServer

open Microsoft.AspNetCore.Authentication.JwtBearer
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http.Connections
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.IdentityModel.Logging

type NameUserIdProvider () =
    interface IUserIdProvider with 
        member this.GetUserId(connection: HubConnectionContext): string =
            connection.User.Identity.Name

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        IdentityModelEventSource.ShowPII = true |> ignore
        // Add framework services.
        services.AddControllers() |> ignore
        services.AddSignalR() |> ignore
        services.AddCors() |> ignore
        services.AddSwaggerGen() |> ignore
        services.AddAuthentication(fun options ->
                options.DefaultAuthenticateScheme <- JwtBearerDefaults.AuthenticationScheme
                options.DefaultChallengeScheme <- JwtBearerDefaults.AuthenticationScheme
            )
            .AddJwtBearer() |> ignore
        services.AddSingleton<IUserIdProvider, NameUserIdProvider>() |> ignore
        services.AddSingleton<ChatHub>() |> ignore
        
        

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        app.UseSwagger() |> ignore
        app.UseSwaggerUI(fun c ->
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sticky API V1") |> ignore
        ) |> ignore 
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
            endpoints.MapHub<ChatHub>("/chat-hub",
              fun options ->
                  options.Transports = (HttpTransportType.WebSockets ||| HttpTransportType.LongPolling) |> ignore
            ) |> ignore
        ) |> ignore

    member val Configuration : IConfiguration = null with get, set
