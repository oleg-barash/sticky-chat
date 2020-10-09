namespace stickyServer

type UserMessage() =
    new(name: string, text: string) as this =
        UserMessage() then
        this.Name <- name
        this.Message <- text
    member val Name = null with get,set
    member val Message = null with get,set
