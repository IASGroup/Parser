<template>
  <v-list>
    <v-list-item
        v-for="(item, i) in tasks" :key="i" :value="item">
      <v-list-item-title @click="selectedTask(item)">{{ item.name }}</v-list-item-title>
    </v-list-item>

  </v-list>
</template>

<script setup>
import {storeToRefs} from "pinia";
import {useTaskStore} from "@/store/TaskStore"

const {tasks} = storeToRefs(useTaskStore());
const emit = defineEmits(['selected']);

const selectedTask = (task) => {
  emit('selected', task)
};

</script>