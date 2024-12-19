<!-- src/views/EnrollmentView.vue -->
<template>
  <div class="enrollment-container">
    <!-- Header -->
    <div class="header-section">
      <h2>Enrollments</h2>
      <button @click="showCreateModal = true" class="primary-button">Create Enrollment</button>
    </div>

    <!-- Stats or Summary -->
    <div class="summary-section">
      <div class="stat-card">
        <h4>Total Enrollments</h4>
        <p>{{ enrollments.length }}</p>
      </div>
      <div class="stat-card">
        <h4>Active Enrollments</h4>
        <p>{{ activeEnrollments }}</p>
      </div>
    </div>

    <!-- Filters -->
    <div class="filters-section">
      <div class="filter-group">
        <label>Student:</label>
        <select v-model="selectedStudentFilter" @change="handleStudentFilterChange">
          <option value="">All Students</option>
          <option v-for="student in students" :key="student.id" :value="student.id">
            {{ student.firstName }} {{ student.lastName }}
          </option>
        </select>
      </div>

      <div class="filter-group">
        <label>Course:</label>
        <select v-model="selectedCourseFilter" @change="handleCourseFilterChange">
          <option value="">All Courses</option>
          <option v-for="course in courses" :key="course.id" :value="course.id">
            {{ course.title }}
          </option>
        </select>
      </div>

      <div class="filter-group">
        <label>Status:</label>
        <select v-model="statusFilter">
          <option value="">All Statuses</option>
          <option v-for="status in enrollmentStatuses" :key="status" :value="status">
            {{ status }}
          </option>
        </select>
      </div>
    </div>

    <!-- Enrollments Table -->
    <div class="table-container">
      <div v-if="loading" class="loading-state">
        <p>Loading enrollments...</p>
      </div>
      <table v-else class="data-table">
        <thead>
          <tr>
            <th>Student</th>
            <th>Course</th>
            <th>Enrollment Date</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="enrollment in filteredEnrollments" :key="enrollment.id">
            <td>{{ getStudentName(enrollment.studentId) }}</td>
            <td>{{ getCourseName(enrollment.courseId) }}</td>
            <td>{{ formatDate(enrollment.enrollmentDate) }}</td>
            <td>
              <!-- <span :class="['status-badge', enrollment.status.toLowerCase()]"> -->
              {{ enrollment.status }}
              <!-- </span> -->
            </td>
            <td class="actions-cell">
              <button
                v-if="enrollment.status !== 'Cancelled'"
                @click="cancelEnrollment(enrollment.id)"
                class="cancel-button"
              >
                Cancel
              </button>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="!loading && filteredEnrollments.length === 0" class="empty-state">
        No enrollments found
      </div>
    </div>

    <!-- Create Enrollment Modal -->
    <div v-if="showCreateModal" class="modal">
      <div class="modal-content">
        <div class="modal-header">
          <h3>Create New Enrollment</h3>
          <button @click="closeCreateModal" class="close-button">&times;</button>
        </div>

        <form @submit.prevent="createEnrollment" class="enrollment-form">
          <div class="form-group">
            <label for="studentSelect">Student:</label>
            <select
              id="studentSelect"
              v-model="newEnrollment.studentId"
              required
              class="form-select"
            >
              <option value="">Select a student</option>
              <option v-for="student in students" :key="student.id" :value="student.id">
                {{ student.firstName }} {{ student.lastName }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label for="courseSelect">Course:</label>
            <select id="courseSelect" v-model="newEnrollment.courseId" required class="form-select">
              <option value="">Select a course</option>
              <option v-for="course in availableCourses" :key="course.id" :value="course.id">
                {{ course.title }} ({{ course.currentEnrollment }}/{{ course.maxEnrollment }})
              </option>
            </select>
          </div>

          <div class="form-actions">
            <button type="button" @click="closeCreateModal" class="secondary-button">Cancel</button>
            <button type="submit" class="primary-button" :disabled="!isFormValid">
              Create Enrollment
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import type { Student } from '@/types/student'
import type { Course } from '@/types/course'
import type { EnrollmentDto, CreateEnrollmentDto } from '@/types/enrollment'
import { enrollmentService } from '@/services/enrollmentService'
import { studentService } from '@/services/studentService'
import { courseService } from '@/services/courseService'
import { useNotification } from '@kyvg/vue3-notification'

// State
const loading = ref(false)
const showCreateModal = ref(false)
const enrollments = ref<EnrollmentDto[]>([])
const students = ref<Student[]>([])
const courses = ref<Course[]>([])

// Filters
const selectedStudentFilter = ref('')
const selectedCourseFilter = ref('')
const statusFilter = ref('')

// Form
const newEnrollment = ref<CreateEnrollmentDto>({
  studentId: 0,
  courseId: 0,
})

const notification = useNotification()

// Computed
const enrollmentStatuses = ['Active', 'Pending', 'Completed', 'Cancelled']

const activeEnrollments = computed(
  () => enrollments.value.filter((e) => e.status === 'Active').length,
)

const availableCourses = computed(() =>
  courses.value.filter((course) => course.currentEnrollment < course.maxEnrollment),
)

const filteredEnrollments = computed(() => {
  let filtered = enrollments.value

  if (selectedStudentFilter.value) {
    filtered = filtered.filter((e) => e.studentId === Number(selectedStudentFilter.value))
  }

  if (selectedCourseFilter.value) {
    filtered = filtered.filter((e) => e.courseId === Number(selectedCourseFilter.value))
  }

  if (statusFilter.value) {
    filtered = filtered.filter((e) => e.status === statusFilter.value)
  }

  return filtered
})

const isFormValid = computed(() => newEnrollment.value.studentId && newEnrollment.value.courseId)

// Methods
const fetchData = async () => {
  loading.value = true
  try {
    const [enrollmentData, studentData, courseData] = await Promise.all([
      enrollmentService.getAllEnrollments(),
      studentService.getAllStudents(),
      courseService.getAllCourses(),
    ])
    // console.log('enmr', enrollment.value)
    console.log('students', studentData)
    console.log('courses', courseData)
    enrollments.value = enrollmentData
    students.value = studentData
    courses.value = courseData
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
  } catch (error) {
    notification.notify({
      type: 'error',
      text: 'Failed to fetch data',
    })
  } finally {
    loading.value = false
  }
}

const getStudentName = (studentId: number) => {
  const student = students.value.find((s) => s.id === studentId)
  return student ? `${student.firstName} ${student.lastName}` : 'Unknown'
}

const getCourseName = (courseId: number) => {
  const course = courses.value.find((c) => c.id === courseId)
  return course ? course.title : 'Unknown'
}

const createEnrollment = async () => {
  try {
    await enrollmentService.createEnrollment(newEnrollment.value)
    notification.notify({
      type: 'success',
      text: 'Enrollment created successfully',
    })
    closeCreateModal()
    await fetchData()
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
  } catch (error) {
    notification.notify({
      type: 'error',
      text: 'Failed to create enrollment',
    })
  }
}

const cancelEnrollment = async (id: number) => {
  try {
    await enrollmentService.cancelEnrollment(id)
    notification.notify({
      type: 'success',
      text: 'Enrollment cancelled successfully',
    })
    await fetchData()
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
  } catch (error) {
    notification.notify({
      type: 'error',
      text: 'Failed to cancel enrollment',
    })
  }
}

const closeCreateModal = () => {
  showCreateModal.value = false
  newEnrollment.value = { studentId: 0, courseId: 0 }
}

const handleStudentFilterChange = async () => {
  if (selectedStudentFilter.value) {
    const studentEnrollments = await enrollmentService.getEnrollmentsByStudent(
      Number(selectedStudentFilter.value),
    )
    enrollments.value = studentEnrollments
  } else {
    await fetchData()
  }
}

const handleCourseFilterChange = async () => {
  if (selectedCourseFilter.value) {
    const courseEnrollments = await enrollmentService.getEnrollmentsByCourse(
      Number(selectedCourseFilter.value),
    )
    enrollments.value = courseEnrollments
  } else {
    await fetchData()
  }
}

const formatDate = (date: string | Date) => {
  return new Date(date).toLocaleDateString()
}

// Lifecycle
onMounted(fetchData)
</script>

<style scoped>
.enrollment-container {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.header-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.summary-section {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 24px;
}

.stat-card {
  padding: 16px;
  background: rgb(94, 94, 94);
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.filters-section {
  display: flex;
  gap: 16px;
  margin-bottom: 24px;
  flex-wrap: wrap;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 4px;
  min-width: 200px;
}

.table-container {
  background: rgb(94, 94, 94);
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table th,
.data-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.status-badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.85em;
}

.status-badge.active {
  background-color: #4caf50;
  color: white;
}

.status-badge.pending {
  background-color: #ffc107;
  color: black;
}

.status-badge.cancelled {
  background-color: #f44336;
  color: white;
}

.status-badge.completed {
  background-color: #2196f3;
  color: white;
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
}

.modal-content {
  background: white;
  padding: 24px;
  border-radius: 8px;
  min-width: 400px;
  max-width: 90%;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.enrollment-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.form-select {
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  margin-top: 16px;
}

.primary-button {
  background-color: #4caf50;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
}

.secondary-button {
  background-color: #9e9e9e;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
}

.cancel-button {
  background-color: #f44336;
  color: white;
  border: none;
  padding: 4px 8px;
  border-radius: 4px;
  cursor: pointer;
}

.loading-state,
.empty-state {
  padding: 40px;
  text-align: center;
  color: #d1d1d1;
}

.close-button {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: #d1d1d1;
}

button:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}
</style>
