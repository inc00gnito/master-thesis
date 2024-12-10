// src/services/enrollmentService.ts
import axios from 'axios'
import type { EnrollmentDto, CreateEnrollmentDto } from '@/types/enrollment'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export const enrollmentService = {
  // Get enrollment by ID
  async getAllEnrollments(): Promise<EnrollmentDto[]> {
    const response = await axios.get(`${API_URL}/enrollment-api/enrollment/`)
    return response.data
  },

  async getEnrollment(id: number): Promise<EnrollmentDto> {
    const response = await axios.get(`${API_URL}/enrollment-api/enrollment/${id}`)
    return response.data
  },

  // Get enrollments by student
  async getEnrollmentsByStudent(studentId: number): Promise<EnrollmentDto[]> {
    const response = await axios.get(`${API_URL}/enrollment-api/enrollment/student/${studentId}`)
    return response.data
  },

  // Get enrollments by course
  async getEnrollmentsByCourse(courseId: number): Promise<EnrollmentDto[]> {
    const response = await axios.get(`${API_URL}/enrollment-api/enrollment/course/${courseId}`)
    return response.data
  },

  // Create new enrollment
  async createEnrollment(enrollment: CreateEnrollmentDto): Promise<EnrollmentDto> {
    const response = await axios.post(`${API_URL}/enrollment-api/enrollment`, enrollment)
    return response.data
  },

  // Cancel enrollment
  async cancelEnrollment(id: number): Promise<void> {
    await axios.post(`${API_URL}/enrollment-api/enrollment/${id}/cancel`)
  },
}
