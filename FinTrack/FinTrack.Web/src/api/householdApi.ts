import { useBaseApi } from './baseApi'
import type { HouseholdDTO, HouseholdMemberDTO, SetupDTO } from './models'

export function useHouseholdApi() {
  const { baseApi } = useBaseApi()

  const getHouseholds = async () => {
    const response = await baseApi.get<HouseholdMemberDTO[]>('api/household')
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

  const getPendingHouseholdInvites = async () => {
    const response = await baseApi.get<HouseholdMemberDTO[]>('api/household/pending-invites')
    return response.data
  }

  const acceptInvite = async (householdId: string) => {
    await baseApi.patch(`api/household/invites/accept/${householdId}`)
  }

  return {
    getHouseholds,
    createHousehold,
    setupHousehold,
    getPendingHouseholdInvites,
    acceptInvite,
  }
}
