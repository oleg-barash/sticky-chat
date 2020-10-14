import Vue from 'vue'
import Vuex from 'vuex'
import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr"
import urljoin from 'url-join'
import router from './router'
import Antd from 'ant-design-vue'
import App from "@/App";
import * as axios from "axios";
Vue.use(Vuex)
Vue.use(Antd)
Vue.config.productionTip = false
window.apiUrl = 'http://35.223.68.13/'

const store = new Vuex.Store({
    state: {
        messages: [],
        connection: {}
    },
    mutations: {
        addMessage (state, message) {
            state.messages = [...state.messages, message]
        },
        historyFetched (state, messages){
            state.messages = messages.data
        },
        send (state, message){
            axios.post(urljoin(window.apiUrl, `message?message=${message}`))
                .catch(function (error) {
                    console.log(error);
                })
        },
        start(){
            this.connection = new HubConnectionBuilder()
                .withUrl(urljoin(window.apiUrl, 'chat-hub'), {
                    accessTokenFactory: () => {
                        return window.token
                    }})
                .configureLogging(LogLevel.Information)
                .build()
            this.connection.start()
                .then(() => {
                })
                .catch(() => {
                    return new Promise(() =>
                        setTimeout(
                            () => store.commit("start"),
                            5000
                        )
                    );
                });

            this.connection.onclose(() => store.commit("start"))
            this.connection.on('OnMessage', data => {
                store.commit('addMessage', data)
                console.log(data)
            });
        }
    }
})


new Vue({
    store,
    render: h => h(App),
    router
}).$mount('#app')




