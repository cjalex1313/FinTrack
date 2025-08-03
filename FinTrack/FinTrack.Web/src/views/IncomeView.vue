<template>
  <main class="p-1 md:p-4">
    <!-- Loading state -->
    <div v-if="loading" class="flex flex-col items-center justify-center min-h-[60vh] gap-4">
      <ProgressSpinner />
      <p class="text-gray-500 m-0">Loading your incomes...</p>
    </div>

    <!-- Income management -->
    <div v-else class="px-1 md:px-8 py-6">
      <!-- Header -->
      <header class="flex flex-col sm:flex-row sm:justify-between items-center gap-2 mb-6">
        <h1 class="m-0 text-slate-800 text-3xl font-bold tracking-[0.2px]">Income Management</h1>
        <div class="flex gap-2">
          <Button class="whitespace-nowrap" @click="onAddOneTimeIncomeClick"
            >Add One-time Income</Button
          >
          <Button class="whitespace-nowrap" @click="onAddRecurringIncomeClick"
            >Add Recurring Income</Button
          >
        </div>
      </header>

      <!-- Month Selection -->
      <section class="mb-6">
        <MonthPicker v-model="selectedMonth" :loading="loading" @change="onMonthChange" />
      </section>

      <!-- Income Sections -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- One-time Incomes Section -->
        <div
          class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 max-h-[700px] overflow-y-auto"
        >
          <h2 class="text-xl font-semibold text-slate-800 mb-4">One-time Incomes</h2>
          <Skeleton v-if="oneTimeIncomes == null" height="100px" width="100%" />
          <div v-else>
            <div v-if="oneTimeIncomes.length === 0" class="bg-gray-50 rounded-lg p-8 text-center">
              <div class="text-gray-400 mb-2">
                <svg
                  class="w-12 h-12 mx-auto"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"
                  ></path>
                </svg>
              </div>
              <p class="text-gray-500 font-medium">No one-time incomes</p>
              <p class="text-sm text-gray-400 mt-1">Add one-time income sources to see them here</p>
            </div>
            <div v-else>
              <OneTimeIncomeCard
                v-for="income in oneTimeIncomes"
                :key="income.id"
                :income="income"
                :show-date="true"
                :show-action-buttons="true"
                @edit="editOneTimeIncome"
                @delete="deleteOneTimeIncome"
              />
            </div>
          </div>
        </div>

        <!-- Recurring Incomes Section -->
        <div
          class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 max-h-[700px] overflow-y-auto"
        >
          <h2 class="text-xl font-semibold text-slate-800 mb-4">Recurring Incomes</h2>
          <Skeleton v-if="recurringIncomes == null" height="100px" width="100%" />
          <div v-else>
            <div v-if="recurringIncomes.length === 0" class="bg-gray-50 rounded-lg p-8 text-center">
              <div class="text-gray-400 mb-2">
                <svg
                  class="w-12 h-12 mx-auto"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
                  ></path>
                </svg>
              </div>
              <p class="text-gray-500 font-medium">No recurring incomes</p>
              <p class="text-sm text-gray-400 mt-1">
                Add recurring income sources to see them here
              </p>
            </div>
            <div v-else>
              <RecurringIncomeCard
                v-for="income in recurringIncomes"
                :key="income.id"
                :income="income"
                :show-date="true"
                :show-action-buttons="true"
                @edit="editRecurringIncome"
                @delete="deleteRecurringIncome"
              />
            </div>
          </div>
        </div>
      </div>

      <!-- Summary Statistics -->
      <section class="mt-8">
        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <h2 class="text-xl font-semibold text-slate-800 mb-4">Income Summary</h2>
          <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <div
              class="bg-gradient-to-r from-green-50 to-emerald-50 rounded-lg p-4 border border-green-200"
            >
              <h3 class="text-sm font-medium text-green-800 mb-1">Total One-time Income</h3>
              <p class="text-2xl font-bold text-green-600">${{ totalOneTimeIncome.toFixed(2) }}</p>
            </div>
            <div
              class="bg-gradient-to-r from-blue-50 to-cyan-50 rounded-lg p-4 border border-blue-200"
            >
              <h3 class="text-sm font-medium text-blue-800 mb-1">Monthly Recurring Income</h3>
              <p class="text-2xl font-bold text-blue-600">
                ${{ monthlyRecurringIncome.toFixed(2) }}
              </p>
            </div>
            <div
              class="bg-gradient-to-r from-purple-50 to-indigo-50 rounded-lg p-4 border border-purple-200"
            >
              <h3 class="text-sm font-medium text-purple-800 mb-1">Total Monthly Income</h3>
              <p class="text-2xl font-bold text-purple-600">${{ totalMonthlyIncome.toFixed(2) }}</p>
            </div>
          </div>
        </div>
      </section>

      <!-- One-time Income Dialog -->
      <OneTimeIncomeDialog
        v-if="showOneTimeIncomeDialog"
        :income="editingOneTimeIncome"
        @save="saveOneTimeIncome"
        @closed="
          () => {
            showOneTimeIncomeDialog = false
            editingOneTimeIncome = undefined
          }
        "
      />

      <!-- Recurring Income Dialog -->
      <RecurringIncomeDialog
        v-if="showRecurringIncomeDialog"
        :income="editingRecurringIncome"
        @save="saveRecurringIncome"
        @closed="
          () => {
            showRecurringIncomeDialog = false
            editingRecurringIncome = undefined
          }
        "
      />
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { EMPTY_GUID, type OneTimeIncomeDTO, type RecurringIncomeDTO } from '@/api/models'
import { useIncomeApi } from '@/api/incomeApi'
import ProgressSpinner from 'primevue/progressspinner'
import { Button, Skeleton } from 'primevue'
import { useConfirm } from 'primevue/useconfirm'
import OneTimeIncomeCard from '@/components/income/OneTimeIncomeCard.vue'
import RecurringIncomeCard from '@/components/income/RecurringIncomeCard.vue'
import OneTimeIncomeDialog from '@/components/income/OneTimeIncomeDialog.vue'
import RecurringIncomeDialog from '@/components/income/RecurringIncomeDialog.vue'
import MonthPicker from '@/components/common/MonthPicker.vue'
import getMonthlyEquivalent from '@/helpers/getMonthlyEquivalent'

