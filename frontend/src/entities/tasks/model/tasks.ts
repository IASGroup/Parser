import {defineStore} from "pinia";
import {ref} from "vue";
import {TaskListModel} from "./models";

export const useTaskStore = defineStore("tasks", () => {
  const tasks = ref<Array<TaskListModel>>([]);
  return {
    tasks,
  };
});
