export interface CreateEnrollmentDto {
  studentId: number
  courseId: number
}

export interface StudentEnrollment {
  id: number
  courseId: number
  courseName: string
  enrollmentDate: Date
  status: EnrollmentStatus
}

export enum EnrollmentStatus {
  Pending = 'Pending',
  Active = 'Active',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
}

export interface Enrollment {
  id: number
  studentId: number
  courseId: number
  studentName: string
  courseName: string
  enrollmentDate: Date
  status: EnrollmentStatus
  prerequisites?: PrerequisiteStatus[]
}
export interface EnrollmentDto {
  id: number
  studentId: number
  courseId: number
  enrollmentDate: Date
  status: EnrollmentStatus
}

export interface CreateEnrollmentDto {
  studentId: number
  courseId: number
}

export interface PrerequisiteStatus {
  prerequisiteId: number
  courseName: string
  isMet: boolean
  isMandatory: boolean
}

export interface EnrollmentValidationResult {
  isValid: boolean
  message?: string
  missingPrerequisites?: PrerequisiteStatus[]
}
