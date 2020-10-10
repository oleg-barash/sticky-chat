namespace stickyServer

open Microsoft.AspNetCore.Authorization
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Logging
open System.Linq

[<Authorize>]
type ChatHub(logger: ILogger<ChatHub>) =
    inherit Hub()
    member val Logger : ILogger<ChatHub> = logger
    member val Messages: seq<UserMessage> = Enumerable.Empty<UserMessage>() with get, set

    
    member this.Send(message: string) =
        try
            let userMessage = UserMessage(this.Context.User.Identity.Name, message)
            this.Messages <- this.Messages.Append(userMessage)
            match this.Clients with
            | null -> ()
            | _ ->
                this.Clients.All.SendAsync("OnMessage", userMessage)
                |> ignore 
        with :? System.StackOverflowException -> this.Logger.LogError "Stackowerflow!"
