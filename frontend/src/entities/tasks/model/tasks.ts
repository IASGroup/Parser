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
    statusId: TaskStatuses.Created
  })

  return {
    tasks,
  };
});
