namespace stickyServer

open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Logging
open System.Linq
open stickyServer.AuthorizeAttribute

[<Authorize>]
type ChatHub(logger: ILogger<ChatHub>) =
    inherit Hub()
    member val Logger : ILogger<ChatHub> = logger

    [<Authorize>]
    member this.Send(message: string) =
        try
            let userMessage = UserMessage(this.Context.User.Identity.Name, message)
            Storage.Messages <- Storage.Messages.Append(userMessage)
            match this.Clients with
            | null -> ()
            | _ ->
                this.Clients.All.SendAsync("OnMessage", userMessage)
                |> ignore 
        with :? System.StackOverflowException -> this.Logger.LogError "Stackowerflow!"
