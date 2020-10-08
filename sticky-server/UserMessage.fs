namespace stickyServer

type UserMessage(name: string, text: string) =
    member val Name = name with get,set
    member val Text = text with get,set
