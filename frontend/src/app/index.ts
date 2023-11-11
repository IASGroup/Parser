import { createApp } from "vue";
import { router, store, vuetify } from "./providers";
import App from "./index.vue";

const initApp = createApp(App).use(router).use(store).use(vuetify);
export const app = initApp;
