namespace stickyServer

open System.Linq

type Storage() =
    static member val Messages: seq<UserMessage> = Enumerable.Empty<UserMessage>() with get, set

