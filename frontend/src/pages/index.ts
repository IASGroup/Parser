import { createRouter, createWebHistory } from "vue-router";
import Routing from "./index.vue";

export const routes = [
  { path: "/", component: () => import("./card-list"), name: "Home" },
  // {
  //   path: "/tasks/:id",
  //   name: "Task",
  //   component: () => import("@/views/Task.vue"),
  //   props: true,
  // },
];

export { Routing };
