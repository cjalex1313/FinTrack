<template>
  <Dialog
    :visible="true"
    :modal="true"
    :draggable="false"
    :maximized="isMobile"
    :class="{ 'p-dialog-maximized': isMobile }"
    header="Invite Member"
    @update:visible="handleVisibilityChange"
  >
    <div class="w-full md:w-96">
      <div class="space-y-4">
        <!-- Email Field -->
        <div class="field">
          <label for="email" class="block text-sm font-medium mb-2">Email Address *</label>
          <InputText
            id="email"
            v-model="formData.email"
            type="email"
            class="w-full"
            :class="{ 'p-invalid': errors.email }"
            placeholder="Enter email address"
            @keyup.enter="handleSave"
          />
          <small v-if="errors.email" class="p-error text-red-500">{{ errors.email }}</small>
        </div>

        <!-- Information Message -->
        <div class="bg-blue-50 border border-blue-200 rounded-lg p-3">
          <div class="flex items-start">
            <svg
              class="w-5 h-5 text-blue-400 mt-0.5 mr-2 flex-shrink-0"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
              ></path>
            </svg>
            <div class="text-sm text-blue-700">
              <p class="font-medium">Invitation Process</p>
              <p>
                An email invitation will be sent to this address. The recipient will need to sign up
                or log in to accept the invitation and join your household.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <template #footer>
      <div class="flex justify-end space-x-2">
        <Button label="Cancel" severity="secondary" @click="handleCancel" :disabled="loading" />
        <Button
          label="Send Invitation"
          @click="handleSave"
          :loading="loading"
          :disabled="!isFormValid"
        />
      </div>
    </template>
  </Dialog>
</template>

<script setup lang="ts">
import { computed, reactive, ref } from 'vue'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Button from 'primevue/button'
import { useDeviceType } from '@/composables/useDeviceType'
import { useHouseholdApi } from '@/api/householdApi'
import { useCustomToast } from '@/composables/useCustomToast'

// Props
interface Props {
  householdId: string
}

const props = defineProps<Props>()

// Emits
const emit = defineEmits<{
  close: []
  invited: []
}>()

// Composables
const { isMobile } = useDeviceType()
const householdApi = useHouseholdApi()
const toast = useCustomToast()

// Reactive state
const loading = ref(false)
const formData = reactive({
  email: '',
})

const errors = reactive({
  email: '',
})

// Computed
const isFormValid = computed(() => {
  return formData.email.trim() !== '' && isValidEmail(formData.email) && !errors.email
})

// Validation functions
const isValidEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

const validateForm = (): boolean => {
  // Reset errors
  errors.email = ''

  // Validate email
  if (!formData.email.trim()) {
    errors.email = 'Email address is required'
    return false
  }

  if (!isValidEmail(formData.email)) {
    errors.email = 'Please enter a valid email address'
    return false
  }

  return true
}

// Event handlers
const handleVisibilityChange = (visible: boolean) => {
  if (!visible) {
    emit('close')
  }
}

const handleCancel = () => {
  emit('close')
}

const handleSave = async () => {
  if (!validateForm()) {
    return
  }

  try {
    loading.value = true
    await householdApi.inviteMember(props.householdId, formData.email.trim())

    toast.add({
      severity: 'success',
      summary: 'Invitation Sent',
      detail: `Invitation has been sent to ${formData.email}`,
      life: 3000,
    })

    emit('invited')
  } catch (error: any) {
    console.error('Failed to invite member:', error)

    let errorMessage = 'Failed to send invitation. Please try again.'

    // Handle specific error cases
    if (error.response?.status === 400) {
      errorMessage = 'Invalid email address or user already invited.'
    } else if (error.response?.status === 409) {
      errorMessage = 'User is already a member of this household.'
    }

    toast.add({
      severity: 'error',
      summary: 'Invitation Failed',
      detail: errorMessage,
      life: 5000,
    })
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.field {
  margin-bottom: 1rem;
}

.p-error {
  font-size: 0.875rem;
  margin-top: 0.25rem;
}
</style>
