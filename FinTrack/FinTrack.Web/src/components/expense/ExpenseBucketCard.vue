<template>
  <div
    class="bg-white border border-gray-200 rounded-lg p-4 mb-3 shadow-sm hover:shadow-md transition-shadow"
  >
    <div class="flex justify-between items-start">
      <div class="flex-1">
        <h3 class="font-semibold text-slate-800 mb-1">{{ bucket.name }}</h3>
        <p class="text-sm text-gray-600 mb-2">{{ bucket.description }}</p>
        <div class="flex items-center gap-2 text-sm text-gray-500">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
            ></path>
          </svg>
          <span>Monthly Budget</span>
        </div>
      </div>
      <div class="text-right">
        <div class="text-lg font-bold text-red-600">${{ bucket.monthlyAmount.toFixed(2) }}</div>
        <span
          class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-orange-100 text-orange-800 mt-1"
        >
          Expense Bucket
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ExpenseBucketDTO } from '@/api/models'

interface Props {
  bucket: ExpenseBucketDTO
}

defineProps<Props>()
</script>
