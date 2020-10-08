import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr'
export default {
    install () {
        const connection = new HubConnectionBuilder()
            .withUrl('http://localhost:5000/chat-hub')
            .configureLogging(LogLevel.Information)
            .build()

        let startedPromise = null
        function start () {
            startedPromise = connection.start().catch(err => {
                console.error('Failed to connect with hub', err)
                return new Promise((resolve, reject) =>
                    setTimeout(() => start().then(resolve).catch(reject), 5000))
            })
            return startedPromise
        }
        connection.onclose(() => start())

        start()
    }
}