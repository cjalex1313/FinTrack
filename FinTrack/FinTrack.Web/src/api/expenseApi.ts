import { format } from 'date-fns'
import { useBaseApi } from './baseApi'
import type { ExpenseBucketDTO, ExpenseDTO } from './models'

export function useExpenseApi() {
  const { baseApi } = useBaseApi()

  const createExpense = async (expense: ExpenseDTO) => {
    const response = await baseApi.post<ExpenseDTO>('api/expense', expense)
    return response.data
  }

  const updateExpense = async (expense: ExpenseDTO) => {
    const response = await baseApi.put<ExpenseDTO>('api/expense', expense)
    return response.data
  }

  const deleteExpense = async (expenseId: string) => {
    await baseApi.delete(`api/expense/${expenseId}`)
  }

  const getBucketsForHousehold = async (householdId: string) => {
    const response = await baseApi.get<ExpenseBucketDTO[]>('api/expense/buckets', {
      params: {
        householdId,
      },
    })
    return response.data
  }

  const createExpenseBucket = async (expenseBucket: ExpenseBucketDTO) => {
    const response = await baseApi.post<ExpenseBucketDTO>('api/expense/bucket', expenseBucket)
    return response.data
  }

  const updateExpenseBucket = async (expenseBucket: ExpenseBucketDTO) => {
    const response = await baseApi.put<ExpenseBucketDTO>('api/expense/bucket', expenseBucket)
    return response.data
  }

  const deleteExpenseBucket = async (expenseBucketId: string) => {
    await baseApi.delete(`api/expense/bucket/${expenseBucketId}`)
  }

  const getExpensesForMonth = async (householdId: string, dateInMonth: Date) => {
    const formatedDate = format(dateInMonth, 'yyyy-MM-dd')
    const response = await baseApi.get<ExpenseDTO[]>('api/expense', {
      params: {
        householdId,
        dateInMonth: formatedDate,
      },
    })
    return response.data
  }

  return {
    createExpense,
    getBucketsForHousehold,
    getExpensesForMonth,
    createExpenseBucket,
    updateExpenseBucket,
    deleteExpenseBucket,
    updateExpense,
    deleteExpense,
  }
}