const oneTimeIncomes = ref<OneTimeIncomeDTO[] | null>(null)
const recurringIncomes = ref<RecurringIncomeDTO[] | null>(null)
const loading = ref(true)
const showOneTimeIncomeDialog = ref(false)
const showRecurringIncomeDialog = ref(false)
const editingOneTimeIncome = ref<OneTimeIncomeDTO | undefined>()
const editingRecurringIncome = ref<RecurringIncomeDTO | undefined>()
const selectedMonth = ref<Date>(new Date())

const incomeApi = useIncomeApi()
const confirm = useConfirm()

onMounted(() => {
  loadIncomes()
})

const totalOneTimeIncome = computed(() => {
  if (!oneTimeIncomes.value) return 0
  return oneTimeIncomes.value.reduce((total, income) => total + income.amount, 0)
})

const monthlyRecurringIncome = computed(() => {
  if (!recurringIncomes.value) return 0
  return recurringIncomes.value.reduce((total, income) => {
    return total + getMonthlyEquivalent(income.recurrence, income.amount)
  }, 0)
})

const totalMonthlyIncome = computed(() => {
  return totalOneTimeIncome.value + monthlyRecurringIncome.value
})

const loadIncomes = async (month?: Date) => {
  try {
    loading.value = true
    const targetMonth = month || selectedMonth.value
    const incomes = await incomeApi.getIncomesForMonth(targetMonth)
    oneTimeIncomes.value = incomes.oneTimeIncomes
    recurringIncomes.value = incomes.recurringIncomes
  } catch (error) {
    console.error('Failed to load incomes:', error)
  } finally {
    loading.value = false
  }
}

