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
            Create your account
          </h2>
          <h4 class="text-center text-lg tracking-tight text-gray-800">
            Already have an account?
            <RouterLink class="hover:underline text-[#3bbfa1]" :to="{ name: 'Login' }">
              Sign in
            </RouterLink>
          </h4>
        </div>
        <div class="mt-6">
          <div>
            <FloatLabel variant="on">
              <InputText
                v-model="authData.email"
                id="email"
                class="w-full"
                :invalid="!!emailError"
              />
              <label for="email"> Email </label>
            </FloatLabel>
          </div>
          <div class="mt-4">
            <FloatLabel variant="on">
              <Password
                v-model="authData.password"
                id="password"
                class="w-full"
                toggleMask
                fluid
                :invalid="!!passwordError"
                :feedback="false"
              />
              <label for="password"> Password </label>
            </FloatLabel>
          </div>
          <div class="mt-4">
            <FloatLabel variant="on">
              <Password
                v-model="authData.confirmPassword"
                id="password"
                class="w-full"
                toggleMask
                fluid
                :invalid="!!confrimPasswordError"
                :feedback="false"
              />
              <label for="password"> Confirm password </label>
            </FloatLabel>
          </div>
        </div>
        <div v-if="emailError" class="mt-1 text-red-700">
          {{ emailError }}
        </div>
        <div v-if="passwordError" class="mt-1 text-red-700">
          {{ passwordError }}
        </div>
        <div v-if="confrimPasswordError" class="mt-1 text-red-700">
          {{ confrimPasswordError }}
        </div>
        <div v-if="success" class="text-center mt-6">Please check your email</div>
        <div v-else class="mt-6">
          <Button @click="tryRegister" label="Create account" fluid :disabled="isProcessing" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { InputText, FloatLabel, Password, Button } from 'primevue'
import { useVuelidate } from '@vuelidate/core'
import { email, required } from '@vuelidate/validators'
import { computed, reactive, ref } from 'vue'
import { useAuthApi } from '../../api/authApi'

const authApi = useAuthApi()

const authData = reactive({
  email: '',
  password: '',
  confirmPassword: '',
})

const success = ref(false)
const isProcessing = ref(false)

const passwordIsSame = (password: string) => {
  return password === authData.password
}

const rules = {
  email: {
    email,
    required,
  },
  password: {
    required,
  },
  confirmPassword: {
    required,
    passwordIsSame,
  },
}

const v$ = useVuelidate(rules, authData)

const tryRegister = async () => {
  v$.value.$touch()
  if (v$.value.$invalid) {
    return
  }
  isProcessing.value = true
  try {
    await authApi.register(authData.email, authData.password)
    success.value = true
  } finally {
    isProcessing.value = false
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
    return 'Email is invalid'
  }
  return ''
})

const passwordError = computed(() => {
  if (!v$.value.password.$dirty) {
    return ''
  }
  if (v$.value.password.required.$invalid) {
    return 'Password is required'
  }
  return ''
})

const confrimPasswordError = computed(() => {
  if (!v$.value.password.$dirty) {
    return ''
  }
  if (v$.value.password.required.$invalid) {
    return 'Confirm password is required'
  }
  if (v$.value.confirmPassword.passwordIsSame.$invalid) {
    return 'Password do not match'
  }
  return ''
})
</script>
