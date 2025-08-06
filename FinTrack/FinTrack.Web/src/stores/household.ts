import { useHouseholdApi } from '@/api/householdApi'
import type { HouseholdDTO } from '@/api/models'
import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useHouseholdStore = defineStore('household', () => {
  const households = ref<HouseholdDTO[] | null>(null)
  const currentHousehold = ref<HouseholdDTO | null>(null)
  const loading = ref<boolean>(false)

  const setHouseholds = (newHouseholds: HouseholdDTO[]) => {
    households.value = newHouseholds
    if (newHouseholds.length > 0) {
      currentHousehold.value = newHouseholds[0]
    } else {
      currentHousehold.value = null
    }
  }

  const setIsLoading = (newLoading: boolean) => {
    loading.value = newLoading
  }

  const setCurrentHousehold = (newCurrentHousehold: HouseholdDTO) => {
    currentHousehold.value = newCurrentHousehold
  }

  return {
    households,
    currentHousehold,
    setHouseholds,
    setCurrentHousehold,
    loading,
    setIsLoading,
  }
})
