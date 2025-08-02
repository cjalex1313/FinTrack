import { useBaseApi } from './baseApi'
import type { ExpenseDTO } from './models'

export function useExpenseApi() {
  const { baseApi } = useBaseApi()

  const createExpense = async (expense: ExpenseDTO) => {
    const response = await baseApi.post<ExpenseDTO>('api/expense', expense)
    return response.data
  }

  return { createExpense }
}
