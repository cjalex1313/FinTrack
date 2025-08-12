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
            Welcome back
          </h2>
          <h4 class="text-center text-lg tracking-tight text-gray-800">
            Enter your credentials to access your account
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
              <label for="email">Email</label>
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
              <label for="password">Password</label>
            </FloatLabel>
          </div>
        </div>
        <div v-if="emailError" class="mt-1 text-red-700">
          {{ emailError }}
        </div>
        <div v-if="passwordError" class="mt-1 text-red-700">
          {{ passwordError }}
        </div>
        <div class="mt-6">
          <Button @click="tryLogin" label="Login" fluid />
        </div>
        <div class="mt-6">
          <GoogleLogin
              :callback="signInWithGoogle"
              prompt
            />
        </div>
        <div class="text-center mt-3">
          Don't have an account?
          <RouterLink class="hover:underline text-[#3bbfa1]" :to="{ name: 'Register' }">
            Sign up
          </RouterLink>
        </div>
        <div class="text-center mt-2">
          <RouterLink class="hover:underline text-[#3bbfa1]" :to="{ name: 'ForgotPassword' }">
            Forgot your password?
          </RouterLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { InputText, FloatLabel, Password, Button } from 'primevue'
import { useRouter } from 'vue-router'
import { useVuelidate } from '@vuelidate/core'
import { required } from '@vuelidate/validators'
import { computed, reactive } from 'vue'
import { useAuthApi } from '../../api/authApi'
import { useAuthStore } from '../../stores/auth'
import { decodeCredential } from 'vue3-google-login'

const router = useRouter()
const authApi = useAuthApi()
const authStore = useAuthStore()

const authData = reactive({
  email: '',
  password: '',
})

const rules = {
  email: {
    required,
  },
  password: {
    required,
  },
}

const v$ = useVuelidate(rules, authData)

const successfulLogin = async (response) =>{
  await authStore.setJwt(response.accessToken)
  router.push({
    name: 'Home',
  })
}
const tryLogin = async () => {
  v$.value.$touch()
  if (v$.value.$invalid) {
    return
  }
  const response = await authApi.login(authData.email, authData.password)
  await successfulLogin(response)
}

const signInWithGoogle = async (response) => {
  if (response.credential) {
        const userData = decodeCredential(response.credential);
        const response2 = await authApi.externalLoginCallback({email: userData["email"], providerKey: userData["sub"]});
        await successfulLogin(response2)
      } else {
        console.error('Google login failed:', response);
      }
}

const emailError = computed(() => {
  if (!v$.value.email.$dirty) {
    return ''
  }
  if (v$.value.email.required.$invalid) {
    return 'Email is required'
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
</script>


<style scoped>
.g-btn-wrapper {
  width: 100%;
}
</style>
