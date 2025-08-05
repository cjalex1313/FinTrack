<template>
  <Dialog
    :visible="true"
    :modal="true"
    :draggable="false"
    :maximized="isMobile"
    :class="{ 'p-dialog-maximized': isMobile }"
    :header="isEditMode ? 'Edit Expense Bucket' : 'Add Expense Bucket'"
    @update:visible="handleVisibilityChange"
  >
    <div class="w-full md:w-96">
      <div class="space-y-4">
        <!-- Name Field -->
        <div class="field">
          <label for="name" class="block text-sm font-medium mb-2">Name *</label>
          <InputText
            id="name"
            v-model="formData.name"
            class="w-full"
            :class="{ 'p-invalid': errors.name }"
            placeholder="Enter bucket name"
          />
          <small v-if="errors.name" class="p-error text-red-500">{{ errors.name }}</small>
        </div>

        <!-- Monthly Amount Field -->
        <div class="field">
          <label for="monthlyAmount" class="block text-sm font-medium mb-2">Monthly Amount *</label>
          <InputNumber
            id="monthlyAmount"
            v-model="formData.monthlyAmount"
            :min="0"
            :max-fraction-digits="2"
            mode="currency"
            currency="USD"
            locale="en-US"
            class="w-full"
            :class="{ 'p-invalid': errors.monthlyAmount }"
            placeholder="Enter monthly amount"
          />
          <small v-if="errors.monthlyAmount" class="p-error text-red-500">{{
            errors.monthlyAmount
          }}</small>
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
import { Dialog, Button, InputNumber, InputText, Textarea } from 'primevue'
import type { ExpenseBucketDTO } from '@/api/models'
import { EMPTY_GUID } from '@/api/models'

interface Props {
  bucket?: ExpenseBucketDTO
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'save', bucket: ExpenseBucketDTO): void
  (e: 'closed'): void
}>()

const { isMobile } = useDeviceType()

// Form state
const saving = ref(false)
const formData = ref<{
  name: string
  monthlyAmount: number | null
  description?: string | null
}>({
  name: '',
  monthlyAmount: null,
  description: null,
})

// Validation errors
const errors = ref<{
  name?: string
  monthlyAmount?: string
}>({})

// Computed properties
const isEditMode = computed(() => !!props.bucket)

// Initialize form data
const initializeForm = () => {
  if (props.bucket) {
    // Edit mode - populate with existing bucket data
    formData.value = {
      name: props.bucket.name,
      monthlyAmount: props.bucket.monthlyAmount,
      description: props.bucket.description,
    }
  } else {
    // Add mode - set defaults
    formData.value = {
      name: '',
      monthlyAmount: null,
      description: null,
    }
  }
}

// Validation
const validateForm = (): boolean => {
  errors.value = {}
  let isValid = true

  if (!formData.value.name || formData.value.name.trim() === '') {
    errors.value.name = 'Name is required'
    isValid = false
  }

  if (!formData.value.monthlyAmount || formData.value.monthlyAmount <= 0) {
    errors.value.monthlyAmount = 'Monthly amount is required and must be greater than 0'
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
    const bucketDTO: ExpenseBucketDTO = {
      id: props.bucket?.id || EMPTY_GUID,
      householdId: props.bucket?.householdId || EMPTY_GUID,
      name: formData.value.name.trim(),
      monthlyAmount: formData.value.monthlyAmount!,
      description: formData.value.description,
    }

    emit('save', bucketDTO)
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
