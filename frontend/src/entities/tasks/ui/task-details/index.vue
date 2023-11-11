<script setup lang="ts">
import { Task } from "@/entities/tasks";
const { item } = defineProps<{ item: Task }>();
</script>

<template>
  <v-card>
    {{ item.name }}
  </v-card>
</template>
