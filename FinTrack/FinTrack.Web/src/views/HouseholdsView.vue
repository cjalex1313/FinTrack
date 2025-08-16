<template>
  <div class="max-w-4xl mx-auto p-6">
    <h1 class="text-3xl font-bold text-gray-900 mb-8">Households</h1>
  </div>
</template>

<script setup lang="ts">
import { useHouseholdApi } from '@/api/householdApi'
import type { HouseholdMemberDTO } from '@/api/models'
import { onMounted, ref } from 'vue'

const householdApi = useHouseholdApi()
const householdMembers = ref<HouseholdMemberDTO[]>([])

onMounted(() => {
  loadHouseholdData()
})

const loadHouseholdData = async () => {
  await Promise.all([loadOwnedHouseholds()])
}

const loadOwnedHouseholds = async () => {
  const result = await householdApi.getHouseholds()
  householdMembers.value = result
}
</script>
