<script setup lang="ts">
defineProps({
  id: { type: String, required: true }
})
</script>

<template>
  <p>Задача парсинга: {{ id }}</p>
</template>
