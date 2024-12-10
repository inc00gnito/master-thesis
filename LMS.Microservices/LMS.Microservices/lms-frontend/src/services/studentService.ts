// src/services/studentService.ts
import axios from 'axios'
import type { Student, CreateStudentDto } from '@/types/student'
import type { StudentEnrollment } from '@/types/enrollment'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export const studentService = {
  async getAllStudents(): Promise<Student[]> {
    const response = await axios.get(`${API_URL}/student-api/student`)
    return response.data
  },

  async getStudentById(id: number): Promise<Student> {
    const response = await axios.get(`${API_URL}/student-api/student/${id}`)
    return response.data
  },

  async createStudent(student: CreateStudentDto): Promise<Student> {
    const response = await axios.post(`${API_URL}/student-api/student`, student)
    return response.data
  },

  async updateStudent(id: number, student: Partial<CreateStudentDto>): Promise<Student> {
    const response = await axios.put(`${API_URL}/student-api/student/${id}`, student)
    return response.data
  },

  async deleteStudent(id: number): Promise<void> {
    await axios.delete(`${API_URL}/student-api/student/${id}`)
  },

  async getEnrollments(studentId: number): Promise<StudentEnrollment[]> {
    const response = await axios.get(`${API_URL}/student-api/student/${studentId}/enrollments`)
    return response.data
  },
}
