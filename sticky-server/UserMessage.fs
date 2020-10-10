namespace stickyServer

open System

type UserMessage() =
    new(name: string, message: string) as this =
        UserMessage() then
        this.Name <- name
        this.Message <- message
    member val Name = "unknown" with get,set
    member val Message = null with get,set
    member val Time = DateTime.UtcNow with get,set
    
