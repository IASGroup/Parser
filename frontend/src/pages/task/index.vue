<script setup lang="ts">
import {TaskDetails} from "@/entities/tasks"

const { id } = defineProps<{ id: string }>();
</script>

<template>
  <v-container>
    <v-layout class="d-flex justify-center">
      <v-card class="w-75 rounded-0" :flat="true">
        <task-details :id="id" />
      </v-card>
    </v-layout>
  </v-container>
</template>
