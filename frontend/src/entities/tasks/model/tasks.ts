import { defineStore } from "pinia";
import { ref, reactive } from "vue";
import { Task } from "@/shared/api";

export const useTaskStore = defineStore("tasks", () => {
  const tasks = ref<Array<Task>>([]);

  tasks.value.push({
    id: "id",
    name: "first task",
    status: {
      id: 1,
      description: "descript",
      key: "key",
    },
    statusId: 1,
    url: "url",
    type: {
      id: 1,
      description: "descript",
      name: "name",
    },
  });

  return {
    tasks,
  };
});
