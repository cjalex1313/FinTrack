import { useBaseApi } from './baseApi'
import type { RecurringExpenseDTO } from './models'

export function useRecurringExpenseApi() {
  const { baseApi } = useBaseApi()

  const getRecurringExpenses = async (householdId: string) => {
    const response = await baseApi.get<RecurringExpenseDTO[]>(
      `api/recurrentexpense/household/${householdId}`,
    )
    return response.data
  }

  const getRecurringExpense = async (recurringExpenseId: string) => {
    const response = await baseApi.get<RecurringExpenseDTO>(
      `api/recurrentexpense/${recurringExpenseId}`,
    )
    return response.data
  }

  const createRecurringExpense = async (recurringExpense: RecurringExpenseDTO) => {
    const response = await baseApi.post<RecurringExpenseDTO>(
      `api/recurrentexpense`,
      recurringExpense,
    )
    return response.data
  }

  const updateRecurringExpense = async (recurringExpense: RecurringExpenseDTO) => {
    const response = await baseApi.put<RecurringExpenseDTO>(
      `api/recurrentexpense`,
      recurringExpense,
    )
    return response.data
  }

  const deleteRecurringExpense = async (recurringExpenseId: string) => {
    await baseApi.delete(`api/recurrentexpense/${recurringExpenseId}`)
  }

  return {
    getRecurringExpenses,
    getRecurringExpense,
    createRecurringExpense,
    updateRecurringExpense,
    deleteRecurringExpense,
  }
}
