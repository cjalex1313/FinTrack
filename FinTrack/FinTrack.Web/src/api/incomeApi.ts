import { format } from 'date-fns'
import { useBaseApi } from './baseApi'
import type { IncomesForMonthDTO } from './models'

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

  return { getIncomesForMonth }
}
