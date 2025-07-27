<script setup lang="ts">
import { RouterView } from 'vue-router'
import Toast from 'primevue/toast'
import { ConfirmDialog } from 'primevue'
import { useAuthStore } from './stores/auth'

const authStore = useAuthStore()

authStore.getProfile()
</script>

<template>
  <RouterView />
  <Toast />
  <ConfirmDialog></ConfirmDialog>
</template>

<style scoped></style>
