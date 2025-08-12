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
          <!-- <button @click="signInWithGoogle" class="google-login-btn" fluid>
            <img src="https://developers.google.com/identity/images/g-logo.png" alt="Google logo" />
            Sign in with Google
          </button> -->
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
// import { googleSdkLoaded } from 'vue3-google-login'
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

const tryLogin = async () => {
  v$.value.$touch()
  if (v$.value.$invalid) {
    return
  }
  const response = await authApi.login(authData.email, authData.password)
  await authStore.setJwt(response.accessToken)
  router.push({
    name: 'Home',
  })
}

const signInWithGoogle = async (response) => {
  debugger
  if (response.credential) {
        const userData = decodeCredential(response.credential);
        console.log('User Data:', userData);
        // Here, you can send userData to your backend for registration or
        // handle user data for client-side registration/login.
        // Example: Call an API to register the user with their Google details.
        const p = await authApi.externalLoginCallback({email: userData["email"]})
      } else {
        console.error('Google login failed:', response);
      }
  // googleSdkLoaded(google => {
  //   google.accounts.oauth2
  //     .initCodeClient({
  //       client_id: '432211571385-482i3g0pnqo5h7kcmke8fc9ge9409uts.apps.googleusercontent.com',
  //       scope: 'email profile openid',
  //       redirect_uri: "http://localhost:5173/auth/signin-oidc",
  //       callback: (response) => {
  //         debugger;
  //         console.log(response)
  //       //   if (response.code) {
  //       //     fetchUserDataFrom(response.code)
  //       // }
  //     }
  //   })
  //     .requestCode()
  // })
}

const emailError = computed(() => {
  if (!v$.value.email.$dirty) {
    return ''
  }
  if (v$.value.email!.required!.$invalid) {
    return 'Email is required'
  }
  return ''
})

const passwordError = computed(() => {
  if (!v$.value.password.$dirty) {
    return ''
  }
  if (v$.value.password!.required!.$invalid) {
    return 'Password is required'
  }
  return ''
})
</script>


<style scoped>
.google-login-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 16px;
  background-color: white;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-weight: bold;
  cursor: pointer;
}

.google-login-btn img {
  width: 20px;
  height: 20px;
}
</style>
