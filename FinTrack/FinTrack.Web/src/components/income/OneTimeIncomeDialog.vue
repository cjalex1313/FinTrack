<template>
  <Dialog
    :visible="true"
    :modal="true"
    :draggable="false"
    :maximized="isMobile"
    :class="{ 'p-dialog-maximized': isMobile }"
    :header="isEditMode ? 'Edit One-time Income' : 'Add One-time Income'"
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
          <small v-if="errors.date" class="p-error text-red-500">{{ errors.date }}</small>
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
import { Dialog, Button, InputNumber, DatePicker, Textarea } from 'primevue'
import type { OneTimeIncomeDTO } from '@/api/models'
import { EMPTY_GUID } from '@/api/models'
import { format } from 'date-fns'

interface Props {
  income?: OneTimeIncomeDTO
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'save', income: OneTimeIncomeDTO): void
  (e: 'closed'): void
}>()

const { isMobile } = useDeviceType()

// Form state
const saving = ref(false)
const formData = ref<{
  amount: number | null
  date: Date
  description?: string | null
}>({
  amount: null,
  date: new Date(),
  description: null,
})

// Validation errors
const errors = ref<{
  amount?: string
  date?: string
}>({})

// Computed properties
const isEditMode = computed(() => !!props.income)

// Initialize form data
const initializeForm = () => {
  if (props.income) {
    // Edit mode - populate with existing income data
    formData.value = {
      amount: props.income.amount,
      date: new Date(props.income.date),
      description: props.income.description,
    }
  } else {
    // Add mode - set defaults
    formData.value = {
      amount: null,
      date: new Date(),
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
    const incomeDTO: OneTimeIncomeDTO = {
      id: props.income?.id || EMPTY_GUID,
      householdId: props.income?.householdId || EMPTY_GUID,
      amount: formData.value.amount!,
      date: format(formData.value.date, 'yyyy-MM-dd'),
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
