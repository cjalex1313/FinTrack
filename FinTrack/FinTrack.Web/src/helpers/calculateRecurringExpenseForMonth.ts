import { RecurrenceType } from '@/models/recurrenceType'

/**
 * Calculates the total amount of a recurring expense that will occur in a given month
 * @param nextDate The next date when the expense will occur (ISO string)
 * @param recurrence The recurrence type of the expense
 * @param amount The amount of each occurrence
 * @param targetMonth The month to calculate for (Date object)
 * @returns The total amount for the month
 */
export default function calculateRecurringExpenseForMonth(
  nextDate: string,
  recurrence: RecurrenceType,
  amount: number,
  targetMonth: Date = new Date(),
): number {
  const nextDateObj = new Date(nextDate)
  const targetYear = targetMonth.getFullYear()
  const targetMonthNum = targetMonth.getMonth()

  // Get the first and last day of the target month in UTC to match the input date timezone
  const monthStart = new Date(Date.UTC(targetYear, targetMonthNum, 1))
  const monthEnd = new Date(Date.UTC(targetYear, targetMonthNum + 1, 0, 23, 59, 59, 999))

  let totalAmount = 0
  let currentDate = new Date(nextDateObj)

  // If the next date is after the target month, no occurrences
  if (currentDate > monthEnd) {
    return 0
  }

  // Find the first occurrence in the target month
  while (currentDate < monthStart) {
    currentDate = getNextOccurrence(currentDate, recurrence)
  }

  // Count all occurrences within the target month
  while (currentDate <= monthEnd) {
    totalAmount += amount
    currentDate = getNextOccurrence(currentDate, recurrence)
  }

  return totalAmount
}

function getNextOccurrence(date: Date, recurrence: RecurrenceType): Date {
  const nextDate = new Date(date)

  switch (recurrence) {
    case RecurrenceType.Daily:
      nextDate.setDate(nextDate.getDate() + 1)
      break
    case RecurrenceType.Weekly:
      nextDate.setDate(nextDate.getDate() + 7)
      break
    case RecurrenceType.BiWeekly:
      nextDate.setDate(nextDate.getDate() + 14)
      break
    case RecurrenceType.Monthly:
      nextDate.setMonth(nextDate.getMonth() + 1)
      break
    case RecurrenceType.Quarterly:
      nextDate.setMonth(nextDate.getMonth() + 3)
      break
    case RecurrenceType.Yearly:
      nextDate.setFullYear(nextDate.getFullYear() + 1)
      break
  }

  return nextDate
}

export { getNextOccurrence }
