<template>
  <v-row>
    <v-col>
      <v-card class="mx-auto" title="Список задач">
        <TaskList @selected="selectedTask"/>
        <v-btn color="primary">Создать задачу</v-btn>
      </v-card>
    </v-col>
    <v-col>
      <v-card title="Описание задачи">
        <TaskDetails v-if="task !== null" :task="task"/>
      </v-card>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <p>Список сообщений hubs: </p>
      <p v-if="messages.length === 0">Список пуст</p>
      <v-list>
        <v-list-item v-for="message in messages">
          {{ message }}
        </v-list-item>
      </v-list>
    </v-col>
  </v-row>
</template>

<script setup>
import {ref} from "vue";
import TaskList from "@/components/task/TaskList.vue";
import TaskDetails from "@/components/task/TaskDetails.vue";
import { HubConnectionBuilder } from '@microsoft/signalr';
const messages = ref([]);
const connection = new HubConnectionBuilder()
    .withUrl('https://localhost:7245/parser-tasks-hub')
    .build();

connection.on('NewParserTaskCollectMessage', (body) => {
  messages.value.push(body);
})

connection.start()

const task = ref(null);
const selectedTask = (selected) => {
  task.value = selected;
};

</script>
