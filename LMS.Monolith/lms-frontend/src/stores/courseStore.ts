// src/stores/courseStore.ts
import { defineStore } from 'pinia'
import { courseService } from '@/services/courseService'
import type { Course } from '@/types/course'

export const useCourseStore = defineStore('course', {
  state: () => ({
    courses: [] as Course[],
    loading: false,
    error: null as string | null,
  }),

  actions: {
    async fetchCourses() {
      this.loading = true
      try {
        this.courses = await courseService.getAllCourses()
      } catch (error) {
        this.error = 'Failed to fetch courses'
        console.error(error)
      } finally {
        this.loading = false
      }
    },

    async createCourse(course: Omit<Course, 'id' | 'currentEnrollment'>) {
      try {
        const newCourse = await courseService.createCourse(course)
        this.courses.push(newCourse)
        return newCourse
      } catch (error) {
        this.error = 'Failed to create course'
        throw error
      }
    },
  },
})
