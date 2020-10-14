# sticky-chat
Приложениe опубликовано на dockerhub.

Фронт: https://hub.docker.com/repository/docker/olegsdockerid/sicky-client

Бэк: https://hub.docker.com/repository/docker/olegsdockerid/sticky-server

Так же приложения развёрнуты на GKE

Фронт: http://35.184.35.166/

Бэк: http://35.223.68.13/swagger/index.html

При переходе на фронт открывется форма авторизации. Никаких правил валидации нет, и никакой регистрации. Просто вводите никнейм и попадаете в единственную комнату, где видны сообщения от всех пользователей.

Авторизация пользователя реализована на JWT.

