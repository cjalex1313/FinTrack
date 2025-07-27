<template>
  <div class="text-center mt-20">
    <ProgressSpinner />
  </div>
</template>

<script setup lang="ts">
import { ProgressSpinner } from 'primevue'
import { onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthApi } from '../../api/authApi'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const route = useRoute()
const authApi = useAuthApi()
const authStore = useAuthStore()

const tryConfirmEmail = async () => {
  if (route.query.token && route.query.userId) {
    const response = await authApi.confirmEmail(
      route.query.userId as string,
      route.query.token as string,
    )
    await authStore.setJwt(response.accessToken)
    router.push({
      name: 'Home',
    })
  }
}

onMounted(async () => {
  await tryConfirmEmail()
})
</script>
