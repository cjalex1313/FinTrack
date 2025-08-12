<template>
  <div class="text-center mt-20">
    <div v-if="loading">
      <ProgressSpinner />
      <p class="mt-4 text-gray-600">Confirming your email...</p>
    </div>

    <div v-else-if="passwordSetNeeded" class="max-w-md mx-auto">
      <div class="bg-white p-8 rounded-lg shadow-md">
        <h1 class="text-2xl font-bold text-gray-900 mb-6">Set Your Password</h1>
        <p class="text-gray-600 mb-6">
          Your email has been confirmed. Please set a password to complete your account setup.
        </p>

        <form @submit.prevent="setUserPassword" class="space-y-4">
          <div>
            <label for="password" class="block text-sm font-medium text-gray-700 mb-2">
              New Password
            </label>
            <InputText
              id="password"
              v-model="password"
              type="password"
              class="w-full"
              :class="{ 'p-invalid': passwordError }"
              placeholder="Enter your new password"
              required
            />
            <small v-if="passwordError" class="p-error">{{ passwordError }}</small>
          </div>

          <div>
            <label for="confirmPassword" class="block text-sm font-medium text-gray-700 mb-2">
              Confirm Password
            </label>
            <InputText
              id="confirmPassword"
              v-model="confirmPassword"
              type="password"
              class="w-full"
              :class="{ 'p-invalid': confirmPasswordError }"
              placeholder="Confirm your new password"
              required
            />
            <small v-if="confirmPasswordError" class="p-error">{{ confirmPasswordError }}</small>
          </div>

          <Button
            type="submit"
            label="Set Password"
            icon="pi pi-check"
            :loading="settingPassword"
            :disabled="!isPasswordValid"
            class="w-full"
          />
        </form>
      </div>
    </div>

    <div v-else-if="error" class="max-w-md mx-auto">
      <div class="bg-red-50 border border-red-200 rounded-lg p-6">
        <i class="pi pi-exclamation-triangle text-red-500 text-3xl mb-4"></i>
        <h2 class="text-xl font-semibold text-red-800 mb-2">Email Confirmation Failed</h2>
        <p class="text-red-600 mb-4">{{ error }}</p>
        <Button label="Go to Login" icon="pi pi-sign-in" @click="goToLogin" class="w-full" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ProgressSpinner, Button, InputText } from 'primevue'
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthApi } from '../../api/authApi'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const route = useRoute()
const authApi = useAuthApi()
const authStore = useAuthStore()

const loading = ref(true)
const passwordSetNeeded = ref(false)
const error = ref('')
const password = ref('')
const confirmPassword = ref('')
const settingPassword = ref(false)

const passwordError = computed(() => {
  if (!password.value) return ''
  if (password.value.length < 6) return 'Password must be at least 6 characters long'
  return ''
})

const confirmPasswordError = computed(() => {
  if (!confirmPassword.value) return ''
  if (password.value !== confirmPassword.value) return 'Passwords do not match'
  return ''
})

const isPasswordValid = computed(() => {
  return (
    password.value &&
    confirmPassword.value &&
    password.value === confirmPassword.value &&
    password.value.length >= 6 &&
    !passwordError.value &&
    !confirmPasswordError.value
  )
})

const tryConfirmEmail = async () => {
  if (route.query.token && route.query.userId) {
    try {
      const response = await authApi.confirmEmail(
        route.query.userId as string,
        route.query.token as string,
      )

      if (response.passwordSetNeeded) {
        passwordSetNeeded.value = true
      } else {
        router.push({ name: 'Home' })
      }
      await authStore.setJwt(response.accessToken)
    } catch (err) {
      error.value = 'Failed to confirm email. The link may be invalid or expired.'
    } finally {
      loading.value = false
    }
  } else {
    error.value = 'Invalid confirmation link. Missing required parameters.'
    loading.value = false
  }
}

const setUserPassword = async () => {
  if (!isPasswordValid.value) return

  settingPassword.value = true
  try {
    await authApi.setPassword(password.value)

    router.push({ name: 'Home' })
  } catch (err) {
    error.value = 'Failed to set password. Please try again.'
    settingPassword.value = false
  }
}

const goToLogin = () => {
  router.push({ name: 'Login' })
}

onMounted(async () => {
  await tryConfirmEmail()
})
</script>
