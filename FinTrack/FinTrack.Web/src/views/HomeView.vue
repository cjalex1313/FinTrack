<template>
  <main class="p-4">
    <!-- Loading state -->

    <div
      v-if="householdStore.loading"
      class="flex flex-col items-center justify-center min-h-[60vh] gap-4"
    >
      <ProgressSpinner />
      <p class="text-gray-500 m-0">Loading your households...</p>
    </div>
    <!-- Setup wizard -->
    <div v-else-if="householdStore.currentHousehold == null">
      <HouseholdSetupWizard @completed="loadHouseholds" />
    </div>

    <!-- Dashboard -->
    <div v-else class="text-center px-8 py-6">
      <!-- Header with responsive alignment -->
      <header
        class="flex justify-between items-center gap-2 mb-6 sm:px-10"
        :class="{
          'w-full flex-col items-center': isMobile,
        }"
      >
        <h1 class="m-0 text-slate-800 text-3xl font-bold tracking-[0.2px]">
          {{ householdStore.currentHousehold.name }}
        </h1>
        <Button class="whitespace-nowrap" @click="onAddExpenseClick">Add expense</Button>
      </header>

      <!-- Widgets -->
      <section class="grid grid-cols-1 gap-4 mt-2 sm:grid-cols-2 lg:grid-cols-4">
        <DashboardStatWidget title="Monthly Income" :value="monthlyIncome" size="lg" type="money" />
        <DashboardStatWidget title="Monthly Budget" :value="monthlyBudget" size="lg" type="money" />
        <DashboardStatWidget
          title="Budget Utilisation"
          :value="budgetUtilisiation"
          size="lg"
          type="percentage"
        />
        <DashboardStatWidget
          title="Current Cashflow"
          :value="currentCashflow"
          size="lg"
          type="money"
        />
      </section>

      <!-- Income and Expense Buckets Section -->
      <section class="mt-8">
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Income Section -->
          <div
            class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 max-h-[600px] overflow-y-auto"
          >
            <h2 class="text-xl font-semibold text-slate-800 mb-4">Incomes</h2>
            <Skeleton v-if="oneTimeIncomes == null" height="100px" width="100%" />
            <div v-else>
              <div v-if="allIncomes.length === 0" class="bg-gray-50 rounded-lg p-8 text-center">
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
                <p class="text-gray-500 font-medium">No income records</p>
                <p class="text-sm text-gray-400 mt-1">Add income sources to see them here</p>
              </div>
              <div v-else>
                <template v-for="income in allIncomes" :key="income.id">
                  <OneTimeIncomeCard v-if="income.type === 'one-time'" :income="income" />
                  <RecurringIncomeCard v-else-if="income.type === 'recurring'" :income="income" />
                </template>
              </div>
            </div>
          </div>

          <!-- Expense Buckets Section -->
          <div
            class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 max-h-[600px] overflow-y-auto"
          >
            <h2 class="text-xl font-semibold text-slate-800 mb-4">Expense Buckets</h2>
            <Skeleton v-if="calculatedBuckets === null" height="100px" width="100%" />
            <div v-else>
              <div
                v-if="calculatedBuckets.length === 0"
                class="bg-gray-50 rounded-lg p-8 text-center"
              >
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
                      d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"
                    ></path>
                  </svg>
                </div>
                <p class="text-gray-500 font-medium">No expense buckets</p>
                <p class="text-sm text-gray-400 mt-1">Create expense buckets to see them here</p>
              </div>
              <div v-else>
                <ExpenseBucketCard
                  v-for="bucket in calculatedBuckets"
                  :key="bucket.id"
                  :bucket="bucket"
                  :utilization-percentage="bucket.utilizationPercentage"
                />
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Expense Log Section -->
      <section class="mt-8">
        <div
          class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 max-h-[600px] overflow-y-auto"
        >
          <h2 class="text-xl font-semibold text-slate-800 mb-4">Expense Log</h2>
          <Skeleton v-if="currentMonthExpenses == null" height="100px" width="100%" />
          <div v-else>
            <div
              v-if="currentMonthExpenses.length === 0"
              class="bg-gray-50 rounded-lg p-8 text-center"
            >
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
                    d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
                  ></path>
                </svg>
              </div>
              <p class="text-gray-500 font-medium">No expenses recorded</p>
              <p class="text-sm text-gray-400 mt-1">Your expense history will appear here</p>
            </div>
            <div v-else>
              <ExpenseCard
                v-for="expense in currentMonthExpenses"
                :key="expense.id"
                :expense="expense"
                :expense-buckets="expenseBuckets!"
              />
            </div>
          </div>
        </div>
      </section>

      <ExpenseDialog
        v-if="showExpenseDialog"
        :household-id="householdStore.currentHousehold.id"
        :expense-buckets="expenseBuckets!"
        @closed="showExpenseDialog = false"
        @save="saveExpense"
      />
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import type {
  OneTimeIncomeDTO,
  RecurringIncomeDTO,
  ExpenseBucketDTO,
  ExpenseDTO,
} from '@/api/models'
import HouseholdSetupWizard from '@/components/HouseholdSetupWizard.vue'
import ProgressSpinner from 'primevue/progressspinner'
import DashboardStatWidget from '@/components/common/DashboardStatWidget.vue'
import { Button, Skeleton } from 'primevue'
import { useDeviceType } from '@/composables/useDeviceType'
import { useExpenseApi } from '@/api/expenseApi'
import ExpenseDialog from '@/components/expense/ExpenseDialog.vue'
import { useIncomeApi } from '@/api/incomeApi'
import getMonthlyEquivalent from '@/helpers/getMonthlyEquivalent'
import OneTimeIncomeCard from '@/components/income/OneTimeIncomeCard.vue'
import RecurringIncomeCard from '@/components/income/RecurringIncomeCard.vue'
import ExpenseBucketCard from '@/components/expense/ExpenseBucketCard.vue'
import ExpenseCard from '@/components/expense/ExpenseCard.vue'
import { useHouseholdStore } from '@/stores/household'

