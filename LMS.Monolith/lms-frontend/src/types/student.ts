import type { StudentEnrollment } from './enrollment'

export interface Student {
  id: number
  firstName: string
  lastName: string
  email: string
  enrollments?: StudentEnrollment[]
}

export interface CreateStudentDto {
  firstName: string
  lastName: string
  email: string
}

export interface UpdateStudentDto {
  firstName?: string
  lastName?: string
  email?: string
}
