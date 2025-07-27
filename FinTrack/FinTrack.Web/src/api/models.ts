import type { Role } from '@/models/role'

export const EMPTY_GUID = '00000000-0000-0000-0000-000000000000'

export interface WeatherForecast {
  temperatureC: number
  date: Date
  summary: string
}

export interface LoginResponse {
  accessToken: string
}

export interface ProfileDTO {
  id: string
  email: string
  roles: [Role]
}
