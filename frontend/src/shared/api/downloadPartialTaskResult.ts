import {Result} from "@/shared/api/result";
import {axiosApi} from "@/shared/api/axiosApi";

export async function DownloadPartialTaskResult(taskId: string, resultId: string): Promise<Result<Blob>> {
  const response = await axiosApi.get(`api/reports/parser-tasks/${taskId}/results/${resultId}/download`);
  return response.status === 200
    ? { isSuccess: true, result: new Blob([JSON.stringify(response.data)]), errorMessage: null }
    : { isSuccess: false, result: null, errorMessage: response.data }
}
