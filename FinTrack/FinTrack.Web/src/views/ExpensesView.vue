<template>
  <main class="p-1 md:p-4">
    <!-- Loading state -->
    <div v-if="loading" class="flex flex-col items-center justify-center min-h-[60vh] gap-4">
      <ProgressSpinner />
      <p class="text-gray-500 m-0">Loading your expenses...</p>
    </div>

    <!-- Expense management -->
    <div v-else class="px-1 md:px-8 py-6">
      <!-- Header -->
      <header class="flex flex-col sm:flex-row sm:justify-between items-center gap-2 mb-6">
        <h1 class="m-0 text-slate-800 text-3xl font-bold tracking-[0.2px]">Expense Management</h1>
        <div class="flex gap-2">
          <Button class="whitespace-nowrap" @click="onAddExpenseClick">Add Expense</Button>
        </div>
      </header>

      <!-- Month Selection -->
      <section class="mb-6">
        <MonthPicker v-model="selectedMonth" :loading="loading" @change="onMonthChange" />
      </section>

      <!-- Expenses Section -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
        <h2 class="text-xl font-semibold text-slate-800 mb-4">Expenses</h2>
        <Skeleton v-if="expenses == null" height="100px" width="100%" />
        <div v-else>
          <div v-if="expenses.length === 0" class="bg-gray-50 rounded-lg p-8 text-center">
            <div class="text-gray-400 mb-2">
              <svg class="w-12 h-12 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z"
                ></path>
              </svg>
            </div>
            <p class="text-gray-500 font-medium">No expenses</p>
            <p class="text-sm text-gray-400 mt-1">Add expenses to see them here</p>
          </div>
          <div v-else>
            <ExpenseCard
              v-for="expense in expenses"
              :key="expense.id"
              :expense="expense"
              :expense-buckets="expenseBuckets"
              :show-date="true"
              :show-action-buttons="true"
              @edit="editExpense"
              @delete="deleteExpense"
            />
          </div>
        </div>
      </div>

      <!-- Summary Statistics -->
      <section class="mt-8" v-if="expenses != null">
        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <h2 class="text-xl font-semibold text-slate-800 mb-4">Expense Summary</h2>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div
              class="bg-gradient-to-r from-red-50 to-pink-50 rounded-lg p-4 border border-red-200"
            >
              <h3 class="text-sm font-medium text-red-800 mb-1">Total Expenses</h3>
              <p class="text-2xl font-bold text-red-600">${{ totalExpenses.toFixed(2) }}</p>
            </div>
            <div
              class="bg-gradient-to-r from-orange-50 to-amber-50 rounded-lg p-4 border border-orange-200"
            >
              <h3 class="text-sm font-medium text-orange-800 mb-1">Number of Expenses</h3>
              <p class="text-2xl font-bold text-orange-600">{{ expenses?.length || 0 }}</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Expense Dialog -->
      <ExpenseDialog
        v-if="showExpenseDialog"
        :household-id="householdStore.currentHousehold!.id"
        :expense-buckets="expenseBuckets!"
        :expense="editingExpense"
        @save="saveExpense"
        @closed="
          () => {
            showExpenseDialog = false
            editingExpense = undefined
          }
        "
      />
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { EMPTY_GUID, type ExpenseBucketDTO, type ExpenseDTO } from '@/api/models'
import { useExpenseApi } from '@/api/expenseApi'
import ProgressSpinner from 'primevue/progressspinner'
import { Button, Skeleton } from 'primevue'
import { useConfirm } from 'primevue/useconfirm'
import ExpenseCard from '@/components/expense/ExpenseCard.vue'
import ExpenseDialog from '@/components/expense/ExpenseDialog.vue'
import MonthPicker from '@/components/common/MonthPicker.vue'
import { useHouseholdStore } from '@/stores/household'

const expenses = ref<ExpenseDTO[] | null>(null)
const loading = ref(true)
const showExpenseDialog = ref(false)
const editingExpense = ref<ExpenseDTO | undefined>()
const selectedMonth = ref<Date>(new Date())
const householdStore = useHouseholdStore()
const expenseBuckets = ref<ExpenseBucketDTO[] | null>(null)

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

const totalExpenses = computed(() => {
  if (!expenses.value) return 0
  return expenses.value.reduce((total, expense) => total + expense.amount, 0)
})

const loadData = async () => {
  await Promise.all([loadExpenses(), loadExpenseBuckets()])
}

const loadExpenseBuckets = async () => {
  if (!householdStore.currentHousehold) {
    return
  }
  expenseBuckets.value = await expenseApi.getBucketsForHousehold(householdStore.currentHousehold.id)
}

const loadExpenses = async (month?: Date) => {
  if (!householdStore.currentHousehold) {
    return
  }
  try {
    loading.value = true
    const targetMonth = month || selectedMonth.value
    const expensesData = await expenseApi.getExpensesForMonth(
      householdStore.currentHousehold.id,
      targetMonth,
    )
    expenses.value = expensesData
  } catch (error) {
    console.error('Failed to load expenses:', error)
  } finally {
    loading.value = false
  }
}

const onMonthChange = (month: Date) => {
  selectedMonth.value = month
  loadExpenses(month)
}

const onAddExpenseClick = () => {
  editingExpense.value = undefined
  showExpenseDialog.value = true
}

const editExpense = (expense: ExpenseDTO) => {
  editingExpense.value = expense
  showExpenseDialog.value = true
}

const deleteExpense = async (expense: ExpenseDTO) => {
  confirm.require({
    message: `Are you sure you want to delete "${expense.description || 'this expense'}"? This action cannot be undone.`,
    header: 'Delete Expense',
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
        await expenseApi.deleteExpense(expense.id)

        if (expenses.value) {
          const index = expenses.value.findIndex((e) => e.id === expense.id)
          if (index > -1) {
            expenses.value.splice(index, 1)
          }
        }
      } catch (error) {
        console.error('Failed to delete expense:', error)
      }
    },
  })
}

const saveExpense = async (expense: ExpenseDTO) => {
  if (!householdStore.currentHousehold) {
    return
  }
  try {
    if (expense.id === EMPTY_GUID) {
      // Adding new expense
      expense.householdId = householdStore.currentHousehold.id
      const savedExpense = await expenseApi.createExpense(expense)
      if (expenses.value) {
        expenses.value.push(savedExpense)
      }
    } else {
      // Editing existing expense
      const updatedExpense = await expenseApi.updateExpense(expense)

      // Update local state
      if (expenses.value) {
        const index = expenses.value.findIndex((e) => e.id === expense.id)
        if (index > -1) {
          expenses.value[index] = updatedExpense
        }
      }
    }
  } finally {
    showExpenseDialog.value = false
    editingExpense.value = undefined
  }
}
</script>

<style scoped>
/* TailwindCSS utilities now used throughout. No component-scoped CSS needed. */
</style>
