<script setup lang="ts">
import { storeToRefs } from "pinia";
import { taskModel, TaskItem } from "@/entities/tasks";

const { tasks } = storeToRefs(taskModel());
</script>

<template>
  <v-list lines="three">
    <v-list-item v-for="task in tasks" :key="task.id">
      <TaskItem :item="task" />
    </v-list-item>
  </v-list>
</template>
