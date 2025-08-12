<template>
  <div class="household-setup-wizard pt-6 sm:pt-12 max-w-5xl mx-auto px-4 sm:px-6">
    <div class="mb-4 sm:mb-6">
      <h1 class="text-2xl sm:text-3xl font-bold text-gray-900 mb-2">Household Setup</h1>
      <p class="text-sm sm:text-base text-gray-600">
        Let's set up your household financial tracking in 4 simple steps.
      </p>
    </div>

    <Stepper v-model:value="activeStep" class="basis-50rem">
      <StepList ref="stepListRef" class="step-list-container">
        <Step value="1">Household Information</Step>
        <Step value="2">Income Sources</Step>
        <Step value="3">Expense Categories</Step>
        <Step value="4">Invite Members</Step>
      </StepList>

      <StepPanels>
        <StepPanel value="1">
          <div class="flex flex-col">
            <div class="border-2 border-dashed border-gray-300 rounded-lg p-4 sm:p-8 text-center">
              <h2 class="text-lg sm:text-xl font-semibold text-gray-800 mb-2">
                Let's give your household a name
              </h2>
              <p class="text-sm sm:text-base text-gray-600 mb-6 sm:mb-8">
                This will be the main identifier for your financial plan. You can use your family
                name (e.g., "The Miller Household") or your address ("123 Maple St").
              </p>
              <div class="flex justify-center">
                <FloatLabel variant="on">
                  <InputText id="householdName" v-model="household.name" class="w-full max-w-sm" />
                  <label for="householdName">Household Name</label>
                </FloatLabel>
              </div>
            </div>
          </div>
          <div class="flex pt-4 sm:pt-6 justify-end">
            <Button
              label="Next"
              icon="pi pi-arrow-right"
              iconPos="right"
              @click="nextStep"
              :disabled="!household.name"
              class="w-full sm:w-auto"
            />
          </div>
        </StepPanel>

        <StepPanel value="2">
          <div class="flex flex-col space-y-4 sm:space-y-6">
            <div class="border-2 border-dashed border-gray-300 rounded-lg p-4 sm:p-6">
              <h2 class="text-lg sm:text-xl font-semibold text-gray-800 mb-3 sm:mb-4">
                <i class="pi pi-dollar mr-2"></i>
                Recurring Income Sources
              </h2>
              <p class="text-sm sm:text-base text-gray-600 mb-4 sm:mb-6">
                Add your regular income sources like salary, freelance work, investments, etc.
              </p>

              <!-- Income Form -->
              <div class="bg-gray-50 p-3 sm:p-4 rounded-lg mb-4 sm:mb-6">
                <h3 class="text-base sm:text-lg font-medium text-gray-700 mb-3 sm:mb-4">
                  {{ editingIncome ? 'Edit Income Source' : 'Add New Income Source' }}
                </h3>
                <div class="grid grid-cols-1 gap-3 sm:gap-4">
                  <div>
                    <FloatLabel variant="on">
                      <InputText
                        id="description"
                        v-model="currentIncome.description"
                        class="w-full"
                        placeholder="e.g., Main Salary, Freelance Work"
                      />
                      <label for="description">Description</label>
                    </FloatLabel>
                  </div>
                  <div>
                    <FloatLabel variant="on">
                      <InputNumber
                        id="amount"
                        v-model="currentIncome.amount"
                        class="w-full"
                        mode="currency"
                        currency="USD"
                        locale="en-US"
                        :min="0"
                      />
                      <label for="amount">Amount</label>
                    </FloatLabel>
                  </div>
                  <div>
                    <FloatLabel variant="on">
                      <DatePicker
                        id="startDate"
                        v-model="currentIncome.startDate"
                        class="w-full"
                        dateFormat="yy-mm-dd"
                      />
                      <label for="startDate">Start Date</label>
                    </FloatLabel>
                  </div>
                  <div>
                    <FloatLabel variant="on">
                      <DatePicker
                        id="endDate"
                        v-model="currentIncome.endDate"
                        class="w-full"
                        dateFormat="yy-mm-dd"
                        :showButtonBar="true"
                        @clear-click="currentIncome.endDate = null"
                      />
                      <label for="endDate">End Date (Optional)</label>
                    </FloatLabel>
                  </div>
                  <div>
                    <FloatLabel variant="on">
                      <Select
                        id="recurrence"
                        v-model="currentIncome.recurrence"
                        :options="recurrenceOptions"
                        optionLabel="label"
                        optionValue="value"
                        class="w-full"
                      />
                      <label for="recurrence">Recurrence</label>
                    </FloatLabel>
                  </div>
                </div>
                <div
                  class="flex flex-col sm:flex-row justify-end space-y-2 sm:space-y-0 sm:space-x-2 mt-4"
                >
                  <Button
                    v-if="editingIncome"
                    label="Cancel"
                    severity="secondary"
                    @click="cancelEdit"
                    class="w-full sm:w-auto order-2 sm:order-1"
                  />
                  <Button
                    :label="editingIncome ? 'Update' : 'Add Income'"
                    icon="pi pi-plus"
                    @click="saveIncome"
                    :disabled="!isIncomeValid"
                    class="w-full sm:w-auto order-1 sm:order-2"
                  />
                </div>
              </div>

              <!-- Income List -->
              <div v-if="recurringIncomes.length > 0">
                <h3 class="text-base sm:text-lg font-medium text-gray-700 mb-3 sm:mb-4">
                  Added Income Sources
                </h3>
                <div class="space-y-3">
                  <div
                    v-for="(income, index) in recurringIncomes"
                    :key="index"
                    class="flex flex-col sm:flex-row sm:items-center justify-between p-3 sm:p-4 bg-white border border-gray-200 rounded-lg space-y-3 sm:space-y-0"
                  >
                    <div class="flex-1">
                      <div
                        class="flex flex-col sm:flex-row sm:items-center space-y-1 sm:space-y-0 sm:space-x-4"
                      >
                        <div class="flex-1">
                          <p class="font-medium text-gray-900 text-sm sm:text-base">
                            {{ income.description }}
                          </p>
                          <p class="text-xs sm:text-sm text-gray-500">
                            ${{ income.amount.toLocaleString() }} •
                            {{ getRecurrenceLabel(income.recurrence) }}
                          </p>
                          <p class="text-xs text-gray-400">
                            {{ formatDate(income.startDate) }}
                            {{ income.endDate ? ` - ${formatDate(income.endDate)}` : ' (Ongoing)' }}
                          </p>
                        </div>
                      </div>
                    </div>
                    <div class="flex space-x-2 self-end sm:self-auto">
                      <Button
                        icon="pi pi-pencil"
                        severity="secondary"
                        size="small"
                        @click="editIncome(index)"
                        outlined
                      />
                      <Button
                        icon="pi pi-trash"
                        severity="danger"
                        size="small"
                        @click="deleteIncome(index)"
                        outlined
                      />
                    </div>
                  </div>
                </div>
              </div>

              <div v-else class="text-center py-6 sm:py-8">
                <i class="pi pi-info-circle text-xl sm:text-2xl text-gray-400 mb-2"></i>
                <p class="text-sm sm:text-base text-gray-500">No income sources added yet</p>
                <p class="text-xs sm:text-sm text-gray-400">Add your first income source above</p>
              </div>
            </div>
          </div>
          <div
            class="flex flex-col sm:flex-row pt-4 sm:pt-6 justify-between space-y-2 sm:space-y-0"
          >
            <Button
              label="Back"
              icon="pi pi-arrow-left"
              severity="secondary"
              @click="prevStep"
              class="w-full sm:w-auto order-2 sm:order-1"
            />
            <Button
              :label="recurringIncomes.length > 0 ? 'Next' : 'Skip'"
              icon="pi pi-arrow-right"
              iconPos="right"
              @click="nextStep"
              class="w-full sm:w-auto order-1 sm:order-2"
            />
          </div>
        </StepPanel>

        <StepPanel value="3">
          <div class="flex flex-col space-y-4 sm:space-y-6">
            <div class="border-2 border-dashed border-gray-300 rounded-lg p-4 sm:p-6">
              <h2 class="text-lg sm:text-xl font-semibold text-gray-800 mb-3 sm:mb-4">
                <i class="pi pi-tags mr-2"></i>
                Expense Buckets
              </h2>
              <p class="text-sm sm:text-base text-gray-600 mb-4 sm:mb-6">
                Add your expense buckets like Food, Transport, Utilities, Entertainment, etc.
              </p>

              <!-- Expense Bucket Form -->
              <div class="bg-gray-50 p-3 sm:p-4 rounded-lg mb-4 sm:mb-6">
                <h3 class="text-base sm:text-lg font-medium text-gray-700 mb-3 sm:mb-4">
                  {{ editingBucket ? 'Edit Expense Bucket' : 'Add New Expense Bucket' }}
                </h3>
                <div class="grid grid-cols-1 gap-3 sm:gap-4">
                  <div>
                    <FloatLabel variant="on">
                      <InputText
                        id="bucketName"
                        v-model="currentBucket.name"
                        class="w-full"
                        placeholder="e.g., Food, Transport"
                      />
                      <label for="bucketName">Name</label>
                    </FloatLabel>
                  </div>
                  <div>
                    <FloatLabel variant="on">
                      <InputNumber
                        id="bucketAmount"
                        v-model="currentBucket.monthlyAmount"
                        class="w-full"
                        mode="currency"
                        currency="USD"
                        locale="en-US"
                        :min="0"
                      />
                      <label for="bucketAmount">Estimated Monthly Amount</label>
                    </FloatLabel>
                  </div>
                  <div>
                    <FloatLabel variant="on">
                      <InputText
                        id="bucketDescription"
                        v-model="currentBucket.description"
                        class="w-full"
                        placeholder="Optional description"
                      />
                      <label for="bucketDescription">Description (Optional)</label>
                    </FloatLabel>
                  </div>
                </div>
                <div
                  class="flex flex-col sm:flex-row justify-end space-y-2 sm:space-y-0 sm:space-x-2 mt-4"
                >
                  <Button
                    v-if="editingBucket"
                    label="Cancel"
                    severity="secondary"
                    @click="cancelBucketEdit"
                    class="w-full sm:w-auto order-2 sm:order-1"
                  />
                  <Button
                    :label="editingBucket ? 'Update' : 'Add Bucket'"
                    icon="pi pi-plus"
                    @click="saveBucket"
                    :disabled="!isBucketValid"
                    class="w-full sm:w-auto order-1 sm:order-2"
                  />
                </div>
              </div>

              <!-- Expense Buckets List -->
              <div v-if="expenseBuckets.length > 0">
                <h3 class="text-base sm:text-lg font-medium text-gray-700 mb-3 sm:mb-4">
                  Added Expense Buckets
                </h3>
                <div class="space-y-3">
                  <div
                    v-for="(bucket, index) in expenseBuckets"
                    :key="index"
                    class="flex flex-col sm:flex-row sm:items-center justify-between p-3 sm:p-4 bg-white border border-gray-200 rounded-lg space-y-3 sm:space-y-0"
                  >
                    <div class="flex-1">
                      <div
                        class="flex flex-col sm:flex-row sm:items-center space-y-1 sm:space-y-0 sm:space-x-4"
                      >
                        <div class="flex-1">
                          <p class="font-medium text-gray-900 text-sm sm:text-base">
                            {{ bucket.name }}
                          </p>
                          <p class="text-xs sm:text-sm text-gray-500">
                            ${{ bucket.monthlyAmount.toLocaleString() }}
                          </p>
                          <p v-if="bucket.description" class="text-xs text-gray-400">
                            {{ bucket.description }}
                          </p>
                        </div>
                      </div>
                    </div>
                    <div class="flex space-x-2 self-end sm:self-auto">
                      <Button
                        icon="pi pi-pencil"
                        severity="secondary"
                        size="small"
                        @click="editBucket(index)"
                        outlined
                      />
                      <Button
                        icon="pi pi-trash"
                        severity="danger"
                        size="small"
                        @click="deleteBucket(index)"
                        outlined
                      />
                    </div>
                  </div>
                </div>
              </div>

              <div v-else class="text-center py-6 sm:py-8">
                <i class="pi pi-info-circle text-xl sm:text-2xl text-gray-400 mb-2"></i>
                <p class="text-sm sm:text-base text-gray-500">No expense buckets added yet</p>
                <p class="text-xs sm:text-sm text-gray-400">Add your first expense bucket above</p>
              </div>
            </div>
          </div>
          <div
            class="flex flex-col sm:flex-row pt-4 sm:pt-6 justify-between space-y-2 sm:space-y-0"
          >
            <Button
              label="Back"
              icon="pi pi-arrow-left"
              severity="secondary"
              @click="prevStep"
              class="w-full sm:w-auto order-2 sm:order-1"
            />
            <Button
              label="Next"
              icon="pi pi-arrow-right"
              iconPos="right"
              @click="nextStep"
              class="w-full sm:w-auto order-1 sm:order-2"
            />
          </div>
        </StepPanel>

        <StepPanel value="4">
          <div class="flex flex-col space-y-4 sm:space-y-6">
            <div class="border-2 border-dashed border-gray-300 rounded-lg p-4 sm:p-6">
              <h2 class="text-lg sm:text-xl font-semibold text-gray-800 mb-3 sm:mb-4">
                <i class="pi pi-users mr-2"></i>
                Invite Household Members
              </h2>
              <p class="text-sm sm:text-base text-gray-600 mb-4 sm:mb-6">
                Invite family members or roommates to join your household and collaborate on
                financial planning.
              </p>

              <!-- Invite Form -->
              <div class="bg-gray-50 p-3 sm:p-4 rounded-lg mb-4 sm:mb-6">
                <h3 class="text-base sm:text-lg font-medium text-gray-700 mb-3 sm:mb-4">
                  Add New Invite
                </h3>
                <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
                  <div class="flex-1">
                    <FloatLabel variant="on">
                      <InputText
                        id="inviteEmail"
                        v-model="newInviteEmail"
                        class="w-full"
                        placeholder="Enter email address"
                        type="email"
                      />
                      <label for="inviteEmail">Email Address</label>
                    </FloatLabel>
                  </div>
                  <div class="flex-shrink-0">
                    <Button
                      label="Add Invite"
                      icon="pi pi-plus"
                      @click="addInvite"
                      :disabled="!isInviteValid"
                      class="w-full sm:w-auto"
                    />
                  </div>
                </div>
              </div>

              <!-- Invites List -->
              <div v-if="invites.length > 0">
                <h3 class="text-base sm:text-lg font-medium text-gray-700 mb-3 sm:mb-4">
                  Pending Invites
                </h3>
                <div class="space-y-3">
                  <div
                    v-for="(invite, index) in invites"
                    :key="index"
                    class="flex flex-col sm:flex-row sm:items-center justify-between p-3 sm:p-4 bg-white border border-gray-200 rounded-lg space-y-3 sm:space-y-0"
                  >
                    <div class="flex-1">
                      <div
                        class="flex flex-col sm:flex-row sm:items-center space-y-1 sm:space-y-0 sm:space-x-4"
                      >
                        <div class="flex-1">
                          <p class="font-medium text-gray-900 text-sm sm:text-base">
                            {{ invite.email }}
                          </p>
                          <p class="text-xs sm:text-sm text-gray-500">Pending invitation</p>
                        </div>
                      </div>
                    </div>
                    <div class="flex space-x-2 self-end sm:self-auto">
                      <Button
                        icon="pi pi-trash"
                        severity="danger"
                        size="small"
                        @click="removeInvite(index)"
                        outlined
                      />
                    </div>
                  </div>
                </div>
              </div>

              <div v-else class="text-center py-6 sm:py-8">
                <i class="pi pi-info-circle text-xl sm:text-2xl text-gray-400 mb-2"></i>
                <p class="text-sm sm:text-base text-gray-500">No invites added yet</p>
                <p class="text-xs sm:text-sm text-gray-400">
                  Add email addresses above to invite members
                </p>
              </div>
            </div>
          </div>
          <div
            class="flex flex-col sm:flex-row pt-4 sm:pt-6 justify-between space-y-2 sm:space-y-0"
          >
            <Button
              label="Back"
              icon="pi pi-arrow-left"
              severity="secondary"
              @click="prevStep"
              class="w-full sm:w-auto order-2 sm:order-1"
            />
            <Button
              label="Complete Setup"
              icon="pi pi-check"
              iconPos="right"
              severity="success"
              @click="completeSetup"
              class="w-full sm:w-auto order-1 sm:order-2"
            />
          </div>
        </StepPanel>
      </StepPanels>
    </Stepper>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick } from 'vue'
