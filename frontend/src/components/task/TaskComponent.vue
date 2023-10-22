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

</template>

<script setup>
import {ref} from "vue";
import TaskList from "@/components/task/TaskList.vue";
import TaskDetails from "@/components/task/TaskDetails.vue";

const task = ref(null);
const selectedTask = (selected) => {
  task.value = selected;
};

</script>
