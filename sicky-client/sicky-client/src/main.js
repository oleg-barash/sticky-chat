import Vue from 'vue'
import Vuex from 'vuex'
import App from './App.vue'
import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";
Vue.use(Vuex)
Vue.config.productionTip = false
const connection = new HubConnectionBuilder()
    .withUrl(`http://localhost:5000/chat-hub`)
    .configureLogging(LogLevel.Information)
    .build();

let startedPromise = null;
function start() {
  startedPromise = connection.start()
      .catch(() => {
        return new Promise((resolve, reject) =>
            setTimeout(
                () => start().then(resolve).catch(reject),
                5000
            )
        );
      });
  return startedPromise;
}
const store = new Vuex.Store({
    state: {
        messages: []
    },
    mutations: {
        addMessage (state, message) {
            state.messages = [...state.messages, message]
        }
    }
})
connection.onclose(() => start())
connection.on("addMessage", data => {
    store.commit('addMessage', data)
    console.log(data)
});
start().then(() => {
  new Vue({
      render: h => h(App),
      store: store,
  }).$mount('#app')
});
