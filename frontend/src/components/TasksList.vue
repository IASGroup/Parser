<script setup lang="ts">
import TaskItem from "@/components/TaskListItem.vue";
import {useTasksStore} from "@/store/tasks";

const tasksStore = useTasksStore();
</script>

<template>
  <p>Список задач</p>
  <v-list>
    <v-list-item v-for="task in tasksStore.tasks" :key="task.id">
      <TaskItem :id="task.id" :name="task.name"/>
    </v-list-item>
  </v-list>
</template>