const expenseBuckets = ref<ExpenseBucketDTO[] | null>(null)
const currentMonthExpenses = ref<ExpenseDTO[] | null>(null)
const oneTimeIncomes = ref<OneTimeIncomeDTO[] | null>(null)
const recurringIncomes = ref<RecurringIncomeDTO[] | null>(null)
const showExpenseDialog = ref(false)

const expenseApi = useExpenseApi()
const incomeApi = useIncomeApi()
const { isMobile } = useDeviceType()
const householdStore = useHouseholdStore()

const monthlyBudget = computed(() => {
  if (!expenseBuckets.value) {
    return null
  }
  if (expenseBuckets.value.length == 0) {
    return 0
  }
  const result = expenseBuckets.value.reduce((a, b) => a + b.monthlyAmount, 0)
  return result
})

const calculatedBuckets = computed(() => {
  if (expenseBuckets.value == null) {
    return null
  }
  const result = expenseBuckets.value.map((bucket) => {
    let utilizationPercentage: number | null = null
    if (currentMonthExpenses.value != null) {
      const expensesUsed = currentMonthExpenses.value
        .filter((e) => e.expenseBucketId === bucket.id)
        .reduce((a, b) => a + b.amount, 0)
      const utilisation = (expensesUsed * 100) / bucket.monthlyAmount
      utilizationPercentage = utilisation
    }
    return {
      ...bucket,
      utilizationPercentage,
    }
  })
  if (currentMonthExpenses.value != null) {
    const otherExpenses = currentMonthExpenses.value.filter((e) => e.expenseBucketId == null)
    const otherExpensesAmount = otherExpenses.reduce((a, b) => a + b.amount, 0)
    if (otherExpensesAmount > 0) {
      result.push({
        id: 'Other',
        householdId: '',
        name: 'Other',
        monthlyAmount: otherExpensesAmount,
        description: 'Unknown bucket',
        utilizationPercentage: null,
      })
    }
  }

  return result
})

const loadHouseholds = () => {
  householdStore.loadHouseholds()
}

