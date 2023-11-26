import { axiosApi } from "@/shared/api/axiosApi";
import { Result } from "@/shared/api/result"

export type RunTaskResponse = {
  parserTaskId: string
}

export async function RunTaskAsync(taskId: string): Promise<Result<RunTaskResponse>> {
  const response = await axiosApi.post(`/api/manage/parser-tasks/${taskId}/run`)
  return response.status === 200
    ? { isSuccess: true, result: response.data as RunTaskResponse, errorMessage: null }
    : { isSuccess: false, result: null, errorMessage: response.data }
}
