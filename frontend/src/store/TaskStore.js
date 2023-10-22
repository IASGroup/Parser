import {defineStore} from 'pinia'
import {onMounted, ref} from 'vue'

export const useTaskStore = defineStore('taskStore', () => {
    const tasks = ref([]);

    onMounted(() => {
        tasks.value.push({
                url: 'https://en.wikipedia.org/wiki/Transhumanism/page/{page}/attribute',
                type: 1,
                name: 'Спарсить погоду'
            },
            {
                url: 'https://en.wikipedia.org/wiki/Transhumanism/page/{page}/attribute',
                type: 1,
                name: 'Спарсить погоду1'
            }
        );
    });

    return {
        tasks
    }
})
