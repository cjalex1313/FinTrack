<script setup lang="ts">
import { RouterView, useRoute } from 'vue-router'
import NavBar from '@/components/common/NavBar.vue'
import { useHouseholdStore } from '@/stores/household'
import { onBeforeMount } from 'vue'

const route = useRoute()
const householdStore = useHouseholdStore()

onBeforeMount(async () => {
  await householdStore.loadHouseholds()
})
</script>

<template>
  <div class="min-h-full">
    <NavBar />
    <RouterView :key="route.fullPath" />
  </div>
</template>
