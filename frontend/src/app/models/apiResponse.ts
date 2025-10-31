export interface ApiResponse<T> {
  IsSuccess: boolean,
  Value: T,
  Errors: string[]
}
