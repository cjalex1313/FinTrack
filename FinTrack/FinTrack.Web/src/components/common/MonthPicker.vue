<template>
  <div class="flex items-center gap-3 bg-white rounded-lg shadow-sm border border-gray-200 p-4">
    <Button
      @click="previousMonth"
      icon="pi pi-chevron-left"
      severity="secondary"
      text
      rounded
      size="small"
      :disabled="loading"
    />

    <div class="flex-1 text-center">
      <DatePicker
        v-model="selectedDate"
        view="month"
        date-format="MM - yy"
        class="w-full"
        placeholder="Select month"
        :disabled="loading"
        @update:model-value="(value) => onDateChange(value as Date)"
      />
    </div>

    <Button
      @click="nextMonth"
      icon="pi pi-chevron-right"
      severity="secondary"
      text
      rounded
      size="small"
      :disabled="loading"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { Button, DatePicker } from 'primevue'

interface Props {
  modelValue?: Date
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => new Date(),
  loading: false,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: Date): void
  (e: 'change', value: Date): void
}>()

const selectedDate = ref<Date>(new Date())

// Initialize with prop value
onMounted(() => {
  if (props.modelValue) {
    selectedDate.value = new Date(props.modelValue)
  }
})

// Watch for external changes
watch(
  () => props.modelValue,
  (newValue) => {
    if (newValue) {
      selectedDate.value = new Date(newValue)
    }
  },
  { immediate: true },
)

const previousMonth = () => {
  const newDate = new Date(selectedDate.value)
  newDate.setMonth(newDate.getMonth() - 1)
  selectedDate.value = newDate
  emitChange(newDate)
}

const nextMonth = () => {
  const newDate = new Date(selectedDate.value)
  newDate.setMonth(newDate.getMonth() + 1)
  selectedDate.value = newDate
  emitChange(newDate)
}

const onDateChange = (value: Date) => {
  if (value) {
    selectedDate.value = value
    emitChange(value)
  }
}

const emitChange = (date: Date) => {
  emit('update:modelValue', date)
  emit('change', date)
}
</script>

<style scoped>
/* Custom styling for the date picker if needed */
</style>
