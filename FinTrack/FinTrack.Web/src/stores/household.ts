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
      // Attempt to retrieve the current household id from localStorage
      const storedHouseholdId = localStorage.getItem('currentHouseholdId')
      let selectedHousehold
      if (storedHouseholdId) {
        // Assuming each household has a 'householdId' property
        selectedHousehold = newHouseholds.find(
          (household) => household.householdId?.toString() === storedHouseholdId,
        )
      }
      // If not found in localStorage or not present in the array, use original workflow
      if (!selectedHousehold) {
        selectedHousehold =
          newHouseholds.find((household) => household.role === HouseholdMemberRole.Owner) ||
          newHouseholds[0]
      }
      currentHousehold.value = selectedHousehold
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
    // Store the currently selected household's id in localStorage for persistence
    if (newCurrentHousehold && newCurrentHousehold.householdId) {
      localStorage.setItem('currentHouseholdId', newCurrentHousehold.householdId.toString())
    }
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
