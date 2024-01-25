import { axiosApi } from "@/shared/api/axiosApi";
import { Result } from "@/shared/api/result"


export async function TorSetupAsync(socksPort: number, controlPort: number, controlPassword: string): Promise<Result<string>> {
  const response = await axiosApi.post(`/api/tor/setup`, {
    socksPort: socksPort,
    controlPort: controlPort,
    controlPassword: controlPassword
  })
  return response.status === 200
    ? { isSuccess: true, result: response.data as string, errorMessage: null }
    : { isSuccess: false, result: null, errorMessage: response.data }
}
