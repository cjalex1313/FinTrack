import { useHouseholdApi } from '@/api/householdApi'
import type { HouseholdDTO } from '@/api/models'
import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useHouseholdStore = defineStore('hosuehold', () => {
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

  const setCurrentHousehold = (newCurrentHousehold: HouseholdDTO) => {
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
