import axios from "axios"

export const axiosApi = axios.create({
  baseURL: "https://localhost:5140"
});
