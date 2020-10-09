namespace stickyServer.Controllers
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Logging
open stickyServer

[<ApiController>]
[<Route("[controller]")>]
type MessageController private () =
    inherit ControllerBase()
    new (logger : ILogger<MessageController>, hubContext: IHubContext<ChatHub>) as this =
        MessageController() then
        this.Logger <- logger
        this.HubContext <- hubContext
        
    member val HubContext : IHubContext<ChatHub> = null with get, set
    member val Logger : ILogger<MessageController> = null with get, set
    [<HttpPost>]
    member __.Post(message: UserMessage) =
        __.HubContext.Clients.All.SendAsync("addMessage", message)