import Stepper from 'primevue/stepper'
import StepList from 'primevue/steplist'
import Step from 'primevue/step'
import StepPanels from 'primevue/steppanels'
import StepPanel from 'primevue/steppanel'
import Button from 'primevue/button'
import { format } from 'date-fns'
import { FloatLabel, InputText, InputNumber, DatePicker, Select } from 'primevue'
import {
  EMPTY_GUID,
  type HouseholdDTO,
  type SetupDTO,
  type RecurringIncomeDTO,
  type ExpenseBucketDTO,
  type HouseholdInviteDTO,
} from '@/api/models'
import { RecurrenceType } from '@/models/recurrenceType'
import { useHouseholdApi } from '@/api/householdApi'
import { useDeviceType } from '@/composables/useDeviceType'

const houstholdApi = useHouseholdApi()
const { isMobile } = useDeviceType()

const emit = defineEmits<{
  (e: 'completed'): void
}>()

const activeStep = ref('1')
const stepListRef = ref<any>(null)
const household = ref<HouseholdDTO>({
  id: EMPTY_GUID,
  name: '',
})

// Recurring Income Management
const recurringIncomes = ref<RecurringIncomeDTO[]>([])
const editingIncome = ref(false)
const editingIndex = ref(-1)

// Expense Buckets Management
const expenseBuckets = ref<ExpenseBucketDTO[]>([])
const invites = ref<HouseholdInviteDTO[]>([])
const editingBucket = ref(false)
const editingBucketIndex = ref(-1)

