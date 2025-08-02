import { RecurrenceType } from '@/models/recurrenceType'

export default function getMonthlyEquivalent(recurrence: RecurrenceType, amount: number): number {
  switch (recurrence) {
    case RecurrenceType.Daily:
      return amount * 30.4 // Approximate month
    case RecurrenceType.Weekly:
      return amount * 4.345 // 52 weeks / 12 months
    case RecurrenceType.BiWeekly:
      return amount * 2.1725 // 26 bi-weekly periods / 12
    case RecurrenceType.Monthly:
      return amount
    case RecurrenceType.Quarterly:
      return amount / 3
    case RecurrenceType.Yearly:
      return amount / 12
    default:
      return amount
  }
}
