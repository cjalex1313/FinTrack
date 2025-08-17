<template>
  <main class="p-1 md:p-4">
    <!-- Loading state -->
    <div v-if="loading" class="flex flex-col items-center justify-center min-h-[60vh] gap-4">
      <ProgressSpinner />
      <p class="text-gray-500 m-0">Loading your expense buckets...</p>
    </div>

    <!-- Expense bucket management -->
    <div v-else class="px-1 md:px-8 py-6">
      <!-- Header -->
      <header class="flex flex-col sm:flex-row sm:justify-between items-center gap-2 mb-6">
        <h1 class="m-0 text-slate-800 text-3xl font-bold tracking-[0.2px]">
          Expense Bucket Management
        </h1>
        <div class="flex gap-2">
          <Button class="whitespace-nowrap" @click="onAddExpenseBucketClick"
            >Add Expense Bucket</Button
          >
        </div>
      </header>

      <!-- Expense Buckets Section -->
      <div
        class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 max-h-[700px] overflow-y-auto"
      >
        <h2 class="text-xl font-semibold text-slate-800 mb-4">Expense Buckets</h2>
        <Skeleton v-if="expenseBuckets == null" height="100px" width="100%" />
        <div v-else>
          <div v-if="expenseBuckets.length === 0" class="bg-gray-50 rounded-lg p-8 text-center">
            <div class="text-gray-400 mb-2">
              <svg class="w-12 h-12 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"
                ></path>
              </svg>
            </div>
            <p class="text-gray-500 font-medium">No expense buckets</p>
            <p class="text-sm text-gray-400 mt-1">
              Create expense buckets to organize your spending
            </p>
          </div>
          <div v-else>
            <ExpenseBucketCard
              v-for="bucket in expenseBuckets"
              :key="bucket.id"
              :bucket="bucket"
              :show-action-buttons="true"
              @edit="editExpenseBucket"
              @delete="deleteExpenseBucket"
            />
          </div>
        </div>
      </div>

      <!-- Summary Statistics -->
      <section class="mt-8">
        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <h2 class="text-xl font-semibold text-slate-800 mb-4">Budget Summary</h2>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div
              class="bg-gradient-to-r from-red-50 to-pink-50 rounded-lg p-4 border border-red-200"
            >
              <h3 class="text-sm font-medium text-red-800 mb-1">Total Monthly Budget</h3>
              <p class="text-2xl font-bold text-red-600">${{ totalMonthlyBudget.toFixed(2) }}</p>
            </div>
            <div
              class="bg-gradient-to-r from-blue-50 to-cyan-50 rounded-lg p-4 border border-blue-200"
            >
              <h3 class="text-sm font-medium text-blue-800 mb-1">Number of Buckets</h3>
              <p class="text-2xl font-bold text-blue-600">{{ expenseBuckets?.length || 0 }}</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Expense Bucket Dialog -->
      <ExpenseBucketDialog
        v-if="showExpenseBucketDialog"
        :bucket="editingExpenseBucket"
        @save="saveExpenseBucket"
        @closed="
          () => {
            showExpenseBucketDialog = false
            editingExpenseBucket = undefined
          }
        "
      />
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { EMPTY_GUID, type ExpenseBucketDTO } from '@/api/models'
import { useExpenseApi } from '@/api/expenseApi'
import ProgressSpinner from 'primevue/progressspinner'
import { Button, Skeleton } from 'primevue'
import { useConfirm } from 'primevue/useconfirm'
import ExpenseBucketCard from '@/components/expense/ExpenseBucketCard.vue'
import ExpenseBucketDialog from '@/components/expense/ExpenseBucketDialog.vue'
import { useHouseholdStore } from '@/stores/household'

const expenseBuckets = ref<ExpenseBucketDTO[] | null>(null)
const loading = ref(true)
const showExpenseBucketDialog = ref(false)
const editingExpenseBucket = ref<ExpenseBucketDTO | undefined>()
const householdStore = useHouseholdStore()

const expenseApi = useExpenseApi()
const confirm = useConfirm()

onMounted(() => {
  loadData()
})

watch(
  () => householdStore.currentHousehold,
  () => {
    loadData()
  },
)

const totalMonthlyBudget = computed(() => {
  if (!expenseBuckets.value) return 0
  return expenseBuckets.value.reduce((total, bucket) => total + bucket.monthlyAmount, 0)
})

const loadData = async () => {
  await loadExpenseBuckets()
}

const loadExpenseBuckets = async () => {
  if (!householdStore.currentHousehold) {
    return
  }
  try {
    loading.value = true
    const buckets = await expenseApi.getBucketsForHousehold(
      householdStore.currentHousehold.householdId,
    )
    expenseBuckets.value = buckets
  } catch (error) {
    console.error('Failed to load expense buckets:', error)
  } finally {
    loading.value = false
  }
}

const onAddExpenseBucketClick = () => {
  editingExpenseBucket.value = undefined
  showExpenseBucketDialog.value = true
}

const editExpenseBucket = (bucket: ExpenseBucketDTO) => {
  editingExpenseBucket.value = bucket
  showExpenseBucketDialog.value = true
}

const deleteExpenseBucket = async (bucket: ExpenseBucketDTO) => {
  confirm.require({
    message: `Are you sure you want to delete "${bucket.name}"? This action cannot be undone.`,
    header: 'Delete Expense Bucket',
    icon: 'pi pi-exclamation-triangle',
    rejectProps: {
      label: 'Cancel',
      severity: 'secondary',
      outlined: true,
    },
    acceptProps: {
      label: 'Delete',
      severity: 'danger',
    },
    accept: async () => {
      try {
        await expenseApi.deleteExpenseBucket(bucket.id)

        if (expenseBuckets.value) {
          const index = expenseBuckets.value.findIndex((b) => b.id === bucket.id)
          if (index > -1) {
            expenseBuckets.value.splice(index, 1)
          }
        }
      } catch (error) {
        console.error('Failed to delete expense bucket:', error)
      }
    },
  })
}

const saveExpenseBucket = async (bucket: ExpenseBucketDTO) => {
  if (!householdStore.currentHousehold) {
    return
  }
  try {
    if (bucket.id === EMPTY_GUID) {
      // Adding new bucket
      bucket.householdId = householdStore.currentHousehold.householdId
      const savedBucket = await expenseApi.createExpenseBucket(bucket)
      if (expenseBuckets.value) {
        expenseBuckets.value.push(savedBucket)
      }
    } else {
      // Editing existing bucket
      const updatedBucket = await expenseApi.updateExpenseBucket(bucket)

      // Update local state
      if (expenseBuckets.value) {
        const index = expenseBuckets.value.findIndex((b) => b.id === bucket.id)
        if (index > -1) {
          expenseBuckets.value[index] = updatedBucket
        }
      }
    }
  } finally {
    showExpenseBucketDialog.value = false
    editingExpenseBucket.value = undefined
  }
}
</script>

<style scoped>
/* TailwindCSS utilities now used throughout. No component-scoped CSS needed. */
</style>
