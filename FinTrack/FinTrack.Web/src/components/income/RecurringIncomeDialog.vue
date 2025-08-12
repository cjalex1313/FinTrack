<template>
  <Dialog
    :visible="true"
    :modal="true"
    :draggable="false"
    :maximized="isMobile"
    :class="{ 'p-dialog-maximized': isMobile }"
    :header="isEditMode ? 'Edit Recurring Income' : 'Add Recurring Income'"
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

        <!-- Recurrence Type Field -->
        <div class="field">
          <label for="recurrence" class="block text-sm font-medium mb-2">Frequency *</label>
          <Select
            id="recurrence"
            v-model="formData.recurrence"
            :options="recurrenceOptions"
            option-label="label"
            option-value="value"
            placeholder="Select frequency"
            class="w-full"
            :class="{ 'p-invalid': errors.recurrence }"
          />
          <small v-if="errors.recurrence" class="p-error text-red-500">{{
            errors.recurrence
          }}</small>
        </div>

        <!-- Start Date Field -->
        <div class="field">
          <label for="startDate" class="block text-sm font-medium mb-2">Start Date *</label>
          <DatePicker
            id="startDate"
            v-model="formData.startDate"
            date-format="mm/dd/yy"
            class="w-full"
            :class="{ 'p-invalid': errors.startDate }"
            placeholder="Select start date"
          />
          <small v-if="errors.startDate" class="p-error text-red-500">{{ errors.startDate }}</small>
        </div>

        <!-- End Date Field -->
        <div class="field">
          <label for="endDate" class="block text-sm font-medium mb-2">End Date</label>
          <DatePicker
            id="endDate"
            v-model="formData.endDate"
            date-format="mm/dd/yy"
            class="w-full"
            placeholder="Select end date (optional)"
            show-clear
          />
          <small class="text-gray-500 text-xs">Leave empty for ongoing income</small>
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
import { Dialog, Button, InputNumber, DatePicker, Textarea, Select } from 'primevue'
import type { RecurringIncomeDTO } from '@/api/models'
import type { RecurrenceType } from '@/models/recurrenceType'
import { EMPTY_GUID } from '@/api/models'
import { format } from 'date-fns'

interface Props {
  income?: RecurringIncomeDTO
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'save', income: RecurringIncomeDTO): void
  (e: 'closed'): void
}>()

const { isMobile } = useDeviceType()

// Recurrence options
const recurrenceOptions = [
  { label: 'Daily', value: 0 },
  { label: 'Weekly', value: 1 },
  { label: 'Bi-weekly', value: 2 },
  { label: 'Monthly', value: 3 },
  { label: 'Quarterly', value: 4 },
  { label: 'Yearly', value: 5 },
]

// Form state
const saving = ref(false)
const formData = ref<{
  amount: number | null
  recurrence: RecurrenceType | null
  startDate: Date
  endDate: Date | null
  description?: string | null
}>({
  amount: null,
  recurrence: null,
  startDate: new Date(),
  endDate: null,
  description: null,
})

// Validation errors
const errors = ref<{
  amount?: string
  recurrence?: string
  startDate?: string
}>({})

// Computed properties
const isEditMode = computed(() => !!props.income)

// Initialize form data
const initializeForm = () => {
  if (props.income) {
    // Edit mode - populate with existing income data
    formData.value = {
      amount: props.income.amount,
      recurrence: props.income.recurrence,
      startDate: new Date(props.income.startDate),
      endDate: props.income.endDate ? new Date(props.income.endDate) : null,
      description: props.income.description,
    }
  } else {
    // Add mode - set defaults
    formData.value = {
      amount: null,
      recurrence: null,
      startDate: new Date(),
      endDate: null,
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

  if (formData.value.recurrence === null || formData.value.recurrence === undefined) {
    errors.value.recurrence = 'Frequency is required'
    isValid = false
  }

  if (!formData.value.startDate) {
    errors.value.startDate = 'Start date is required'
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
    const incomeDTO: RecurringIncomeDTO = {
      id: props.income?.id || EMPTY_GUID,
      householdId: props.income?.householdId || EMPTY_GUID,
      amount: formData.value.amount!,
      recurrence: formData.value.recurrence!,
      startDate: format(formData.value.startDate, 'yyyy-MM-dd'),
      endDate: formData.value.endDate ? format(formData.value.endDate, 'yyyy-MM-dd') : null,
      description: formData.value.description,
    }

    emit('save', incomeDTO)
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
