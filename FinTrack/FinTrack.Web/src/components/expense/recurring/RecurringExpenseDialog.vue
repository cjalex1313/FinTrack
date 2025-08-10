<template>
  <Dialog
    :visible="true"
    :modal="true"
    :draggable="false"
    :maximized="isMobile"
    :class="{ 'p-dialog-maximized': isMobile }"
    :header="isEditMode ? 'Edit Recurring Expense' : 'Add Recurring Expense'"
    @update:visible="handleVisibilityChange"
  >
    <div class="w-full md:w-96">
      <div class="space-y-4">
        <!-- Amount Field -->
        <div class="field">
          <label for="amount" class="block text-sm font-medium mb-2">Amount *</label>
          <InputNumber
            id="amount"
            v-model="formData.amount"
            :min="0"
            :max-fraction-digits="2"
            mode="currency"
            currency="USD"
            locale="en-US"
            class="w-full"
            :class="{ 'p-invalid': errors.amount }"
            placeholder="Enter amount"
          />
          <small v-if="errors.amount" class="p-error text-red-500">{{ errors.amount }}</small>
        </div>

        <!-- Next Date Field -->
        <div class="field">
          <label for="nextDate" class="block text-sm font-medium mb-2">Next Date *</label>
          <DatePicker
            id="nextDate"
            v-model="formData.nextDate"
            date-format="mm/dd/yy"
            class="w-full"
            :min-date="minDateValue"
            :class="{ 'p-invalid': errors.nextDate }"
            placeholder="Select next occurrence date"
          />
          <small v-if="errors.nextDate" class="p-error text-red-500">{{ errors.nextDate }}</small>
        </div>

        <!-- Recurrence Type Field -->
        <div class="field">
          <label for="recurrence" class="block text-sm font-medium mb-2">Recurrence *</label>
          <Select
            id="recurrence"
            v-model="formData.recurrence"
            :options="recurrenceOptions"
            option-label="label"
            option-value="value"
            placeholder="Select recurrence type"
            class="w-full"
            :class="{ 'p-invalid': errors.recurrence }"
          />
          <small v-if="errors.recurrence" class="p-error text-red-500">{{
            errors.recurrence
          }}</small>
        </div>

        <!-- Expense Bucket Field -->
        <div class="field">
          <label for="expenseBucket" class="block text-sm font-medium mb-2">Expense Bucket</label>
          <Select
            id="expenseBucket"
            v-model="formData.expenseBucketId"
            :options="expenseBuckets"
            option-label="name"
            option-value="id"
            placeholder="Select expense bucket (optional)"
            class="w-full"
            show-clear
          />
        </div>

        <!-- Description Field -->
        <div class="field">
          <label for="description" class="block text-sm font-medium mb-2">Description</label>
          <Textarea
            id="description"
            v-model="formData.description"
            rows="3"
            class="w-full resize-none"
            placeholder="Enter description (optional)"
          />
        </div>

        <!-- Action Buttons -->
        <div
          class="flex flex-col md:flex-row md:justify-end items-center space-y-2 md:space-y-0 md:space-x-4 mt-6"
        >
          <Button @click="trySave" class="w-full md:w-auto" :loading="saving">
            {{ isEditMode ? 'Update' : 'Save' }}
          </Button>
          <Button @click="closeDialog" severity="secondary" class="w-full md:w-auto">
            Cancel
          </Button>
        </div>
      </div>
    </div>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useDeviceType } from '@/composables/useDeviceType'
import { Dialog, Button, InputNumber, DatePicker, Select, Textarea } from 'primevue'
import type { RecurringExpenseDTO, ExpenseBucketDTO } from '@/api/models'
import { EMPTY_GUID } from '@/api/models'
import { RecurrenceType } from '@/models/recurrenceType'
import { format } from 'date-fns'

interface Props {
  householdId: string
  expenseBuckets: ExpenseBucketDTO[]
  recurringExpense?: RecurringExpenseDTO
}

const minDateValue = new Date()

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'save', recurringExpense: RecurringExpenseDTO): void
  (e: 'closed'): void
}>()

const { isMobile } = useDeviceType()

// Form state
const saving = ref(false)
const formData = ref<{
  amount: number | null
  nextDate: Date
  recurrence: RecurrenceType | null
  expenseBucketId: string | null
  description?: string | null
}>({
  amount: null,
  nextDate: new Date(),
  recurrence: null,
  expenseBucketId: null,
  description: null,
})

// Validation errors
const errors = ref<{
  amount?: string
  nextDate?: string
  recurrence?: string
}>({})

// Recurrence options for the dropdown
const recurrenceOptions = [
  { label: 'Daily', value: RecurrenceType.Daily },
  { label: 'Weekly', value: RecurrenceType.Weekly },
  { label: 'Bi-Weekly', value: RecurrenceType.BiWeekly },
  { label: 'Monthly', value: RecurrenceType.Monthly },
  { label: 'Quarterly', value: RecurrenceType.Quarterly },
  { label: 'Yearly', value: RecurrenceType.Yearly },
]

// Computed properties
const isEditMode = computed(() => !!props.recurringExpense)

// Initialize form data
const initializeForm = () => {
  if (props.recurringExpense) {
    // Edit mode - populate with existing recurring expense data
    formData.value = {
      amount: props.recurringExpense.amount,
      nextDate: new Date(props.recurringExpense.nextDate),
      recurrence: props.recurringExpense.recurrence,
      expenseBucketId: props.recurringExpense.expenseBucketId,
      description: props.recurringExpense.description,
    }
  } else {
    // Add mode - set defaults
    formData.value = {
      amount: null,
      nextDate: new Date(),
      recurrence: null,
      expenseBucketId: null,
      description: null,
    }
  }
}

// Validation
const validateForm = (): boolean => {
  errors.value = {}
  let isValid = true

  if (!formData.value.amount || formData.value.amount <= 0) {
    errors.value.amount = 'Amount is required and must be greater than 0'
    isValid = false
  }

  if (!formData.value.nextDate) {
    errors.value.nextDate = 'Next date is required'
    isValid = false
  }

  if (formData.value.recurrence === null || formData.value.recurrence === undefined) {
    errors.value.recurrence = 'Recurrence type is required'
    isValid = false
  }

  return isValid
}

// Event handlers
const handleVisibilityChange = (visible: boolean) => {
  if (!visible) {
    emit('closed')
  }
}

const closeDialog = () => {
  emit('closed')
}

const trySave = async () => {
  if (!validateForm()) {
    return
  }

  saving.value = true

  try {
    const recurringExpenseDTO: RecurringExpenseDTO = {
      id: props.recurringExpense?.id || EMPTY_GUID,
      householdId: props.householdId,
      expenseBucketId: formData.value.expenseBucketId,
      amount: formData.value.amount!,
      nextDate: format(formData.value.nextDate, 'yyyy-MM-dd'),
      recurrence: formData.value.recurrence!,
      description: formData.value.description,
    }

    emit('save', recurringExpenseDTO)
  } finally {
    saving.value = false
  }
}

// Initialize form when component mounts
onMounted(() => {
  initializeForm()
})
</script>

<style scoped>
.field {
  margin-bottom: 1rem;
}

.p-error {
  font-size: 0.875rem;
}

.p-invalid {
  border-color: var(--red-500);
}
</style>