const onMonthChange = (month: Date) => {
  selectedMonth.value = month
  loadIncomes(month)
}

const onAddOneTimeIncomeClick = () => {
  editingOneTimeIncome.value = undefined
  showOneTimeIncomeDialog.value = true
}

const onAddRecurringIncomeClick = () => {
  editingRecurringIncome.value = undefined
  showRecurringIncomeDialog.value = true
}

const editOneTimeIncome = (income: OneTimeIncomeDTO) => {
  editingOneTimeIncome.value = income
  showOneTimeIncomeDialog.value = true
}

const editRecurringIncome = (income: RecurringIncomeDTO) => {
  editingRecurringIncome.value = income
  showRecurringIncomeDialog.value = true
}

const deleteOneTimeIncome = async (income: OneTimeIncomeDTO) => {
  confirm.require({
    message: `Are you sure you want to delete "${income.description}"? This action cannot be undone.`,
    header: 'Delete One-time Income',
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
        await incomeApi.deleteOneTimeIncome(income.id)

        if (oneTimeIncomes.value) {
          const index = oneTimeIncomes.value.findIndex((i) => i.id === income.id)
          if (index > -1) {
            oneTimeIncomes.value.splice(index, 1)
          }
        }
      } catch (error) {
        console.error('Failed to delete one-time income:', error)
      }
    },
  })
}

const deleteRecurringIncome = async (income: RecurringIncomeDTO) => {
  confirm.require({
    message: `Are you sure you want to delete "${income.description}"? This action cannot be undone.`,
    header: 'Delete Recurring Income',
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
        await incomeApi.deleteRecurringIncome(income.id)

        if (recurringIncomes.value) {
          const index = recurringIncomes.value.findIndex((i) => i.id === income.id)
          if (index > -1) {
            recurringIncomes.value.splice(index, 1)
          }
        }
      } catch (error) {
        console.error('Failed to delete recurring income:', error)
      }
    },
  })
}

const saveOneTimeIncome = async (income: OneTimeIncomeDTO) => {
  try {
    if (income.id === EMPTY_GUID) {
      // Adding new income
      const savedIncome = await incomeApi.addOneTimeIncome(income)
      if (oneTimeIncomes.value) {
        oneTimeIncomes.value.push(savedIncome)
      }
    } else {
      // Editing existing income
      var updatedIncome = await incomeApi.updateOneTimeIncome(income)

      // For now, just update local state
      if (oneTimeIncomes.value) {
        const index = oneTimeIncomes.value.findIndex((i) => i.id === income.id)
        if (index > -1) {
          oneTimeIncomes.value[index] = updatedIncome
        }
      }
    }
  } finally {
    showOneTimeIncomeDialog.value = false
    editingOneTimeIncome.value = undefined
  }
}

const saveRecurringIncome = async (income: RecurringIncomeDTO) => {
  try {
    if (income.id === EMPTY_GUID) {
      // Adding new income
      const savedIncome = await incomeApi.addRecurringIncome(income)
      if (recurringIncomes.value) {
        recurringIncomes.value.push(savedIncome)
      }
    } else {
      // Editing existing income
      var updatedIncome = await incomeApi.updateRecurringIncome(income)

      // For now, just update local state
      if (recurringIncomes.value) {
        const index = recurringIncomes.value.findIndex((i) => i.id === income.id)
        if (index > -1) {
          recurringIncomes.value[index] = updatedIncome
        }
      }
    }
  } finally {
    showRecurringIncomeDialog.value = false
    editingRecurringIncome.value = undefined
  }
}
</script>

<style scoped>
/* TailwindCSS utilities now used throughout. No component-scoped CSS needed. */
</style>