const currentBucket = ref<ExpenseBucketDTO>({
  id: EMPTY_GUID,
  householdId: EMPTY_GUID,
  name: '',
  monthlyAmount: 0,
  description: '',
})

const currentIncome = ref({
  id: EMPTY_GUID,
  householdId: EMPTY_GUID,
  amount: 0,
  startDate: null as Date | null,
  endDate: null as Date | null,
  recurrence: RecurrenceType.Monthly,
  description: '',
})

const newInviteEmail = ref('')

const recurrenceOptions = [
  { label: 'Daily', value: RecurrenceType.Daily },
  { label: 'Weekly', value: RecurrenceType.Weekly },
  { label: 'Bi-Weekly', value: RecurrenceType.BiWeekly },
  { label: 'Monthly', value: RecurrenceType.Monthly },
  { label: 'Quarterly', value: RecurrenceType.Quarterly },
  { label: 'Yearly', value: RecurrenceType.Yearly },
]

const isIncomeValid = computed(() => {
  return (
    currentIncome.value.description &&
    currentIncome.value.amount > 0 &&
    currentIncome.value.startDate &&
    currentIncome.value.recurrence !== undefined
  )
})

const isBucketValid = computed(() => {
  const name = currentBucket.value.name?.trim()
  const amountOk = currentBucket.value.monthlyAmount > 0
  const unique =
    name &&
    !expenseBuckets.value.some(
      (b, idx) =>
        b.name.trim().toLowerCase() === name.toLowerCase() &&
        (!editingBucket.value || idx !== editingBucketIndex.value),
    )
  return !!name && amountOk && !!unique
})

