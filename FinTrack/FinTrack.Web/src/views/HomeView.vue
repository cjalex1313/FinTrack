<template>
  <main class="p-4">
    <div v-if="loading" class="loading-container">
      <ProgressSpinner />
      <p>Loading your households...</p>
    </div>

    <div v-else-if="!household">
      <HouseholdSetupWizard @completed="loadHouseholds" />
    </div>

    <div v-else class="dashboard-container">
      <header class="household-header">
        <h1 class="household-title">{{ household?.name }}</h1>
      </header>

      <section class="widgets-grid">
        <DashboardStatWidget title="Monthly Income" :value="'$2,500'" size="lg" />
        <DashboardStatWidget title="Monthly Budget" :value="'$1,420'" size="lg" />
        <DashboardStatWidget title="Budget Utilisation" :value="'40%'" size="lg" />
        <DashboardStatWidget title="Current Cashflow" :value="'$8,900'" size="lg" />
      </section>
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useHouseholdApi } from '@/api/householdApi'
import type { HouseholdDTO } from '@/api/models'
import HouseholdSetupWizard from '@/components/HouseholdSetupWizard.vue'
import ProgressSpinner from 'primevue/progressspinner'
import DashboardStatWidget from '@/components/common/DashboardStatWidget.vue'

const household = ref<HouseholdDTO>()
const loading = ref(true)

const { getHouseholds } = useHouseholdApi()

const loadHouseholds = async () => {
  try {
    loading.value = true
    const data = await getHouseholds()
    if (data && data.length > 0) {
      household.value = data[0]
    }
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

.widgets-grid {
  display: grid;
  grid-template-columns: repeat(1, minmax(0, 1fr));
  gap: 1rem;
  margin-top: 0.5rem;
}

@media (min-width: 640px) {
  .widgets-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (min-width: 1024px) {
  .widgets-grid {
    grid-template-columns: repeat(4, minmax(0, 1fr));
  }
}

.household-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.25rem;
  margin-bottom: 1.5rem;
}

.household-title {
  margin: 0;
  color: #2c3e50;
  font-size: 2rem;
  font-weight: 700;
  letter-spacing: 0.2px;
}

.household-subtitle {
  margin: 0;
  color: #7f8c8d;
  font-size: 0.95rem;
}
</style>
