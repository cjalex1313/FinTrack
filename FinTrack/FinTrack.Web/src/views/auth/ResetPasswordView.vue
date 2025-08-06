<template>
  <div class="flex min-h-full flex-col justify-center py-12 sm:px-6 lg:px-8">
    <div class="mt-10 sm:mx-auto sm:w-full sm:max-w-[480px]">
      <div class="bg-white px-6 py-12 shadow-sm sm:rounded-lg sm:px-12">
        <div class="sm:mx-auto sm:w-full sm:max-w-md">
          <img
            class="mx-auto h-10 w-auto"
            src="https://tailwindcss.com/plus-assets/img/logos/mark.svg?color=indigo&shade=600"
            alt="Your Company"
          />
          <h2 class="mt-6 text-center text-3xl/9 font-bold tracking-tight text-gray-900">
            Reset Password
          </h2>
          <h4 class="text-center text-lg tracking-tight text-gray-800">
            Enter your new password below
          </h4>
        </div>
        <div class="mt-6">
          <div>
            <FloatLabel variant="on">
              <Password
                v-model="password"
                id="password"
                class="w-full"
                toggleMask
                fluid
                :invalid="!!passwordError"
                :feedback="false"
              />
              <label for="password">New Password</label>
            </FloatLabel>
          </div>
          <div class="mt-4">
            <FloatLabel variant="on">
              <Password
                v-model="confirmPassword"
                id="confirmPassword"
                class="w-full"
                toggleMask
                fluid
                :invalid="!!confirmPasswordError"
                :feedback="false"
              />
              <label for="confirmPassword">Confirm New Password</label>
            </FloatLabel>
          </div>
        </div>
        <div v-if="passwordError" class="mt-1 text-red-700">
          {{ passwordError }}
        </div>
        <div v-if="confirmPasswordError" class="mt-1 text-red-700">
          {{ confirmPasswordError }}
        </div>
        <div v-if="generalError" class="mt-1 text-red-700">
          {{ generalError }}
        </div>
        <div v-if="successMessage" class="mt-1 text-green-700">
          {{ successMessage }}
        </div>
        <div class="mt-6">
          <Button
            @click="resetPassword"
            label="Reset Password"
            fluid
            :loading="isLoading"
            :disabled="isLoading"
          />
        </div>
        <div class="text-center mt-3">
          Remember your password?
          <RouterLink class="hover:underline text-[#3bbfa1]" :to="{ name: 'Login' }">
            Sign in
          </RouterLink>
        </div>
        <div class="text-center mt-2">
          <RouterLink class="hover:underline text-[#3bbfa1]" :to="{ name: 'ForgotPassword' }">
            Request new reset link
          </RouterLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Password, FloatLabel, Button } from 'primevue'
import { useRouter, useRoute } from 'vue-router'
import { useVuelidate } from '@vuelidate/core'
import { required, minLength } from '@vuelidate/validators'
import { computed, ref, onMounted } from 'vue'
import { useAuthApi } from '../../api/authApi'

const router = useRouter()
const route = useRoute()
const authApi = useAuthApi()

const password = ref('')
const confirmPassword = ref('')
const isLoading = ref(false)
const successMessage = ref('')
const generalError = ref('')

const userId = ref('')
const token = ref('')

onMounted(() => {
  const userIdParam = route.query.userId as string
  const tokenParam = route.query.resetToken as string

  if (!userIdParam || !tokenParam) {
    generalError.value = 'Invalid reset link. Please request a new password reset.'
    return
  }

  userId.value = userIdParam
  token.value = tokenParam
})

const rules = {
  password: {
    required,
    minLength: minLength(6),
  },
  confirmPassword: {
    required,
  },
}

const v$ = useVuelidate(rules, { password, confirmPassword })

const resetPassword = async () => {
  v$.value.$touch()
  if (v$.value.$invalid) {
    return
  }

  if (password.value !== confirmPassword.value) {
    return
  }

  if (!userId.value || !token.value) {
    generalError.value = 'Invalid reset link. Please request a new password reset.'
    return
  }

  isLoading.value = true
  try {
    await authApi.resetPassword(userId.value, token.value, password.value)
    successMessage.value = 'Password has been reset successfully. Redirecting to login...'

    // Redirect to login after a short delay
    setTimeout(() => {
      router.push({ name: 'Login' })
    }, 2000)
  } catch (error) {
    console.error('Error resetting password:', error)
    generalError.value = 'Failed to reset password. Please try again or request a new reset link.'
  } finally {
    isLoading.value = false
  }
}

const passwordError = computed(() => {
  if (!v$.value.password.$dirty) {
    return ''
  }
  if (v$.value.password.required.$invalid) {
    return 'Password is required'
  }
  if (v$.value.password.minLength.$invalid) {
    return 'Password must be at least 6 characters long'
  }
  return ''
})

const confirmPasswordError = computed(() => {
  if (!v$.value.confirmPassword.$dirty) {
    return ''
  }
  if (v$.value.confirmPassword.required.$invalid) {
    return 'Please confirm your password'
  }
  if (password.value && confirmPassword.value && password.value !== confirmPassword.value) {
    return 'Passwords do not match'
  }
  return ''
})
</script>
