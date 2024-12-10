// src/types/course.ts
export interface Course {
  id: number
  title: string
  description: string
  maxEnrollment: number
  currentEnrollment: number
}

export interface CreateCourseDto {
  title: string
  description: string
  maxEnrollment: number
}

export interface Prerequisite {
  courseId: number
  prerequisiteCourseId: number
  prerequisiteCourseName: string
  isMandatory: boolean
}

export interface CreatePrerequisiteDto {
  prerequisiteCourseId: number
  isMandatory: boolean
}