const budgetUtilisiation = computed(() => {
  if (!expenseBuckets.value || currentMonthExpenses.value == null) {
    return null
  }
  if (expenseBuckets.value.length == 0) {
    return '-'
  }
  const bucketsSum = expenseBuckets.value.reduce((a, b) => a + b.monthlyAmount, 0)
  const expensesUsed = currentMonthExpenses.value.reduce((a, b) => a + b.amount, 0)
  const utilisation = (expensesUsed * 100) / bucketsSum
  return utilisation
})

const monthlyIncome = computed(() => {
  if (oneTimeIncomes.value == null || recurringIncomes.value == null) {
    return null
  }
  let oneTimeMoney = 0
  if (oneTimeIncomes.value && oneTimeIncomes.value.length > 0) {
    oneTimeMoney += oneTimeIncomes.value.reduce((a, b) => a + b.amount, 0)
  }
  let recurringMoney = 0
  if (recurringIncomes.value && recurringIncomes.value.length > 0) {
    recurringMoney += recurringIncomes.value.reduce((a, b) => {
      return a + getMonthlyEquivalent(b.recurrence, b.amount)
    }, 0)
  }
  return oneTimeMoney + recurringMoney
})

const currentCashflow = computed(() => {
  if (monthlyIncome.value == null || currentMonthExpenses.value == null) {
    return null
  }
  const totalMonthPaid = currentMonthExpenses.value.reduce((a, b) => a + b.amount, 0)
  return monthlyIncome.value - totalMonthPaid
})

const allIncomes = computed(() => {
  const incomes: any[] = []

  // Add one-time incomes
  if (oneTimeIncomes.value) {
    incomes.push(
      ...oneTimeIncomes.value.map((income) => ({
        ...income,
        type: 'one-time' as const,
      })),
    )
  }

  // Add recurring incomes
  if (recurringIncomes.value) {
    incomes.push(
      ...recurringIncomes.value.map((income) => ({
        ...income,
        type: 'recurring' as const,
      })),
    )
  }

  return incomes
})

const loadData = async () => {
  const loadBucketsPromise = loadBuckets()
  const loadExpensesPromise = loadExpenses()
  const loadIncomesPromise = loadIncomes()
  await Promise.all([loadBucketsPromise, loadExpensesPromise, loadIncomesPromise])
}

const loadIncomes = async () => {
  if (!householdStore.currentHousehold) {
    return
  }
  const incomes = await incomeApi.getIncomesForMonth(new Date(), householdStore.currentHousehold.id)
  oneTimeIncomes.value = incomes.oneTimeIncomes
  recurringIncomes.value = incomes.recurringIncomes
}

const loadBuckets = async () => {
  if (!householdStore.currentHousehold) {
    return
  }
  const buckets = await expenseApi.getBucketsForHousehold(householdStore.currentHousehold.id)
  expenseBuckets.value = buckets
}

const loadExpenses = async () => {
  if (!householdStore.currentHousehold) {
    return
  }
  const expenses = await expenseApi.getExpensesForMonth(
    householdStore.currentHousehold.id,
    new Date(),
  )
  currentMonthExpenses.value = expenses
}

const onAddExpenseClick = () => {
  showExpenseDialog.value = true
}

const saveExpense = async (expense: ExpenseDTO) => {
  try {
    const savedExpense = await expenseApi.createExpense(expense)

    const now = new Date()
    const expenseDate = new Date(savedExpense.date)
    if (
      expenseDate.getFullYear() === now.getFullYear() &&
      expenseDate.getMonth() === now.getMonth()
    ) {
      if (currentMonthExpenses.value != null) {
        currentMonthExpenses.value.push(savedExpense)
      }
    }
  } finally {
    showExpenseDialog.value = false
  }
}

watch(
  () => householdStore.currentHousehold,
  (newHousehold) => {
    if (newHousehold) {
      loadData()
    }
  },
  { immediate: true },
)
</script>

<style scoped>
/* TailwindCSS utilities now used throughout. No component-scoped CSS needed. */
</style>
