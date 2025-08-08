<template>
  <div
    class="bg-white border border-gray-200 rounded-lg p-4 mb-3 shadow-sm hover:shadow-md transition-shadow"
  >
    <div class="flex justify-between items-start">
      <div class="flex-1">
        <h3 class="font-semibold text-slate-800 mb-1">{{ expense.description }}</h3>
        <p class="text-sm text-gray-600 mb-2">{{ getBucketName(expense.expenseBucketId) }}</p>
        <div class="flex items-center gap-2 text-sm text-gray-500">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
            ></path>
          </svg>
          <span>{{ formatDate(expense.date) }}</span>
        </div>
      </div>
      <div class="text-right">
        <div class="text-lg font-bold text-red-600">-${{ expense.amount.toFixed(2) }}</div>
        <span
          class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-red-100 text-red-800 mt-1"
        >
          Expense
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ExpenseDTO, ExpenseBucketDTO } from '@/api/models'

interface Props {
  expense: ExpenseDTO
  expenseBuckets?: ExpenseBucketDTO[] | null
}

const props = defineProps<Props>()

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}

const getBucketName = (bucketId: string | null) => {
  if (!props.expenseBuckets) return 'Unknown bucket'
  const bucket = props.expenseBuckets.find((b) => b.id === bucketId)
  return bucket ? bucket.name : 'Unknown bucket'
}
</script>
