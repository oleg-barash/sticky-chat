<template>
  <div>
    <div>
      <a-list class="list-container" :data-source="this.$store.state.messages">
        <a-list-item slot="renderItem" slot-scope="item">
          <a-list-item-meta :description="format(parseISO(item.time), 'hh:mm:ss')"/>
          <b>{{ item.name }}:</b> {{ item.message }}
        </a-list-item>
      </a-list>
    </div>
    <a-input-search @search="send" v-model="message" placeholder="Введите сообщение" size="large" enter-button="Отправить">
    </a-input-search>
  </div>
</template>

<script>
import axios from "axios";
import urljoin from "url-join";
import { format, parseISO } from 'date-fns';
export default {
  name: 'Home',
  data() {  return { message: "", format, parseISO }
  },
  mounted() {
    let me  = this;
    axios.get(urljoin(window.apiUrl, 'message'))
        .then(function (response) {
          console.log(response)
          me.$store.commit('historyFetched', response)
        })
        .catch(function (error) {
          console.log(error);
          if (error.response.status === 401) {
            me.$router.push('login')
          }
        });
  },
  methods: {
    send() {
      this.$store.commit('send', this.message)
      this.message = ''
    },
  },
}
</script>

<style>
div.list-container {
  bottom: 40px;
  width: 100%;
  padding: 0 2em;
}
</style>