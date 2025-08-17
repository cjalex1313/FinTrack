import { useHouseholdApi } from '@/api/householdApi'
import type { HouseholdDTO, HouseholdMemberDTO } from '@/api/models'
import { HouseholdMemberRole } from '@/models/role'
import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useHouseholdStore = defineStore('household', () => {
  const households = ref<HouseholdMemberDTO[] | null>(null)
  const currentHousehold = ref<HouseholdMemberDTO | null>(null)
  const loading = ref<boolean>(false)

  const setHouseholds = (newHouseholds: HouseholdMemberDTO[]) => {
    households.value = newHouseholds
    if (newHouseholds.length > 0) {
      const ownerHousehold = newHouseholds.find(
        (household) => household.role === HouseholdMemberRole.Owner,
      ) // 2 represents Owner role
      currentHousehold.value = ownerHousehold || newHouseholds[0]
    } else {
      currentHousehold.value = null
    }
  }

  const loadHouseholds = async () => {
    try {
      loading.value = true
      const householdApi = useHouseholdApi()
      const data = await householdApi.getHouseholds()
      setHouseholds(data)
    } finally {
      loading.value = false
    }
  }

  const setCurrentHousehold = (newCurrentHousehold: HouseholdMemberDTO) => {
    currentHousehold.value = newCurrentHousehold
  }

  return {
    households,
    currentHousehold,
    setHouseholds,
    setCurrentHousehold,
    loadHouseholds,
    loading,
  }
})
