namespace stickyServer.Controllers
open System
open System.IdentityModel.Tokens.Jwt
open System.Security.Claims
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Microsoft.IdentityModel.Tokens
open stickyServer

type TokenModel (token: string, userName: string) = 
  member val Token: string = token
  member val UserName: string = userName

[<ApiController>]
[<Route("[controller]")>]
type UserController (logger : ILogger<UserController>, hub: ChatHub) =
    inherit ControllerBase()
    member val Hub : ChatHub = hub
    member val Logger : ILogger<UserController> = logger

    member private this.GetIdentity(username: string): ClaimsIdentity = 
        let claims = [Claim(ClaimsIdentity.DefaultNameClaimType, username)]
        ClaimsIdentity(claims, "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

    [<HttpPost("login")>]
    member __.Login(username: string): TokenModel =
        let now = Nullable<DateTime>(DateTime.UtcNow)
        let identity = __.GetIdentity(username)
        let jwt = JwtSecurityToken(
                    AuthOptions.ISSUER,
                    AuthOptions.AUDIENCE,
                    identity.Claims,
                    now,
                    Nullable<DateTime>(now.Value.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME))),
                    SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256))
        let encodedJwt = JwtSecurityTokenHandler().WriteToken(jwt)
        TokenModel(encodedJwt, identity.Name)

        