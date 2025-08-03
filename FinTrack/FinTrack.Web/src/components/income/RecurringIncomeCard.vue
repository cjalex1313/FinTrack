<template>
  <div
    class="bg-white border border-gray-200 rounded-lg p-4 mb-3 shadow-sm hover:shadow-md transition-shadow"
  >
    <div class="flex justify-between items-start mb-3">
      <div class="flex-1">
        <h3 class="font-semibold text-slate-800 mb-1">{{ income.description }}</h3>
        <!-- <p class="text-sm text-gray-600 mb-2">Source</p> -->
        <div class="flex items-center gap-2 text-sm text-gray-500">
          <svg
            v-if="!showDate"
            class="w-4 h-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
            ></path>
          </svg>
          <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
            ></path>
          </svg>
          <span v-if="!showDate">{{ formatRecurrence(income.recurrence) }}</span>
          <span v-else>{{ formatDateRange(income.startDate, income.endDate) }}</span>
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

    <div v-if="showActionButtons" class="flex gap-3 pt-3 border-t border-gray-100">
      <Button
        @click="$emit('edit', income)"
        size="small"
        severity="secondary"
        outlined
        class="flex-1"
      >
        <i class="pi pi-pencil mr-2"></i>
        Edit
      </Button>
      <Button
        @click="$emit('delete', income)"
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
import type { RecurringIncomeDTO } from '@/api/models'
import type { RecurrenceType } from '@/models/recurrenceType'
import { Button } from 'primevue'

interface Props {
  income: RecurringIncomeDTO
  showDate?: boolean
  showActionButtons?: boolean
}

defineProps<Props>()

defineEmits<{
  (e: 'edit', income: RecurringIncomeDTO): void
  (e: 'delete', income: RecurringIncomeDTO): void
}>()

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

const formatDateRange = (startDate: string, endDate?: string | null) => {
  const start = new Date(startDate)
  const formattedStart = start.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })

  if (!endDate) {
    return `From ${formattedStart}`
  }

  const end = new Date(endDate)
  const formattedEnd = end.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })

  return `${formattedStart} - ${formattedEnd}`
}
</script>
