<template>
  <main class="p-4">
    <!-- Loading state -->
    <div v-if="loading" class="flex flex-col items-center justify-center min-h-[60vh] gap-4">
      <ProgressSpinner />
      <p class="text-gray-500 m-0">Loading your households...</p>
    </div>

    <!-- Setup wizard -->
    <div v-else-if="!household">
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
          {{ household.name }}
        </h1>
        <Button class="whitespace-nowrap" @click="onAddExpenseClick">Add expense</Button>
      </header>

      <!-- Widgets -->
      <section class="grid grid-cols-1 gap-4 mt-2 sm:grid-cols-2 lg:grid-cols-4">
        <DashboardStatWidget title="Monthly Income" :value="'$2,500'" size="lg" />
        <DashboardStatWidget title="Monthly Budget" :value="monthlyBudget" size="lg" />
        <DashboardStatWidget title="Budget Utilisation" :value="budgetUtilisiation" size="lg" />
        <DashboardStatWidget title="Current Cashflow" :value="'$8,900'" size="lg" />
      </section>

      <ExpenseDialog
        v-if="showExpenseDialog"
        :household-id="household.id"
        :expense-buckets="expenseBuckets"
        @closed="showExpenseDialog = false"
        @save="saveExpense"
      />
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useHouseholdApi } from '@/api/householdApi'
import type {
  OneTimeIncomeDTO,
  RecurringIncomeDTO,
  ExpenseBucketDTO,
  ExpenseDTO,
  HouseholdDTO,
} from '@/api/models'
import HouseholdSetupWizard from '@/components/HouseholdSetupWizard.vue'
import ProgressSpinner from 'primevue/progressspinner'
import DashboardStatWidget from '@/components/common/DashboardStatWidget.vue'
import { Button } from 'primevue'
import { useDeviceType } from '@/composables/useDeviceType'
import { useExpenseApi } from '@/api/expenseApi'
import ExpenseDialog from '@/components/expense/ExpenseDialog.vue'
import { useIncomeApi } from '@/api/incomeApi'

const household = ref<HouseholdDTO>()
const expenseBuckets = ref<ExpenseBucketDTO[]>([])
const currentMonthExpenses = ref<ExpenseDTO[] | null>(null)
const oneTimeIncomes = ref<OneTimeIncomeDTO[] | null>(null)
const recurringIncomes = ref<RecurringIncomeDTO[] | null>(null)
const loading = ref(true)
const showExpenseDialog = ref(false)

const { getHouseholds } = useHouseholdApi()
const expenseApi = useExpenseApi()
const incomeApi = useIncomeApi()
const { isMobile } = useDeviceType()

onMounted(() => {
  loadHouseholds()
})

const monthlyBudget = computed(() => {
  if (!expenseBuckets.value) {
    return null
  }
  if (expenseBuckets.value.length == 0) {
    return null
  }
  const result = expenseBuckets.value.reduce((a, b) => a + b.monthlyAmount, 0)
  return `$${result}`
})

const budgetUtilisiation = computed(() => {
  if (
    !expenseBuckets.value ||
    expenseBuckets.value.length == 0 ||
    currentMonthExpenses.value == null
  ) {
    return null
  }
  const bucketsSum = expenseBuckets.value.reduce((a, b) => a + b.monthlyAmount, 0)
  const expensesUsed = currentMonthExpenses.value.reduce((a, b) => a + b.amount, 0)
  const utilisation = (expensesUsed * 100) / bucketsSum
  return `%${utilisation.toFixed(2)}`
})

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
  const loadBucketsPromise = loadBuckets()
  const loadExpensesPromise = loadExpenses()
  const loadIncomesPromise = loadIncomes()
  await Promise.all([loadBucketsPromise, loadExpensesPromise, loadIncomesPromise])
}

const loadIncomes = async () => {
  if (!household.value) {
    return
  }
  const incomes = await incomeApi.getIncomesForMonth(new Date())
  oneTimeIncomes.value = incomes.oneTimeIncomes
  recurringIncomes.value = incomes.recurringIncomes
}

const loadBuckets = async () => {
  if (!household.value) {
    return
  }
  const buckets = await expenseApi.getBucketsForHousehold(household.value.id)
  expenseBuckets.value = buckets
}

const loadExpenses = async () => {
  if (!household.value) {
    return
  }
  const expenses = await expenseApi.getExpensesForMonth(household.value.id, new Date())
  currentMonthExpenses.value = expenses
}

const onAddExpenseClick = () => {
  showExpenseDialog.value = true
}

const saveExpense = async (expense: ExpenseDTO) => {
  try {
    const savedExpense = await expenseApi.createExpense(expense)
    console.log(savedExpense)
  } finally {
    showExpenseDialog.value = false
  }
}
</script>

<style scoped>
/* TailwindCSS utilities now used throughout. No component-scoped CSS needed. */
</style>