const isInviteValid = computed(() => {
  return newInviteEmail.value && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(newInviteEmail.value)
})

// Helper functions
const getRecurrenceLabel = (recurrence: RecurrenceType): string => {
  const option = recurrenceOptions.find((opt) => opt.value === recurrence)
  return option ? option.label : 'Unknown'
}

const formatDate = (dateString: string): string => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return date.toLocaleDateString()
}

// Smooth scroll function for mobile stepper
const scrollToActiveStep = async () => {
  if (!isMobile.value) return

  await nextTick()

  // Find the step list container element
  const stepListElement = document.querySelector('.step-list-container') as HTMLElement
  if (!stepListElement) return

  // Find the active step element - try multiple selectors
  let activeStepElement = stepListElement.querySelector('[aria-selected="true"]') as HTMLElement

  // Alternative selector if the first one doesn't work
  if (!activeStepElement) {
    activeStepElement = stepListElement.querySelector(
      '.p-stepper-step.p-stepper-step-active',
    ) as HTMLElement
  }

  // Another fallback selector
  if (!activeStepElement) {
    const stepIndex = parseInt(activeStep.value) - 1
    const allSteps = stepListElement.querySelectorAll('[data-pc-name="step"]')
    activeStepElement = allSteps[stepIndex] as HTMLElement
  }

  if (activeStepElement) {
    // Calculate the scroll position to center the active step
    const stepListWidth = stepListElement.clientWidth
    const stepLeft = activeStepElement.offsetLeft
    const stepWidth = activeStepElement.offsetWidth

    // Calculate the position to center the step
    const targetScrollLeft = stepLeft - stepListWidth / 2 + stepWidth / 2

    // Scroll smoothly to the target position
    stepListElement.scrollTo({
      left: Math.max(0, targetScrollLeft),
      behavior: 'smooth',
    })
  }
}

