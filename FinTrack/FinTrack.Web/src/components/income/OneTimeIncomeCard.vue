<template>
  <div
    class="bg-white border border-gray-200 rounded-lg p-4 mb-3 shadow-sm hover:shadow-md transition-shadow"
  >
    <div class="flex justify-between items-start mb-3">
      <div class="flex-1">
        <h3 class="font-semibold text-slate-800 mb-1">{{ income.description }}</h3>
        <!-- <p class="text-sm text-gray-600 mb-2">Source</p> -->
        <div v-if="showDate" class="flex items-center gap-2 text-sm text-gray-500">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
            ></path>
          </svg>
          <span>{{ formatDate(income.date) }}</span>
        </div>
      </div>
      <div class="text-right">
        <div class="text-lg font-bold text-green-600">${{ income.amount.toFixed(2) }}</div>
        <span
          class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800 mt-1"
        >
          One-time
        </span>
      </div>
    </div>

    <div v-if="showActionButtons" class="flex gap-3 pt-3 border-t border-gray-100">
      <Button
        @click="emit('edit', income)"
        size="small"
        severity="secondary"
        outlined
        class="flex-1"
      >
        <i class="pi pi-pencil mr-2"></i>
        Edit
      </Button>
      <Button
        @click="emit('delete', income)"
        size="small"
        severity="danger"
        outlined
        class="flex-1"
      >
        <i class="pi pi-trash mr-2"></i>
        Delete
      </Button>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { OneTimeIncomeDTO } from '@/api/models'
import { Button } from 'primevue'

interface Props {
  income: OneTimeIncomeDTO
  showDate?: boolean
  showActionButtons?: boolean
}

defineProps<Props>()

defineEmits<{
  (e: 'edit', income: OneTimeIncomeDTO): void
  (e: 'delete', income: OneTimeIncomeDTO): void
}>()

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}
</script>
