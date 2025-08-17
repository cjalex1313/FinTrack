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

  const getHouseholdMembers = async (householdId: string) => {
    const response = await baseApi.get<HouseholdMemberDTO[]>(
      `api/household/${householdId}/members/all`,
    )
    return response.data
  }

  const inviteMember = async (householdId: string, email: string) => {
    await baseApi.post(`api/household/invite`, { householdId, email })
  }

  const removeHouseholdMember = async (householdId: string, email: string) => {
    await baseApi.delete(`api/household/member`, { data: { householdId, email } })
  }

  const rejectHouseholdInvite = async (householdId: string) => {
    await baseApi.patch(`api/household/${householdId}/invite/reject`)
  }

  return {
    getHouseholds,
    createHousehold,
    setupHousehold,
    getPendingHouseholdInvites,
    acceptInvite,
    getHouseholdMembers,
    inviteMember,
    removeHouseholdMember,
    rejectHouseholdInvite,
  }
}
