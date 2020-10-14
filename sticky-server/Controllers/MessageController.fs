namespace stickyServer.Controllers
open System
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Logging
open stickyServer
open System.Linq
open stickyServer.AuthorizeAttribute
[<ApiController>]
[<Route("[controller]")>]
type MessageController (logger : ILogger<MessageController>, hub: IHubContext<ChatHub>, httpContextAccessor: IHttpContextAccessor) =
    inherit ControllerBase()
    member val Hub : IHubContext<ChatHub> = hub
    member val Logger : ILogger<MessageController> = logger
    member val HttpContextAccessor: IHttpContextAccessor = httpContextAccessor

    [<HttpPost>]
    [<Authorize>]
    member __.Post(message: string) =
        let userMessage = UserMessage(string __.HttpContextAccessor.HttpContext.Items.["UserName"], message)
        Storage.Messages <- Storage.Messages.Append(userMessage)
        match __.Hub.Clients with
        | null -> ()
        | _ ->
            __.Hub.Clients.All.SendAsync("OnMessage", userMessage)
            |> ignore 
        
    [<HttpGet>]
    [<Authorize>]
    member __.History(limit: Nullable<int>): seq<UserMessage> =
        match Seq.length Storage.Messages with
        | 0 -> Seq.empty
        | _ -> match limit.HasValue with
                | false -> Storage.Messages |> Seq.skip ((Seq.length Storage.Messages) - 500) 
                | true -> Storage.Messages |> Seq.skip ((Seq.length Storage.Messages) - limit.Value)
        