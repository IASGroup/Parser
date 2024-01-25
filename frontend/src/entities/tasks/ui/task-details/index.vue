<script setup lang="ts">
import {TaskDetailsModel, TaskDetailsResultModel, TaskStatuses} from "@/entities/tasks";
import {computed, onMounted, reactive, ref} from "vue";
import {
  GetTaskAsync,
  GetTaskResultsAsync, ParserTaskCollectMessage, ParserTaskCollectMessageTypes,
  parserTasksHubMessages,
  parserTasksHubUrl,
  RunTaskAsync,
  StopTaskAsync,
  DownloadPartialTaskResult,
  DownloadTaskResults
} from "@/shared/api";
import {HubConnectionBuilder} from "@microsoft/signalr";
import {VDataTable} from "vuetify/labs/components";
import {router} from "@/app/providers";

const {id} = defineProps<{ id: string }>();

let task = reactive<TaskDetailsModel | null>(null);
const isTaskLoading = ref<boolean>(false);
const isTaskLoaded = ref<boolean>(false);

const taskResults = ref<Array<TaskDetailsResultModel>>([]);
const isTaskResultsLoading = ref<boolean>(false);
const isTaskResultsLoaded = ref<boolean>(false);

const isTaskRunnable = computed(() => !(task?.statusId === TaskStatuses.InProgress || task?.statusId === TaskStatuses.Finished))
const isTaskCompleted = computed(() => task.completedPartsNumber === task.allPartsNumber);


const nexPartUrl = ref<string | null>(null);

onMounted(async () => {
  await loadTask(id);
  await loadTaskResults(id);
  const hub = new HubConnectionBuilder()
    .withUrl(parserTasksHubUrl)
    .build();
  hub.on(parserTasksHubMessages.NewParserTaskCollectMessage, data => {
    const message = data as ParserTaskCollectMessage;
    if (message.parserTaskId !== id) return;
    if (message.type === ParserTaskCollectMessageTypes.StatusChanged && task !== null && message.parserTaskStatusChangedMessage) {
      task.statusId = message.parserTaskStatusChangedMessage.newTaskStatus;
    } else if (message.type === ParserTaskCollectMessageTypes.Progress) {
      nexPartUrl.value = message.parserTaskProgressMessage.nextPartUrl;
      task!.completedPartsNumber = message.parserTaskProgressMessage.completedPartsNumber;
      console.log("Статус 5")
      taskResults.value.push({
        url: message.parserTaskProgressMessage.completedPartUrl,
        id: message.parserTaskProgressMessage.completedPartId,
        parserTaskId: message.parserTaskId,
        statusId: message.parserTaskProgressMessage.completedPartStatusId
      })
    }
  })
  await hub.start();
});

async function loadTask(taskId: string) {
  isTaskLoading.value = true;
  const response = await GetTaskAsync(taskId);
  if (!response.isSuccess) return;
  task = reactive<TaskDetailsModel | null>(response.result);
  isTaskLoaded.value = true;
  isTaskLoading.value = false;
}

async function loadTaskResults(taskId: string) {
  isTaskResultsLoading.value = true;
  const response = await GetTaskResultsAsync(taskId);
  if (!response.isSuccess) return;
  taskResults.value = response.result as Array<TaskDetailsResultModel>;
  isTaskResultsLoaded.value = true;
  isTaskResultsLoading.value = false;
}

async function runTask(taskId: string): Promise<void> {
  const response = await RunTaskAsync(taskId);
  if (!response.isSuccess || !response.result) {
    console.log(response);
  }
}

async function stopTask(taskId: string): Promise<void> {
  const response = await StopTaskAsync(taskId);
  if (!response.isSuccess || !response.result) {
    console.log(response);
  }
}

const headers = ref([
  {title: 'Ссылка', value: 'url'},
  {title: 'Статус', value: 'statusId'},
  {title: 'Действия', key: 'actions', sortable: false},
]);


const isTaskResultDetailsOpened = ref<boolean>(false);
const currentTaskResult = ref<TaskDetailsResultModel | null>(null);

const resultStatuses = [
  { name: 'Успешная загрузка', color: 'green' },
  { name: 'Не успешная загрузка', color: 'red' }
]
function openResultDetails(item: TaskDetailsResultModel) {
  currentTaskResult.value = item;
  isTaskResultDetailsOpened.value = true;
}

