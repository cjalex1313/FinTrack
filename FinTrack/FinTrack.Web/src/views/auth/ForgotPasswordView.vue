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
            Forgot Password
          </h2>
          <h4 class="text-center text-lg tracking-tight text-gray-800">
            Enter your email address and we'll send you a link to reset your password
          </h4>
        </div>
        <div class="mt-6">
          <div>
            <FloatLabel variant="on">
              <InputText
                v-model="email"
                id="email"
                class="w-full"
                :invalid="!!emailError"
                type="email"
              />
              <label for="email">Email</label>
            </FloatLabel>
          </div>
        </div>
        <div v-if="emailError" class="mt-1 text-red-700">
          {{ emailError }}
        </div>
        <div v-if="successMessage" class="mt-1 text-green-700">
          {{ successMessage }}
        </div>
        <div class="mt-6">
          <Button
            @click="requestPasswordReset"
            label="Send Reset Link"
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
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { InputText, FloatLabel, Button } from 'primevue'
import { useVuelidate } from '@vuelidate/core'
import { required, email as emailValidator } from '@vuelidate/validators'
import { computed, ref } from 'vue'
import { useAuthApi } from '../../api/authApi'

const authApi = useAuthApi()

const email = ref('')
const isLoading = ref(false)
const successMessage = ref('')

const rules = {
  email: {
    required,
    email: emailValidator,
  },
}

const v$ = useVuelidate(rules, { email })

const requestPasswordReset = async () => {
  v$.value.$touch()
  if (v$.value.$invalid) {
    return
  }

  isLoading.value = true
  try {
    await authApi.forgotPassword(email.value)
    successMessage.value = 'Password reset link has been sent to your email address.'
    email.value = ''
    v$.value.$reset()
  } catch (error) {
    console.error('Error requesting password reset:', error)
    // You might want to show a more specific error message here
  } finally {
    isLoading.value = false
  }
}

const emailError = computed(() => {
  if (!v$.value.email.$dirty) {
    return ''
  }
  if (v$.value.email.required.$invalid) {
    return 'Email is required'
  }
  if (v$.value.email.email.$invalid) {
    return 'Please enter a valid email address'
  }
  return ''
})
</script>
