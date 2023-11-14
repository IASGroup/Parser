<script setup lang="ts">
import { TaskListModel, TaskStatuses } from "@/entities/tasks";
import {computed} from "vue";

const { task } = defineProps<{ task: TaskListModel }>();

const chipModel = computed(() => {
  switch (task.statusId) {
    case TaskStatuses.Created:
      return {
        color: "blue",
        text: "Создана"
      };
    case TaskStatuses.InProgress:
      return {
        color: "yellow",
        text: "Выполняется"
      };
    case TaskStatuses.Paused:
      return {
        color: "blue-grey",
        text: "Приостановлена"
      };
    case TaskStatuses.Error:
      return {
        color: "red",
        text: "Ошибка"
      };
    case TaskStatuses.Finished:
      return {
        color: "green",
        text: "Завершена"
      };
  }
  return {
    color: "black",
    text: "Неизвестно"
  };
})

</script>

<template>
  <v-card>
    <v-row class="d-flex align-center pa-3">
      <v-col>
        <v-chip :color="chipModel.color" variant="flat">{{ chipModel.text }}</v-chip>
      </v-col>
      <v-col>
        <span>{{ task.name }}</span>
      </v-col>
      <div class="d-flex justify-center">
        <v-col>
          <v-btn icon>
            <v-icon size="30" color="green">mdi-play-circle-outline</v-icon>
          </v-btn>
        </v-col>
        <v-col>
          <v-btn icon>
            <v-icon size="30" color="orange">mdi-menu-open</v-icon>
          </v-btn>
        </v-col>
        <v-col></v-col>
      </div>
    </v-row>
  </v-card>
</template>