// Watch for active step changes and trigger smooth scroll on mobile
watch(activeStep, () => {
  if (isMobile.value) {
    scrollToActiveStep()
  }
})

const resetCurrentIncome = () => {
  currentIncome.value = {
    id: EMPTY_GUID,
    householdId: EMPTY_GUID,
    amount: 0,
    startDate: null,
    endDate: null,
    recurrence: RecurrenceType.Monthly,
    description: '',
  }
}

const resetCurrentBucket = () => {
  currentBucket.value = {
    id: EMPTY_GUID,
    householdId: EMPTY_GUID,
    name: '',
    monthlyAmount: 0,
    description: '',
  }
}

// Income management functions
const saveIncome = () => {
  if (!isIncomeValid.value) return

  const incomeToSave: RecurringIncomeDTO = {
    id: editingIncome.value ? currentIncome.value.id : EMPTY_GUID,
    householdId: household.value.id,
    amount: currentIncome.value.amount,
    startDate: currentIncome.value.startDate
      ? format(currentIncome.value.startDate, 'yyyy-MM-dd')
      : '',
    endDate: currentIncome.value.endDate ? format(currentIncome.value.endDate, 'yyyy-MM-dd') : null,
    recurrence: currentIncome.value.recurrence,
    description: currentIncome.value.description || '',
  }

  if (editingIncome.value) {
    recurringIncomes.value[editingIndex.value] = incomeToSave
    editingIncome.value = false
    editingIndex.value = -1
  } else {
    recurringIncomes.value.push(incomeToSave)
  }

  resetCurrentIncome()
}

