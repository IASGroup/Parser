import {defineStore} from "pinia";
import {ref} from "vue";
import {TaskListModel, TaskStatuses, TaskTypes} from "./models";

export const useTaskStore = defineStore("tasks", () => {
  const tasks = ref<Array<TaskListModel>>([]);

  tasks.value.push({
    id: "26f2942d-6806-44e6-b40f-adae8da5f858",
    name: "Спарсить фейк апи",
    url: "https://jsonplaceholder.typicode.com/todos/{todo}",
    typeId: TaskTypes.Api,
    statusId: TaskStatuses.Created,
    hasError: false,
    allPartsNumber: 40,
    competedPartsNumber: 13
  }, {
    id: "26f2942d-6806-44e6-b40f-adae8da5f858",
    name: "Спарсить фейк апи",
    url: "https://jsonplaceholder.typicode.com/todos/{todo}",
    typeId: TaskTypes.Api,
    statusId: TaskStatuses.InProgress,
    hasError: false,
    allPartsNumber: 40,
    competedPartsNumber: 25
  }, {
    id: "26f2942d-6806-44e6-b40f-adae8da5f858",
    name: "Спарсить фейк апи",
    url: "https://jsonplaceholder.typicode.com/todos/{todo}",
    typeId: TaskTypes.Api,
    statusId: TaskStatuses.Paused,
    hasError: true,
    allPartsNumber: 40,
    competedPartsNumber: 10
  }, {
    id: "26f2942d-6806-44e6-b40f-adae8da5f858",
    name: "Спарсить фейк апи",
    url: "https://jsonplaceholder.typicode.com/todos/{todo}",
    typeId: TaskTypes.Api,
    statusId: TaskStatuses.Error,
    hasError: true,
    allPartsNumber: 60,
    competedPartsNumber: 30
  }, {
    id: "26f2942d-6806-44e6-b40f-adae8da5f858",
    name: "Спарсить фейк апи",
    url: "https://jsonplaceholder.typicode.com/todos/{todo}",
    typeId: TaskTypes.Api,
    statusId: TaskStatuses.Finished,
    hasError: false,
    allPartsNumber: 50,
    competedPartsNumber: 50
  })

  return {
    tasks,
  };
});
