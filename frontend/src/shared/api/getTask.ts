import { axiosApi } from "@/shared/api/axiosApi";
import { Result } from "@/shared/api/result"
import { TaskDetailsModel } from "@/entities/tasks";

export async function GetTaskAsync(taskId: string): Promise<Result<TaskDetailsModel>> {
  const response = await axiosApi.get(`api/reports/parser-tasks/${taskId}`)
  return response.status === 200
    ? { isSuccess: true, result: response.data as TaskDetailsModel, errorMessage: null }
    : { isSuccess: false, result: null, errorMessage: response.data }
}
