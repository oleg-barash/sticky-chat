import Vue from 'vue'
import Vuex from 'vuex'
import App from './App.vue'
import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";
import axios from 'axios';
import urljoin from 'url-join';
Vue.use(Vuex)
Vue.config.productionTip = false
const baseUrl = 'http://localhost:5000/';
const connection = new HubConnectionBuilder()
    .withUrl(urljoin(baseUrl, 'chat-hub'))
    .configureLogging(LogLevel.Information)
    .build()

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
let startedPromise = null;
function start() {
  startedPromise = connection.start()
      .then(() => {
          axios.get(urljoin(baseUrl, 'history'))
              .then(function (response) {
                  console.log(response)
                  debugger
                  store.commit('HistoryFetched', response)
              })
              .catch(function (error) {
                  console.log(error);
              });
      })
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

connection.onclose(() => start())
connection.on('OnMessage', data => {
    store.commit('OnMessage', data)
    console.log(data)
});
start().then(() => {
  new Vue({
      render: h => h(App),
      store: store,
  }).$mount('#app')
});
