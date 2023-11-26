import { axiosApi } from "@/shared/api/axiosApi";
import { Result } from "@/shared/api/result"
import { TaskListModel } from "@/entities/tasks";

export async function GetTasksAsync(): Promise<Result<Array<TaskListModel>>> {
  const response = await axiosApi.get(`api/reports/parser-tasks`)
  return response.status === 200
    ? { isSuccess: true, result: response.data as Array<TaskListModel>, errorMessage: null }
    : { isSuccess: false, result: null, errorMessage: response.data }
}
