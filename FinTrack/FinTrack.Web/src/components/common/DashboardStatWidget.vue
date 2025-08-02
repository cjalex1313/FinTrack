<template>
  <Card class="dashboard-stat-widget">
    <template #title>
      <span class="widget-title">{{ title }}</span>
    </template>
    <template #content>
      <div class="widget-value" :class="sizeClass">{{ value }}</div>
      <div v-if="subtitle" class="widget-subtitle">{{ subtitle }}</div>
    </template>
  </Card>
</template>

<script setup lang="ts">
import Card from 'primevue/card'
import { computed } from 'vue'

interface Props {
  title: string
  value: string | number
  subtitle?: string
  size?: 'sm' | 'md' | 'lg'
}

const props = defineProps<Props>()

const sizeClass = computed(() => {
  switch (props.size) {
    case 'sm':
      return 'size-sm'
    case 'lg':
      return 'size-lg'
    default:
      return 'size-md'
  }
})
</script>

<style scoped>
.dashboard-stat-widget :deep(.p-card-body) {
  padding-top: 0.5rem;
  padding-bottom: 1rem;
}

.widget-title {
  font-size: 0.9rem;
  color: var(--text-color-secondary, #6b7280);
  font-weight: 600;
  letter-spacing: 0.2px;
}

.widget-value {
  font-weight: 700;
  line-height: 1.1;
  color: var(--text-color, #111827);
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 3.5rem;
}

.size-sm {
  font-size: 1.5rem;
}

.size-md {
  font-size: 2rem;
}

.size-lg {
  font-size: 2.5rem;
}

.widget-subtitle {
  margin-top: 0.25rem;
  font-size: 0.85rem;
  color: var(--text-color-secondary, #6b7280);
  text-align: center;
}
</style>