async function downloadTaskResult(item: TaskDetailsResultModel) {
  const response = await DownloadPartialTaskResult(item.parserTaskId, item.id);
  if (response.isSuccess) {
    const link = document.createElement("a");
    document.body.appendChild(link);
    const url = URL.createObjectURL(response.result!);
    link.href = url;
    link.download = 'partResult.txt';
    link.click();
    link.remove();
    URL.revokeObjectURL(url);
  }
}

async function downloadTaskResults(taskId: string) {
  const response = await DownloadTaskResults(taskId);
  if (response.isSuccess) {
    const link = document.createElement("a");
    document.body.appendChild(link);
    const url = URL.createObjectURL(response.result!);
    link.href = url;
    link.download = 'results.txt';
    link.click();
    link.remove();
    URL.revokeObjectURL(url);
  }
}
</script>

<template>
  <v-dialog v-model="isTaskResultDetailsOpened" width="70%">
    <v-card class="pa-5">
      <v-card-title class="ma-0 mb-4 pa-0 text-primary">Результат задачи парсинга</v-card-title>
      <v-row>
        <v-col class="d-flex justify-start">
          {{ currentTaskResult.url }}
        </v-col>
        <v-col class="d-flex justify-end">
          <v-chip :color="resultStatuses[currentTaskResult.statusId - 1].color">{{resultStatuses[currentTaskResult.statusId - 1].name}}</v-chip>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore
          magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo
          consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla
          pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id
          est laborum.
        </v-col>
      </v-row>
    </v-card>
  </v-dialog>
  <v-btn color="primary" @click="router.push(`/`)">Назад</v-btn>
  <v-progress-linear v-if="isTaskResultsLoading" :indeterminate="true" />
  <v-row class="ma-0" v-else-if="isTaskLoaded">
    <v-col>
      <v-card :flat="true" class="d-flex justify-center pa-2 text-h6">
        {{ task.name }}
      </v-card>
    </v-col>
  </v-row>
  <v-row class="ma-0 pa-2 d-flex flex-row justify-space-between" v-if="isTaskLoaded">
    <div v-if="!isTaskCompleted">
      <v-btn class="mr-3" color="green" :disabled="!isTaskRunnable" @click="runTask(id)">
        <v-icon size="27">mdi-play-circle-outline</v-icon>
      </v-btn>
      <v-btn class="mr-3" color="red" :disabled="isTaskRunnable" @click="stopTask(id)">
        <v-icon size="27">mdi-stop</v-icon>
      </v-btn>
    </div>
    <div class="d-flex flex-row justify-end w-100 mt-5 mb-5">
      <v-slider
        :disabled="task.statusId !== TaskStatuses.InProgress" :readonly="true" :hide-details="true"
        thumb-label="always" thumb-size="11" color="primary"
        :max="task.allPartsNumber"
        :model-value="task.completedPartsNumber"
      >
        <template v-slot:append>
          {{ task.allPartsNumber }}
        </template>
      </v-slider>
    </div>
    <div>
      <v-btn width="128" color="primary" @click="downloadTaskResults(task.id)">Загрузить</v-btn>
    </div>
  </v-row>
  <v-row class="ma-0" v-if="isTaskResultsLoaded === true">
    <v-col class="pa-2">
      <v-card>
        <v-list>
          <v-list-item v-if="nexPartUrl && task?.statusId === TaskStatuses.InProgress">
            <v-card class="d-flex justify-center">
              <div class="d-flex align-center">
                <div class="d-flex align-center">
                  {{ nexPartUrl }}
                </div>
                <div class="ml-7 d-flex align-center">
                  <v-progress-circular size="20" indeterminate color="orange"/>
                </div>
              </div>
            </v-card>
          </v-list-item>
        </v-list>
        <v-data-table :items="taskResults" :headers="headers">
          <template v-slot:item.statusId="{ item }">
            <v-chip :color="resultStatuses[item.statusId - 1].color">{{resultStatuses[item.statusId - 1].name}}</v-chip>
          </template>
          <template v-slot:item.actions="{ item }">
            <v-icon
              @click="openResultDetails(item)"
              color="primary"
              class="mr-4"
            >
              mdi-menu-open
            </v-icon>
            <v-icon
              @click="downloadTaskResult(item)"
              color="primary"
            >
              mdi-download-outline
            </v-icon>
          </template>
        </v-data-table>
      </v-card>
    </v-col>
  </v-row>
</template>

<style>
</style>
