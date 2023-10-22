<template>
  <p>{{props.task.name}}</p>
  <p>{{props.task.url}}</p>
</template>

<script setup>
const props = defineProps({
  task: {
    type: Object,
    required: true,
  }
})

console.log(props.task)

</script>


