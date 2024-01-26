import {axiosApi} from "@/shared/api/axiosApi";
import {Result} from "@/shared/api/result"
import {TaskPartResultModel} from "@/entities/tasks";

export async function GetTaskResultAsync(taskId: string, resultId: string): Promise<Result<TaskPartResultModel>> {
  const response = await axiosApi.get(`api/reports/parser-tasks/${taskId}/results/${resultId}`)
  return response.status === 200
    ? {isSuccess: true, result: response.data as TaskPartResultModel, errorMessage: null}
    : {isSuccess: false, result: null, errorMessage: response.data}
}
