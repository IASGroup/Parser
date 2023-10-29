<script setup lang="ts">
import { Task } from "../../../../shared/api";

const { item } = defineProps<{ item: Task }>();
</script>

<template>
  <v-card>
    <!-- TO-DO Сделать статусы выполнения задач -->
    <template v-slot:prepend>
      {{ item.status.key }}
    </template>

    <template v-slot:title>
      {{ item.name }}
    </template>

    <!-- TO-DO Вынести кнопки в отдельный компонент -->
    <template v-slot:actions>
      <v-btn>
        Open details
      </v-btn>
      <v-btn>
        Delete
      </v-btn>
    </template>
  </v-card>
</template>