// Bucket management
const saveBucket = () => {
  if (!isBucketValid.value) return

  const toSave: ExpenseBucketDTO = {
    id: editingBucket.value ? currentBucket.value.id : EMPTY_GUID,
    householdId: household.value.id,
    name: (currentBucket.value.name || '').trim(),
    monthlyAmount: currentBucket.value.monthlyAmount,
    description: currentBucket.value.description || '',
  }

  if (editingBucket.value) {
    expenseBuckets.value[editingBucketIndex.value] = toSave
    editingBucket.value = false
    editingBucketIndex.value = -1
  } else {
    expenseBuckets.value.push(toSave)
  }

  resetCurrentBucket()
}

const editIncome = (index: number) => {
  const income = recurringIncomes.value[index]
  currentIncome.value = {
    id: income.id,
    householdId: income.householdId,
    amount: income.amount,
    startDate: income.startDate ? new Date(income.startDate) : null,
    endDate: income.endDate ? new Date(income.endDate) : null,
    recurrence: income.recurrence,
    description: income.description || '',
  }
  editingIncome.value = true
  editingIndex.value = index
}

const editBucket = (index: number) => {
  const bucket = expenseBuckets.value[index]
  currentBucket.value = {
    id: bucket.id,
    householdId: bucket.householdId,
    name: bucket.name,
    monthlyAmount: bucket.monthlyAmount,
    description: bucket.description || '',
  }
  editingBucket.value = true
  editingBucketIndex.value = index
}

