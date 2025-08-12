<template>
  <div
    class="bg-white border border-gray-200 rounded-lg p-4 mb-3 shadow-sm hover:shadow-md transition-shadow"
  >
    <div class="flex justify-between items-start">
      <div class="flex-1">
        <h3 class="font-semibold text-slate-800 mb-1">
          {{ expense.description || 'Recurring Expense' }}
        </h3>
        <p class="text-sm text-gray-600 mb-2">{{ getBucketName(expense.expenseBucketId) }}</p>
        <div class="flex items-center gap-2 text-sm text-gray-500 mb-2">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 002 2z"
            ></path>
          </svg>
          <span>Next: {{ formatDate(expense.nextDate) }}</span>
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
          <span>{{ getRecurrenceLabel(expense.recurrence) }}</span>
        </div>
      </div>
      <div class="text-right">
        <div class="text-lg font-bold text-red-600">-${{ expense.amount.toFixed(2) }}</div>
        <span
          class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-orange-100 text-orange-800 mt-1"
        >
          Recurring
        </span>
      </div>
    </div>

    <!-- Action Buttons -->
    <div v-if="showActionButtons" class="flex justify-end gap-2 mt-4 pt-4 border-t border-gray-100">
      <Button
        icon="pi pi-pencil"
        size="small"
        severity="secondary"
        outlined
        @click="emit('edit', expense)"
        aria-label="Edit recurring expense"
      />
      <Button
        icon="pi pi-trash"
        size="small"
        severity="danger"
        outlined
        @click="emit('delete', expense)"
        aria-label="Delete recurring expense"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import type { RecurringExpenseDTO, ExpenseBucketDTO } from '@/api/models'
import { RecurrenceType } from '@/models/recurrenceType'
import { Button } from 'primevue'

interface Props {
  expense: RecurringExpenseDTO
  expenseBuckets?: ExpenseBucketDTO[]
  showActionButtons?: boolean
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'edit', expense: RecurringExpenseDTO): void
  (e: 'delete', expense: RecurringExpenseDTO): void
}>()

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
