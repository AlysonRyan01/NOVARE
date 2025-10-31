export interface ApiResponse<T> {
  isSuccess: boolean,
  value: T,
  errors: string[]
}