const cancelEdit = () => {
  editingIncome.value = false
  editingIndex.value = -1
  resetCurrentIncome()
}

const cancelBucketEdit = () => {
  editingBucket.value = false
  editingBucketIndex.value = -1
  resetCurrentBucket()
}

const deleteIncome = (index: number) => {
  recurringIncomes.value.splice(index, 1)
}

const deleteBucket = (index: number) => {
  expenseBuckets.value.splice(index, 1)
}

const addInvite = () => {
  if (!isInviteValid.value) return

  const inviteToAdd: HouseholdInviteDTO = {
    householdId: EMPTY_GUID,
    email: newInviteEmail.value,
  }
  invites.value.push(inviteToAdd)
  newInviteEmail.value = ''
}

const removeInvite = (index: number) => {
  invites.value.splice(index, 1)
}

// Navigation functions
const nextStep = () => {
  const currentStep = parseInt(activeStep.value)
  if (currentStep < 4) {
    activeStep.value = (currentStep + 1).toString()
  }
}

const prevStep = () => {
  const currentStep = parseInt(activeStep.value)
  if (currentStep > 1) {
    activeStep.value = (currentStep - 1).toString()
  }
}

const completeSetup = async () => {
  const setupDto: SetupDTO = {
    household: household.value,
    recurringIncomes: recurringIncomes.value,
    expenseBuckets: expenseBuckets.value,
    invites: invites.value,
  }
  await houstholdApi.setupHousehold(setupDto)

  emit('completed')
}
</script>

<style scoped>
/* Custom styling for the stepper content areas */

/* Enable horizontal scrolling on mobile for step list */
@media (max-width: 768px) {
  :deep(.step-list-container) {
    overflow-x: auto;
    scrollbar-width: none; /* Firefox */
    -ms-overflow-style: none; /* IE and Edge */
  }

  /* Hide scrollbar for Chrome, Safari and Opera */
  :deep(.step-list-container::-webkit-scrollbar) {
    display: none;
  }

  /* Ensure steps are laid out horizontally with proper spacing */
  :deep(.p-stepper-list) {
    display: flex;
    flex-wrap: nowrap;
    min-width: max-content;
  }

  /* Make individual steps more compact on mobile */
  :deep(.p-stepper-step) {
    flex-shrink: 0;
    min-width: fit-content;
  }
}
</style>
