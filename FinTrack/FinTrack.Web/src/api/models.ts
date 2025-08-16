import type { RecurrenceType } from '@/models/recurrenceType'
import type { HouseholdMemberRole, HouseholdMemberStatus, Role } from '@/models/role'

export const EMPTY_GUID = '00000000-0000-0000-0000-000000000000'

export interface WeatherForecast {
  temperatureC: number
  date: Date
  summary: string
}

export interface LoginResponse {
  accessToken: string
  passwordSetNeeded: boolean
}

export interface ProfileDTO {
  id: string
  email: string
  firstName: string | null
  lastName: string | null
  roles: [Role]
}

export interface HouseholdDTO {
  id: string
  name: string
}

export interface SetupDTO {
  household: HouseholdDTO
  recurringIncomes: RecurringIncomeDTO[]
  expenseBuckets: ExpenseBucketDTO[]
  invites: HouseholdInviteDTO[]
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

export interface ExpenseBucketDTO {
  id: string // Guid
  householdId: string // Guid
  name: string // required string
  monthlyAmount: number // decimal
  description: string | null // string?
}

export interface ExpenseDTO {
  id: string
  householdId: string
  expenseBucketId: string | null
  amount: number
  date: string
  description: string | null
}

export interface IncomesForMonthDTO {
  oneTimeIncomes: OneTimeIncomeDTO[]
  recurringIncomes: RecurringIncomeDTO[]
}

export interface OneTimeIncomeDTO {
  id: string // Guid
  householdId: string // Guid
  amount: number // decimal
  date: string // ISO date (from DateOnly)
  description?: string | null
}

export interface ForgotPasswordDTO {
  email: string
}

export interface ResetPasswordDTO {
  userId: string
  token: string
  password: string
}

export interface ChangePasswordDTO {
  oldPassword: string
  newPassword: string
}

export interface UpdateProfileNamesDTO {
  firstName: string | null
  lastName: string | null
}

export interface RecurringExpenseDTO {
  id: string
  householdId: string
  amount: number
  nextDate: string
  recurrence: RecurrenceType
  description?: string | null
  expenseBucketId: string | null
}

export interface HouseholdInviteDTO {
  householdId: string
  email: string
}

export interface HouseholdMemberDTO {
  householdId: string
  householdName: string
  userEmail: string
  userFirstName: string | null
  userLastName: string | null
  role: HouseholdMemberRole
  status: HouseholdMemberStatus
}
