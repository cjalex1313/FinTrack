<script setup lang="ts">
import { RouterView, useRoute } from 'vue-router'
import NavBar from '@/components/common/NavBar.vue'
import { useHouseholdStore } from '@/stores/household'
import { onBeforeMount, onMounted, ref } from 'vue'
import { useHouseholdApi } from '@/api/householdApi'

const route = useRoute()
const householdStore = useHouseholdStore()
const loading = ref<boolean>(false)

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
