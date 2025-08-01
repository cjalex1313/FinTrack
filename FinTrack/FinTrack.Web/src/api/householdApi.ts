import { useBaseApi } from './baseApi'
import type { HouseholdDTO, SetupDTO } from './models'

export function useHouseholdApi() {
  const { baseApi } = useBaseApi()

  const getHouseholds = async () => {
    const response = await baseApi.get<HouseholdDTO[]>('api/household')
    return response.data
  }

  const createHousehold = async (name: string) => {
    const response = await baseApi.post<HouseholdDTO>('api/household', {
      name,
    })
    return response.data
  }

  const setupHousehold = async (dto: SetupDTO) => {
    await baseApi.post('api/household/setup', dto)
  }

  return { getHouseholds, createHousehold, setupHousehold }
}
