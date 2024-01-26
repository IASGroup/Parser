<script setup lang="ts">
import {storeToRefs} from "pinia";
import {CreateTaskDialog, TaskItem, TaskListModel, taskStore} from "@/entities/tasks";
import {onMounted, ref} from "vue";
import {
  GetTasksAsync,
  ParserTaskCollectMessage,
  ParserTaskCollectMessageTypes,
  parserTasksHubMessages,
  parserTasksHubUrl
} from "@/shared/api";
import {HubConnectionBuilder} from "@microsoft/signalr"

const {tasks} = storeToRefs(taskStore());

const showCreateTaskDialog = ref<boolean>(false);

onMounted(async () => {
  taskLoading.value = true;
  const getTasksResponse = await GetTasksAsync();
  if (getTasksResponse.isSuccess) tasks.value = getTasksResponse.result as Array<TaskListModel>;
  else console.log(getTasksResponse)
  taskLoading.value = false;

  const hub = new HubConnectionBuilder()
    .withUrl(parserTasksHubUrl)
    .build();
  hub.on(parserTasksHubMessages.NewParserTaskCollectMessage, data => {
    const message = data as ParserTaskCollectMessage;
    const task = tasks.value.find(x => x.id === message.parserTaskId);
    if (!task) return;
    if (message.type === ParserTaskCollectMessageTypes.StatusChanged) {
      task.statusId = message.parserTaskStatusChangedMessage?.newTaskStatus;
    } else if (message.type === ParserTaskCollectMessageTypes.Progress) {
      task.completedPartsNumber = message.parserTaskProgressMessage.completedPartsNumber;
    }
  })
  await hub.start();
});

const taskLoading = ref<boolean>(false);
</script>

<template>
  <v-dialog width="70%" v-model="showCreateTaskDialog">
    <create-task-dialog @parser-task-created="showCreateTaskDialog = false"/>
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
        <div v-if="taskLoading" class="d-flex flex-column align-center">
          <span class="text-button mb-4">Загрузка данных</span>
          <v-progress-linear color="primary" :indeterminate="true"/>
        </div>
        <div v-else class="d-flex justify-center mb-5">
          <v-list v-if="tasks.length !== 0" width="100%">
            <v-list-item v-for="task in tasks" :key="task.id">
              <task-item :task="task"/>
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
