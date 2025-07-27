<script setup lang="ts">
import { RouterView, useRoute } from 'vue-router'
import NavBar from '@/components/common/NavBar.vue'

const route = useRoute()
</script>

<template>
  <div class="min-h-full">
    <NavBar />
    <RouterView :key="route.fullPath" />
  </div>
</template>
