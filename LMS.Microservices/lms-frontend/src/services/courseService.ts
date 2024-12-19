// src/services/courseService.ts
import type { Course, CreateCourseDto, Prerequisite, CreatePrerequisiteDto } from '@/types/course'
import axios from 'axios'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export const courseService = {
  async getAllCourses(): Promise<Course[]> {
    const response = await axios.get(`${API_URL}/course-api/course`)
    return response.data
  },

  async getCourseById(id: number): Promise<Course> {
    const response = await axios.get(`${API_URL}/course-api/course/${id}`)
    return response.data
  },

  async createCourse(course: CreateCourseDto): Promise<Course> {
    const response = await axios.post(`${API_URL}/course-api/course`, course)
    return response.data
  },

  async getPrerequisites(courseId: number): Promise<Prerequisite[]> {
    const response = await axios.get(`${API_URL}/course-api/course/${courseId}/prerequisites`)
    return response.data
  },

  async addPrerequisite(
    courseId: number,
    prerequisite: CreatePrerequisiteDto,
  ): Promise<Prerequisite> {
    const response = await axios.post(
      `${API_URL}/course-api/course/${courseId}/prerequisites`,
      prerequisite,
    )
    return response.data
  },

  async removePrerequisite(courseId: number, prerequisiteId: number): Promise<void> {
    await axios.delete(`${API_URL}/course-api/course/${courseId}/prerequisites/${prerequisiteId}`)
  },
}
