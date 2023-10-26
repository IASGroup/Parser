import {defineStore} from 'pinia'
import {ref} from "vue";
import { ITask } from "@/entities/task"

export const useTasksStore = defineStore('tasks', () => {
  const tasks = ref<Array<ITask>>([
    {
      id: '1',
      name: 'test'
    },
    {
      id: '1',
      name: 'test'
    }
  ])

  return {
    tasks
  }
})
