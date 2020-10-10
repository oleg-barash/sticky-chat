namespace stickyServer.Controllers
open System
open System.IdentityModel.Tokens.Jwt
open System.Security.Claims
open Microsoft.AspNetCore.Authorization
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Microsoft.IdentityModel.Tokens
open stickyServer

[<ApiController>]
[<Route("[controller]")>]
[<Authorize>]
type MessageController (logger : ILogger<MessageController>, hub: ChatHub) =
    inherit ControllerBase()
    member val Hub : ChatHub = hub
    member val Logger : ILogger<MessageController> = logger

    [<HttpPost>]
    member __.Post(message: string) =
        __.Hub.Send(message)
        
    [<HttpGet>]
    member __.History(): seq<UserMessage> =
        __.Hub.Messages |> Seq.take 500
        