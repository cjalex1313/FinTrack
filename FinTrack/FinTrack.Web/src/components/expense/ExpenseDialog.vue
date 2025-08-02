<template>
  <Dialog
    :visible="true"
    :modal="true"
    :draggable="false"
    :maximized="isMobile"
    :class="{ 'p-dialog-maximized': isMobile }"
    :header="isEditMode ? 'Edit Expense' : 'Add Expense'"
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
          <small v-if="errors.amount" class="p-error">{{ errors.amount }}</small>
        </div>

        <!-- Date Field -->
        <div class="field">
          <label for="date" class="block text-sm font-medium mb-2">Date *</label>
          <DatePicker
            id="date"
            v-model="formData.date"
            date-format="mm/dd/yy"
            class="w-full"
            :class="{ 'p-invalid': errors.date }"
            placeholder="Select date"
          />
          <small v-if="errors.date" class="p-error">{{ errors.date }}</small>
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
            class="w-full"
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
import type { ExpenseDTO, ExpenseBucketDTO } from '@/api/models'
import { EMPTY_GUID } from '@/api/models'
import { format } from 'date-fns'

interface Props {
  householdId: string
  expenseBuckets: ExpenseBucketDTO[]
  expense?: ExpenseDTO
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'save', expense: ExpenseDTO): void
  (e: 'closed'): void
}>()

const { isMobile } = useDeviceType()

// Form state
const saving = ref(false)
const formData = ref<{
  amount: number | null
  date: Date
  expenseBucketId: string | null
  description: string | null
}>({
  amount: null,
  date: new Date(),
  expenseBucketId: null,
  description: null,
})

// Validation errors
const errors = ref<{
  amount?: string
  date?: string
}>({})

// Computed properties
const isEditMode = computed(() => !!props.expense)

// Initialize form data
const initializeForm = () => {
  if (props.expense) {
    // Edit mode - populate with existing expense data
    formData.value = {
      amount: props.expense.amount,
      date: new Date(props.expense.date),
      expenseBucketId: props.expense.expenseBucketId,
      description: props.expense.description,
    }
  } else {
    // Add mode - set defaults
    formData.value = {
      amount: null,
      date: new Date(),
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

  if (!formData.value.date) {
    errors.value.date = 'Date is required'
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
    const expenseDTO: ExpenseDTO = {
      id: props.expense?.id || EMPTY_GUID,
      householdId: props.householdId,
      expenseBucketId: formData.value.expenseBucketId,
      amount: formData.value.amount!,
      date: format(formData.value.date, 'yyyy-MM-dd'),
      description: formData.value.description,
    }

    emit('save', expenseDTO)
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
  color: var(--red-500);
  font-size: 0.875rem;
}

.p-invalid {
  border-color: var(--red-500);
}
</style>
