<script setup lang="ts">
import { Task } from "../../../../shared/api";
const { item } = defineProps<{ item: Task }>();
</script>

<template>
  <v-card>
    {{ item.name }}
  </v-card>
</template>
