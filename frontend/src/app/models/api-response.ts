export interface ApiResponse<T> {
  data: T | null;
  message: string | null;
  errors: string[];
}
