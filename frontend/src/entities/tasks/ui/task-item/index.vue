<script setup lang="ts">
import {TaskListModel, TaskStatuses} from "@/entities/tasks";
import {computed} from "vue";

const {task} = defineProps<{ task: TaskListModel }>();

const chipModel = computed(() => {
  switch (task.statusId) {
    case TaskStatuses.Created:
      return {
        color: "blue-darken-1",
        text: "Создана"
      };
    case TaskStatuses.InProgress:
      return {
        color: "pink-darken-1",
        text: "Выполняется"
      };
    case TaskStatuses.Paused:
      return {
        color: "blue-grey",
        text: "Приостановлена"
      };
    case TaskStatuses.Error:
      return {
        color: "red-darken-1",
        text: "Ошибка"
      };
    case TaskStatuses.Finished:
      return {
        color: "green-darken-1",
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
      <v-col cols="2" class="d-flex flex-column align-center">
        <v-chip :color="chipModel.color" variant="flat">{{ chipModel.text }}</v-chip>
      </v-col>
      <v-col class="d-flex flex-column align-center">
        <span>{{ task.name }}</span>
        <div class="d-flex flex-row justify-end w-100 mt-5">
          <v-slider :disabled="task.statusId !== TaskStatuses.InProgress" :readonly="true" :hide-details="true" thumb-label="always" thumb-size="11"
            color="primary"
            :max="task.allPartsNumber"
            :model-value="task.competedPartsNumber"
          >
            <template v-slot:append>
              {{ task.allPartsNumber }}
            </template>
          </v-slider>
        </div>
      </v-col>
      <div class="d-flex justify-center">
        <v-col>
          <v-btn :disabled="task.statusId === TaskStatuses.Finished" icon>
            <v-icon v-if="!(task.statusId === TaskStatuses.InProgress)" size="30" color="green">mdi-play-circle-outline</v-icon>
            <v-progress-circular v-else color="red" width="2" indeterminate>
              <v-icon color="red">mdi-stop</v-icon>
            </v-progress-circular>
          </v-btn>
        </v-col>
        <v-col class="d-flex flex-column mr-1">
          <v-btn icon>
            <v-icon size="30" color="orange">mdi-menu-open</v-icon>
          </v-btn>
          <v-badge v-if="task.hasError" content="С ошибками" location="bottom" offset-y="-17" color="red-darken-1"/>
        </v-col>
      </div>
    </v-row>
  </v-card>
</template>
