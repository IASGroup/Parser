import { axiosApi } from "@/shared/api/axiosApi";
import { Result } from "@/shared/api/result"

export type StopTaskResponse = {
  parserTaskId: string
}

export async function StopTaskAsync(taskId: string): Promise<Result<StopTaskResponse>> {
  const response = await axiosApi.post(`/api/manage/parser-tasks/${taskId}/stop`)
  return response.status === 200
    ? { isSuccess: true, result: response.data as StopTaskResponse, errorMessage: null }
    : { isSuccess: false, result: null, errorMessage: response.data }
}
