<template>
  <div
    class="bg-white border border-gray-200 rounded-lg p-4 mb-3 shadow-sm hover:shadow-md transition-shadow"
  >
    <div class="flex justify-between items-start">
      <div class="flex-1">
        <div class="flex items-center gap-2 mb-1">
          <h3 class="font-semibold text-slate-800">
            {{ plannedExpense.description || 'Recurring Expense' }}
          </h3>
          <span
            v-if="plannedExpense.occurrence > 1"
            class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800"
          >
            #{{ plannedExpense.occurrence }}
          </span>
        </div>
        <p class="text-sm text-gray-600 mb-2">
          {{ getBucketName(plannedExpense.expenseBucketId) }}
        </p>
        <div class="flex items-center gap-2 text-sm text-gray-500 mb-2">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 002 2z"
            ></path>
          </svg>
          <span>Scheduled: {{ formatDate(plannedExpense.scheduledDate) }}</span>
        </div>
        <div class="flex items-center gap-2 text-sm text-gray-500">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
            ></path>
          </svg>
          <span>{{ getRecurrenceLabel(plannedExpense.recurrence) }}</span>
        </div>
      </div>
      <div class="text-right">
        <div class="text-lg font-bold text-orange-600">
          -${{ plannedExpense.amount.toFixed(2) }}
        </div>
        <span
          class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-orange-100 text-orange-800 mt-1"
        >
          Planned
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { RecurringExpenseDTO, ExpenseBucketDTO } from '@/api/models'
import { RecurrenceType } from '@/models/recurrenceType'

interface PlannedExpense extends RecurringExpenseDTO {
  scheduledDate: string
  occurrence: number
}

interface Props {
  plannedExpense: PlannedExpense
  expenseBuckets?: ExpenseBucketDTO[]
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

const getRecurrenceLabel = (recurrence: RecurrenceType) => {
  switch (recurrence) {
    case RecurrenceType.Daily:
      return 'Daily'
    case RecurrenceType.Weekly:
      return 'Weekly'
    case RecurrenceType.BiWeekly:
      return 'Bi-Weekly'
    case RecurrenceType.Monthly:
      return 'Monthly'
    case RecurrenceType.Quarterly:
      return 'Quarterly'
    case RecurrenceType.Yearly:
      return 'Yearly'
    default:
      return 'Unknown'
  }
}
</script>
