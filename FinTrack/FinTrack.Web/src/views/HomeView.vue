<template>
  <main>
    <div>Home page</div>
  </main>
</template>

<script setup lang="ts">
import { Role } from '@/models/role'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'
import { watch } from 'vue'

const { authData } = useAuthStore()
const router = useRouter()
</script>
