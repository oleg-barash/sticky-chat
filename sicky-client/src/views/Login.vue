<template>
  <div>
    <a-input-search v-bind:disabled="loading" v-model="username"  @search="send"  placeholder="Имя пользователя" size="large" enter-button="Войти" >
    </a-input-search>
  </div>
</template>

<script>
import * as axios from "axios";
import urljoin from "url-join";

export default {
  name: 'Home',
  methods: {
    send() {
      let me  = this;
      me.loading = true;
      axios.post(urljoin(window.apiUrl, `user/login?username=${this.username}`))
          .then(function (res) {
            window.token = res.data.token
            axios.defaults.headers.common['Authorization'] = `Bearer ${res.data.token}`;
            window.username = res.data.userName
            me.$store.commit('start')
            me.$router.push('home')
          })
          .catch(function (error) {
            console.log(error);
          })
          .finally(() => { me.loading = false });
    }
  },
  data() {  return { username: '', loading: false}},
}
</script>
