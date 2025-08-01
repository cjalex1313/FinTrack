import type { RecurrenceType } from '@/models/recurrenceType'
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

export interface HouseholdDTO {
  id: string
  name: string
}

export interface SetupDTO {
  household: HouseholdDTO
  recurringIncomes: RecurringIncomeDTO[]
}

export interface RecurringIncomeDTO {
  id: string
  householdId: string
  amount: number
  startDate: string
  endDate?: string | null
  recurrence: RecurrenceType
  description?: string | null
}
