<template>
  <main class="p-1 md:p-4">
    <!-- Loading state -->
    <div v-if="loading" class="flex flex-col items-center justify-center min-h-[60vh] gap-4">
      <ProgressSpinner />
      <p class="text-gray-500 m-0">Loading your recurring expenses...</p>
    </div>

    <!-- Recurring Expense management -->
    <div v-else class="px-1 md:px-8 py-6">
      <!-- Header -->
      <header class="flex flex-col sm:flex-row sm:justify-between items-center gap-2 mb-6">
        <h1 class="m-0 text-slate-800 text-3xl font-bold tracking-[0.2px]">Recurring Expenses</h1>
        <div class="flex gap-2">
          <Button class="whitespace-nowrap" @click="onAddRecurringExpenseClick"
            >Add Recurring Expense</Button
          >
        </div>
      </header>

      <!-- Recurring Expenses Section -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
        <h2 class="text-xl font-semibold text-slate-800 mb-4">Recurring Expenses</h2>
        <Skeleton v-if="recurringExpenses == null" height="100px" width="100%" />
        <div v-else>
          <div v-if="recurringExpenses.length === 0" class="bg-gray-50 rounded-lg p-8 text-center">
            <div class="text-gray-400 mb-2">
              <svg class="w-12 h-12 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
                ></path>
              </svg>
            </div>
            <p class="text-gray-500 font-medium">No recurring expenses</p>
            <p class="text-sm text-gray-400 mt-1">Add recurring expenses to see them here</p>
          </div>
          <div v-else>
            <RecurringExpenseCard
              v-for="recurringExpense in recurringExpenses"
              :key="recurringExpense.id"
              :expense="recurringExpense"
              :expense-buckets="expenseBuckets || []"
              :show-action-buttons="true"
              @edit="editRecurringExpense"
              @delete="deleteRecurringExpense"
            />
          </div>
        </div>
      </div>

      <!-- Summary Statistics -->
      <section class="mt-8" v-if="recurringExpenses != null">
        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <h2 class="text-xl font-semibold text-slate-800 mb-4">Recurring Expense Summary</h2>
          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
            <div
              class="bg-gradient-to-r from-blue-50 to-indigo-50 rounded-lg p-4 border border-blue-200"
            >
              <h3 class="text-sm font-medium text-blue-800 mb-1">Recurring Expenses</h3>
              <p class="text-2xl font-bold text-blue-600">{{ recurringExpenses?.length || 0 }}</p>
            </div>
            <div
              class="bg-gradient-to-r from-purple-50 to-violet-50 rounded-lg p-4 border border-purple-200"
            >
              <h3 class="text-sm font-medium text-purple-800 mb-1">Total Recurring</h3>
              <p class="text-2xl font-bold text-purple-600">
                ${{ totalRecurringExpenses.toFixed(2) }}
              </p>
            </div>
            <div
              class="bg-gradient-to-r from-green-50 to-emerald-50 rounded-lg p-4 border border-green-200"
            >
              <h3 class="text-sm font-medium text-green-800 mb-1">Monthly Average</h3>
              <p class="text-2xl font-bold text-green-600">${{ monthlyAverage.toFixed(2) }}</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Recurring Expense Dialog -->
      <RecurringExpenseDialog
        v-if="showRecurringExpenseDialog"
        :household-id="householdStore.currentHousehold!.householdId"
        :expense-buckets="expenseBuckets || []"
        :recurring-expense="editingRecurringExpense"
        @save="saveRecurringExpense"
        @closed="
          () => {
            showRecurringExpenseDialog = false
            editingRecurringExpense = undefined
          }
        "
      />
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { type ExpenseBucketDTO, type RecurringExpenseDTO } from '@/api/models'
import { useRecurringExpenseApi } from '@/api/recurringExpenseApi'
import ProgressSpinner from 'primevue/progressspinner'
import { Button, Skeleton } from 'primevue'
import { useConfirm } from 'primevue/useconfirm'
import RecurringExpenseCard from '@/components/expense/recurring/RecurringExpenseCard.vue'
import RecurringExpenseDialog from '@/components/expense/recurring/RecurringExpenseDialog.vue'
import { useHouseholdStore } from '@/stores/household'
import getMonthlyEquivalent from '@/helpers/getMonthlyEquivalent'

