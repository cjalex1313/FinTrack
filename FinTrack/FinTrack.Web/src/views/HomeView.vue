<template>
  <main class="p-4">
    <div v-if="loading" class="loading-container">
      <ProgressSpinner />
      <p>Loading your households...</p>
    </div>

    <div v-else-if="households.length === 0">
      <HouseholdSetupWizard @completed="loadHouseholds" />
    </div>

    <div v-else class="dashboard-container">
      <h1>Welcome to FinTrack!</h1>
      <p>You have {{ households.length }} household(s) set up.</p>
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useHouseholdApi } from '@/api/householdApi'
import type { HouseholdDTO } from '@/api/models'
import HouseholdSetupWizard from '@/components/HouseholdSetupWizard.vue'
import ProgressSpinner from 'primevue/progressspinner'

const households = ref<HouseholdDTO[]>([])
const loading = ref(true)

const { getHouseholds } = useHouseholdApi()

const loadHouseholds = async () => {
  try {
    loading.value = true
    const data = await getHouseholds()
    households.value = data
  } catch (error) {
    console.error('Failed to load households:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadHouseholds()
})
</script>

<style scoped>
.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 60vh;
  gap: 1rem;
}

.loading-container p {
  color: #7f8c8d;
  margin: 0;
}

.dashboard-container {
  padding: 2rem;
  text-align: center;
}

.dashboard-container h1 {
  color: #2c3e50;
  margin-bottom: 1rem;
}

.dashboard-container p {
  color: #7f8c8d;
  font-size: 1.1rem;
}
</style>
