// stolen from https://github.com/aspnet/SignalR-samples/blob/master/ChatSample/ChatSample/Hubs/ChatHub.cs
namespace stickyServer
open Microsoft.AspNetCore.SignalR;
type ChatHub () =
    inherit Hub()
    member val Messages : seq<UserMessage> = null with get, set
    member this.Send(name: string , message: string ) =
        async {
            this.Clients.All.SendAsync("MessageReceived", name, message) |> ignore
            this.Messages <- Seq.append (this.Messages |> Seq.tail) (Seq.singleton (UserMessage(name, message)))
        }