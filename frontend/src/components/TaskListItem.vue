<script setup lang="ts">
import { ITask } from "@/entities/task"

const props = defineProps<ITask>()

</script>

<template>
  <v-row class="ma-0 pa-0">
    <v-col>{{ props.id }}</v-col>
    <v-col>{{ props.name }}</v-col>
    <v-col>
      <router-link :to="{ name: 'Task', params: { id: props.id }}">
        <v-btn>GoToTask</v-btn>
      </router-link>
    </v-col>
  </v-row>
</template>
