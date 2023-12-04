import Routing from "./index.vue";

export const routes = [
  { path: "/", component: () => import("./home"), name: "Home" },
  { path: "/tasks/:id", component: () => import("./task"), name:"Task", props: true }
]
export { Routing };
