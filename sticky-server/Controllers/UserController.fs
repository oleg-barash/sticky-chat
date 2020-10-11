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

    [<HttpPost("login")>]
    member __.Login(username: string): TokenModel =
        let tokenHandler = JwtSecurityTokenHandler()
        let key = AuthOptions.GetSymmetricSecurityKey()
        let now = Nullable<DateTime>(DateTime.UtcNow)
        let tokenDescriptor =
            SecurityTokenDescriptor(
                Subject = ClaimsIdentity([Claim("name", username)]),
                Expires = Nullable<DateTime>(now.Value.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME))),
                SigningCredentials = SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature))
        let token = tokenHandler.CreateToken(tokenDescriptor)
        TokenModel(tokenHandler.WriteToken(token), username)

        