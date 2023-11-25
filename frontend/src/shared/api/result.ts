export type Result<T> = {
  result: T | null,
  errorMessage: string | null,
  isSuccess: boolean
}
