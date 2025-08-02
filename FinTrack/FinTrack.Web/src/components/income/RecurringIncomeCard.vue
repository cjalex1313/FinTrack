<template>
  <div
    class="bg-white border border-gray-200 rounded-lg p-4 mb-3 shadow-sm hover:shadow-md transition-shadow"
  >
    <div class="flex justify-between items-start">
      <div class="flex-1">
        <h3 class="font-semibold text-slate-800 mb-1">{{ income.description }}</h3>
        <p class="text-sm text-gray-600 mb-2">{{ income.source }}</p>
        <div class="flex items-center gap-2 text-sm text-gray-500">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
            ></path>
          </svg>
          <span>{{ formatRecurrence(income.recurrence) }}</span>
        </div>
      </div>
      <div class="text-right">
        <div class="text-lg font-bold text-green-600">${{ income.amount.toFixed(2) }}</div>
        <span
          class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-purple-100 text-purple-800 mt-1"
        >
          Recurring
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { RecurringIncomeDTO } from '@/api/models'
import type { RecurrenceType } from '@/models/recurrenceType'

interface Props {
  income: RecurringIncomeDTO
}

defineProps<Props>()

const formatRecurrence = (recurrence: RecurrenceType) => {
  const recurrenceMap = {
    0: 'Daily',
    1: 'Weekly',
    2: 'Bi-weekly',
    3: 'Monthly',
    4: 'Quarterly',
    5: 'Yearly',
  }
  return recurrenceMap[recurrence] || 'Unknown'
}
</script>
