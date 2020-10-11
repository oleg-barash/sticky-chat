namespace stickyServer.Helpers.JwtMiddlware
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.IdentityModel.Tokens
open System
open System.IdentityModel.Tokens.Jwt
open System.Text
open stickyServer

type JwtMiddleware(next: RequestDelegate) =
    member val _next: RequestDelegate = next with get
    member __.Invoke(context: HttpContext) =
        let tokenHeader = context.Request.Headers.["Authorization"]
        if (tokenHeader.Count > 0) then
            let token = Array.last(tokenHeader.[0].Split(" "))
            __.attachUserToContext(context, token)
        let task = __._next.Invoke(context)
        task

    member __.attachUserToContext(context: HttpContext, token: string) =
        try 
            let tokenHandler = JwtSecurityTokenHandler()
            let key = Encoding.ASCII.GetBytes(AuthOptions.KEY)
            let (_, validatedToken: SecurityToken) =
                      tokenHandler.ValidateToken(token,
                        TokenValidationParameters(
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = SymmetricSecurityKey(key),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ClockSkew = TimeSpan.Zero))
            let jwtToken = validatedToken :?> JwtSecurityToken
            let userNameClaim = jwtToken.Claims
                                    |> Seq.find(fun x -> String.Equals(x.Type, "name"))
            context.Items.["UserName"] <- userNameClaim.Value
        with :? Exception -> printfn "Authentication failed."