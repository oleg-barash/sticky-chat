<template>
  <div>
    <div
        class="infinite-container"
        :infinite-scroll-distance="10"
    >
      <a-list :data-source="this.$store.state.messages">
        <a-list-item slot="renderItem" slot-scope="item">
          <a-list-item-meta :description="item.time"/>
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

export default {
  name: 'Home',
  data() {  return { message: ""}},
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
.infinite-container {
  border: 1px solid #e8e8e8;
  border-radius: 4px;
  overflow: auto;
  padding: 8px 24px;
  height: 300px;
}
.loading-container {
  position: absolute;
  bottom: 40px;
  width: 100%;
  text-align: center;
}
</style>