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

        <!-- Utilization Progress Bar - Only show if utilizationPercentage is provided -->
        <div
          v-if="utilizationPercentage !== undefined && utilizationPercentage !== null"
          class="mt-3"
        >
          <div class="flex justify-between items-center mb-1">
            <span class="text-xs font-medium text-gray-600">Budget Used</span>
            <span class="text-xs font-medium" :class="getUtilizationColorClass()">
              {{ utilizationPercentage.toFixed(1) }}%
            </span>
          </div>
          <div class="w-full bg-gray-200 rounded-full h-2">
            <div
              class="h-2 rounded-full transition-all duration-300"
              :class="getUtilizationBarColorClass()"
              :style="{ width: `${Math.min(utilizationPercentage, 100)}%` }"
            ></div>
          </div>
        </div>
      </div>
      <div class="text-right">
        <div class="text-lg font-bold text-red-600">${{ bucket.monthlyAmount.toFixed(2) }}</div>
        <span
          class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-orange-100 text-orange-800 mt-1"
        >
          Expense Bucket
        </span>

        <!-- Action Buttons -->
        <div v-if="showActionButtons" class="flex gap-2 mt-3">
          <Button
            icon="pi pi-pencil"
            size="small"
            severity="secondary"
            outlined
            @click="$emit('edit', bucket)"
            aria-label="Edit expense bucket"
          />
          <Button
            icon="pi pi-trash"
            size="small"
            severity="danger"
            outlined
            @click="$emit('delete', bucket)"
            aria-label="Delete expense bucket"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ExpenseBucketDTO } from '@/api/models'
import { Button } from 'primevue'

interface Props {
  bucket: ExpenseBucketDTO
  utilizationPercentage?: number | null
  showActionButtons?: boolean
}

const props = defineProps<Props>()

const emit = defineEmits<{
  edit: [bucket: ExpenseBucketDTO]
  delete: [bucket: ExpenseBucketDTO]
}>()

// Helper function to get the appropriate color class for utilization percentage text
const getUtilizationColorClass = () => {
  const percentage = props.utilizationPercentage || 0
  if (percentage >= 90) return 'text-red-600'
  if (percentage >= 75) return 'text-orange-600'
  if (percentage >= 50) return 'text-yellow-600'
  return 'text-green-600'
}

// Helper function to get the appropriate color class for utilization progress bar
const getUtilizationBarColorClass = () => {
  const percentage = props.utilizationPercentage || 0
  if (percentage >= 90) return 'bg-red-500'
  if (percentage >= 75) return 'bg-orange-500'
  if (percentage >= 50) return 'bg-yellow-500'
  return 'bg-green-500'
}
</script>
