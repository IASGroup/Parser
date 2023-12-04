import {axiosApi} from "@/shared/api/axiosApi";
import {Result} from "@/shared/api/result"
import {TaskDetailsResultModel} from "@/entities/tasks";

export async function GetTaskResultsAsync(taskId: string): Promise<Result<Array<TaskDetailsResultModel>>> {
  const response = await axiosApi.get(`api/reports/parser-tasks/${taskId}/results`)
  return response.status === 200
    ? {isSuccess: true, result: response.data as Array<TaskDetailsResultModel>, errorMessage: null}
    : {isSuccess: false, result: null, errorMessage: response.data}
}
