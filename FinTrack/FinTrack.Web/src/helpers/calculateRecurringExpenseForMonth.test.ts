import { describe, it, expect } from 'vitest'
import calculateRecurringExpenseForMonth from '@/helpers/calculateRecurringExpenseForMonth'
import { RecurrenceType } from '@/models/recurrenceType'

describe('calculateRecurringExpenseForMonth', () => {
  it('should calculate weekly recurring expense correctly', () => {
    // Weekly expense of $100 starting on the 1st of the month
    const nextDate = '2025-08-01T00:00:00.000Z'
    const recurrence = RecurrenceType.Weekly
    const amount = 100
    const targetMonth = new Date(2025, 7, 1) // August 2025 (month is 0-indexed)

    const result = calculateRecurringExpenseForMonth(nextDate, recurrence, amount, targetMonth)

    // August 2025 has 31 days, starting from Aug 1st with weekly recurrence:
    // Aug 1, Aug 8, Aug 15, Aug 22, Aug 29 = 5 occurrences
    expect(result).toBe(500)
  })

  it('should calculate monthly recurring expense correctly', () => {
    // Monthly expense of $500 on the 15th
    const nextDate = '2025-08-15T00:00:00.000Z'
    const recurrence = RecurrenceType.Monthly
    const amount = 500
    const targetMonth = new Date(2025, 7, 1) // August 2025

    const result = calculateRecurringExpenseForMonth(nextDate, recurrence, amount, targetMonth)

    // Should be 1 occurrence in August
    expect(result).toBe(500)
  })

  it('should return 0 if next date is after target month', () => {
    // Next date is in September, but we're calculating for August
    const nextDate = '2025-09-01T00:00:00.000Z'
    const recurrence = RecurrenceType.Monthly
    const amount = 100
    const targetMonth = new Date(2025, 7, 1) // August 2025

    const result = calculateRecurringExpenseForMonth(nextDate, recurrence, amount, targetMonth)

    expect(result).toBe(0)
  })

  it('should calculate daily recurring expense correctly', () => {
    // Daily expense of $10 starting from Aug 29th
    const nextDate = '2025-08-29T00:00:00.000Z'
    const recurrence = RecurrenceType.Daily
    const amount = 10
    const targetMonth = new Date(2025, 7, 1) // August 2025

    const result = calculateRecurringExpenseForMonth(nextDate, recurrence, amount, targetMonth)

    // Aug 29, Aug 30, Aug 31 = 3 days
    expect(result).toBe(30)
  })

  it('should calculate bi-weekly recurring expense correctly', () => {
    // Bi-weekly expense of $200 starting Aug 1st
    const nextDate = '2025-08-01T00:00:00.000Z'
    const recurrence = RecurrenceType.BiWeekly
    const amount = 200
    const targetMonth = new Date(2025, 7, 1) // August 2025

    const result = calculateRecurringExpenseForMonth(nextDate, recurrence, amount, targetMonth)

    // Aug 1, Aug 15, Aug 29 = 3 occurrences (every 14 days)
    expect(result).toBe(600)
  })
})
