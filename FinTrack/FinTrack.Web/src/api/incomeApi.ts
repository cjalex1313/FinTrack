import { format } from 'date-fns'
import { useBaseApi } from './baseApi'
import type { IncomesForMonthDTO, OneTimeIncomeDTO, RecurringIncomeDTO } from './models'

export function useIncomeApi() {
  const { baseApi } = useBaseApi()

  const getIncomesForMonth = async (dateInMonth: Date) => {
    const formatedDate = format(dateInMonth, 'yyyy-MM-dd')
    const response = await baseApi.get<IncomesForMonthDTO>('api/income/for-month', {
      params: {
        dateInMonth: formatedDate,
      },
    })
    return response.data
  }

  const addOneTimeIncome = async (income: OneTimeIncomeDTO) => {
    const response = await baseApi.post<OneTimeIncomeDTO>('api/income/one-time', income)
    return response.data
  }

  const addRecurringIncome = async (income: RecurringIncomeDTO) => {
    const response = await baseApi.post<RecurringIncomeDTO>('api/income/recurring', income)
    return response.data
  }

  const updateOneTimeIncome = async (income: OneTimeIncomeDTO) => {
    const response = await baseApi.put<OneTimeIncomeDTO>('api/income/one-time', income)
    return response.data
  }

  const updateRecurringIncome = async (income: RecurringIncomeDTO) => {
    const response = await baseApi.put<RecurringIncomeDTO>('api/income/recurring', income)
    return response.data
  }

  const deleteOneTimeIncome = async (id: string) => {
    const response = await baseApi.delete<OneTimeIncomeDTO>(`api/income/one-time/${id}`)
    return response.data
  }

  const deleteRecurringIncome = async (id: string) => {
    const response = await baseApi.delete<RecurringIncomeDTO>(`api/income/recurring/${id}`)
    return response.data
  }

  return {
    getIncomesForMonth,
    addOneTimeIncome,
    addRecurringIncome,
    updateOneTimeIncome,
    updateRecurringIncome,
    deleteOneTimeIncome,
    deleteRecurringIncome,
  }
}
