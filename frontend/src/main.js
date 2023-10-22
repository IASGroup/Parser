import { createApp } from 'vue'
import { createPinia } from 'pinia'

import "@mdi/font/css/materialdesignicons.css"
import 'vuetify/styles'
import {createVuetify} from "vuetify"
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import * as labsComponents from "vuetify/lib/labs/components";

import App from './App.vue'

const vuetify= createVuetify({
    icons: {
        iconfont: 'mdi',
    },
    components: {
        ...components,
        ...labsComponents
    },
    directives,
})


const app= createApp(App)

app.use(createPinia()).use(vuetify)

app.mount('#app')
