<script setup lang="ts">
import { storeToRefs } from "pinia";
import { taskStore, TaskItem } from "@/entities/tasks";
import { ref } from "vue";

const { tasks } = storeToRefs(taskStore());

const showCreateTaskDialog = ref<boolean>(false);
</script>

<template>
  <v-dialog width="70%" v-model="showCreateTaskDialog">
    <v-container>
      <v-card>
        <v-card-title class="text-primary">Создание задачи парсинга</v-card-title>
        <v-card-actions>
          <v-btn block="true" color="primary" variant="flat">Создать</v-btn>
        </v-card-actions>
      </v-card>
    </v-container>
  </v-dialog>
  <v-container>
    <v-layout class="d-flex justify-center">
      <v-card width="75%" class="pa-3">
        <div class="d-flex flex-row justify-space-between">
          <v-card-title class="text-primary">Задачи парсинга</v-card-title>
          <v-card-actions class="d-flex justify-end">
            <v-btn color="primary" variant="flat" @click="showCreateTaskDialog = true">Создать новую</v-btn>
          </v-card-actions>
        </div>
        <v-divider class="mt-5 mb-5"/>
        <div class="d-flex justify-center mb-5">
          <v-list v-if="tasks.length !== 0" width="100%">
            <v-list-item v-for="task in tasks" :key="task.id">
              <TaskItem :task="task"/>
            </v-list-item>
          </v-list>
          <div v-else>
            Задачи не найдены
          </div>
        </div>
      </v-card>
    </v-layout>
  </v-container>
</template>