const recurringExpenses = ref<RecurringExpenseDTO[] | null>(null)
const loading = ref(true)
const showRecurringExpenseDialog = ref(false)
const editingRecurringExpense = ref<RecurringExpenseDTO | undefined>()
const householdStore = useHouseholdStore()
const expenseBuckets = ref<ExpenseBucketDTO[] | null>(null)

const recurringExpenseApi = useRecurringExpenseApi()
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

const totalRecurringExpenses = computed(() => {
  if (!recurringExpenses.value) return 0
  return recurringExpenses.value.reduce((total, expense) => total + expense.amount, 0)
})

const monthlyAverage = computed(() => {
  if (!recurringExpenses.value || recurringExpenses.value.length === 0) return 0

  // Calculate monthly equivalent for each recurring expense based on its recurrence type
  const totalMonthlyEquivalent = recurringExpenses.value.reduce((total, expense) => {
    return total + getMonthlyEquivalent(expense.recurrence, expense.amount)
  }, 0)

  return totalMonthlyEquivalent
})

const loadData = async () => {
  await Promise.all([loadExpenseBuckets(), loadRecurringExpenses()])
}

const loadExpenseBuckets = async () => {
  if (!householdStore.currentHousehold) {
    return
  }
  // We need to import the expense API to get buckets
  const { useExpenseApi } = await import('@/api/expenseApi')
  const expenseApi = useExpenseApi()
  expenseBuckets.value = await expenseApi.getBucketsForHousehold(
    householdStore.currentHousehold.householdId,
  )
}

const loadRecurringExpenses = async () => {
  if (!householdStore.currentHousehold) {
    return
  }
  try {
    loading.value = true
    const recurringExpensesData = await recurringExpenseApi.getRecurringExpenses(
      householdStore.currentHousehold.householdId,
    )
    recurringExpenses.value = recurringExpensesData
  } catch (error) {
    console.error('Failed to load recurring expenses:', error)
  } finally {
    loading.value = false
  }
}

const onAddRecurringExpenseClick = () => {
  editingRecurringExpense.value = undefined
  showRecurringExpenseDialog.value = true
}

const editRecurringExpense = (recurringExpense: RecurringExpenseDTO) => {
  editingRecurringExpense.value = recurringExpense
  showRecurringExpenseDialog.value = true
}

const deleteRecurringExpense = async (recurringExpense: RecurringExpenseDTO) => {
  confirm.require({
    message: `Are you sure you want to delete "${recurringExpense.description || 'this recurring expense'}"? This action cannot be undone.`,
    header: 'Delete Recurring Expense',
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
        await recurringExpenseApi.deleteRecurringExpense(recurringExpense.id)

        if (recurringExpenses.value) {
          const index = recurringExpenses.value.findIndex((e) => e.id === recurringExpense.id)
          if (index > -1) {
            recurringExpenses.value.splice(index, 1)
          }
        }
      } catch (error) {
        console.error('Failed to delete recurring expense:', error)
      }
    },
  })
}

const saveRecurringExpense = async (recurringExpense: RecurringExpenseDTO) => {
  if (!householdStore.currentHousehold) {
    return
  }
  try {
    if (recurringExpense.id === '00000000-0000-0000-0000-000000000000') {
      // Adding new recurring expense
      recurringExpense.householdId = householdStore.currentHousehold.householdId
      const savedRecurringExpense =
        await recurringExpenseApi.createRecurringExpense(recurringExpense)
      if (recurringExpenses.value) {
        recurringExpenses.value.push(savedRecurringExpense)
      }
    } else {
      // Editing existing recurring expense
      const updatedRecurringExpense =
        await recurringExpenseApi.updateRecurringExpense(recurringExpense)

      // Update local state
      if (recurringExpenses.value) {
        const index = recurringExpenses.value.findIndex((e) => e.id === recurringExpense.id)
        if (index > -1) {
          recurringExpenses.value[index] = updatedRecurringExpense
        }
      }
    }
  } finally {
    showRecurringExpenseDialog.value = false
    editingRecurringExpense.value = undefined
  }
}
</script>

<style scoped>
/* TailwindCSS utilities now used throughout. No component-scoped CSS needed. */
</style>
