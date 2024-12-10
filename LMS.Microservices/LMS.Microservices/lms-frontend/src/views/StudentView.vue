<!-- eslint-disable @typescript-eslint/no-unused-vars -->
<template>
  <div class="student-container">
    <div class="header-actions">
      <h1>Student Management</h1>
      <button @click="showAddModal = true" class="primary-button">Add New Student</button>
    </div>

    <!-- Search and Filter Section
    <div class="search-section">
      <input
        v-model="searchQuery"
        placeholder="Search students..."
        class="search-input"
        @input="handleSearch"
      />
    </div> -->

    <!-- Main Content Grid -->
    <div class="content-grid">
      <!-- Students List -->
      <div class="students-list">
        <div v-if="loading" class="loading-state">Loading students...</div>
        <div v-else-if="error" class="error-state">
          {{ error }}
        </div>
        <div v-else>
          <div
            v-for="student in filteredStudents"
            :key="student.id"
            class="student-card"
            :class="{ active: selectedStudent?.id === student.id }"
            @click="selectStudent(student)"
          >
            <div class="student-info">
              <h3>{{ student.firstName }} {{ student.lastName }}</h3>
              <p>{{ student.email }}</p>
            </div>
            <div class="student-actions">
              <button @click.stop="editStudent(student)" class="edit-button">Edit</button>
              <button @click.stop="confirmDelete(student)" class="delete-button">Delete</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Student Details -->
      <div v-if="selectedStudent" class="student-details">
        <div class="details-header">
          <h2>Student Details</h2>
          <button @click="clearSelection" class="secondary-button">Close</button>
        </div>

        <div class="details-content">
          <div class="personal-info">
            <h3>Personal Information</h3>
            <p>
              <strong>Name:</strong> {{ selectedStudent.firstName }} {{ selectedStudent.lastName }}
            </p>
            <p><strong>Email:</strong> {{ selectedStudent.email }}</p>
          </div>

          <!-- Enrollments Section -->
          <div class="enrollments">
            <h3>Course Enrollments</h3>
            <div v-if="loadingEnrollments" class="loading-state">Loading enrollments...</div>
            <div v-else-if="studentEnrollments.length === 0" class="empty-state">
              No enrollments found
            </div>
            <div v-else class="enrollments-list">
              <div
                v-for="enrollment in studentEnrollments"
                :key="enrollment.id"
                class="enrollment-item"
              >
                <div class="enrollment-info">
                  <h4>{{ enrollment.courseName }}</h4>
                  <p>Enrolled: {{ formatDate(enrollment.enrollmentDate) }}</p>
                  <!-- <span class="status-badge" :class="enrollment.status.toLowerCase()"> -->
                  {{ enrollment.status }}
                  <!-- </span> -->
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Add/Edit Student Modal -->
    <div v-if="showAddModal || showEditModal" class="modal">
      <div class="modal-content">
        <h2>{{ showEditModal ? 'Edit' : 'Add' }} Student</h2>
        <form @submit.prevent="handleSubmit">
          <div class="form-group">
            <label for="firstName">First Name</label>
            <input
              id="firstName"
              v-model="studentForm.firstName"
              required
              placeholder="Enter first name"
            />
          </div>

          <div class="form-group">
            <label for="lastName">Last Name</label>
            <input
              id="lastName"
              v-model="studentForm.lastName"
              required
              placeholder="Enter last name"
            />
          </div>

          <div class="form-group">
            <label for="email">Email</label>
            <input
              id="email"
              v-model="studentForm.email"
              type="email"
              required
              placeholder="Enter email"
            />
          </div>

          <div class="modal-actions">
            <button type="button" @click="closeModal" class="secondary-button">Cancel</button>
            <button type="submit" class="primary-button">
              {{ showEditModal ? 'Update' : 'Create' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal" class="modal">
      <div class="modal-content">
        <h2>Confirm Delete</h2>
        <p>Are you sure you want to delete this student?</p>
        <div class="modal-actions">
          <button @click="showDeleteModal = false" class="secondary-button">Cancel</button>
          <button @click="deleteStudent" class="delete-button">Delete</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useNotification } from '@kyvg/vue3-notification'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import type { Student, CreateStudentDto, UpdateStudentDto } from '@/types/student'
import type { StudentEnrollment } from '@/types/enrollment'
import { studentService } from '@/services/studentService'
import { courseService } from '@/services/courseService'
// import type { Course } from '@/types/course'

const notification = useNotification()

// State
const loading = ref(false)
const loadingEnrollments = ref(false)
const error = ref<string | null>(null)
const students = ref<Student[]>([])
const selectedStudent = ref<Student | null>(null)
const studentEnrollments = ref<StudentEnrollment[]>([])
const searchQuery = ref('')
const showAddModal = ref(false)
const showEditModal = ref(false)
const showDeleteModal = ref(false)
const studentToDelete = ref<Student | null>(null)
// const studentCourses = ref<Course[]>([])

// Form state
const studentForm = ref<CreateStudentDto>({
  firstName: '',
  lastName: '',
  email: '',
})

// Computed
const filteredStudents = computed(() => {
  if (!searchQuery.value) return students.value

  const query = searchQuery.value.toLowerCase()
  return students.value.filter(
    (student) =>
      student.firstName.toLowerCase().includes(query) ||
      student.lastName.toLowerCase().includes(query) ||
      student.email.toLowerCase().includes(query),
  )
})

// Methods
const fetchStudents = async () => {
  console.log('fetching students....')
  loading.value = true
  try {
    students.value = await studentService.getAllStudents()
    console.log('all,', students.value)
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
  } catch (err) {
    error.value = 'Failed to fetch students'
    notification.notify({
      type: 'error',
      text: 'Failed to load students',
    })
  } finally {
    loading.value = false
  }
}

async function fetchEnrollmentsWithCourses(studentId: number) {
  try {
    loadingEnrollments.value = true
    const enrollments = await studentService.getEnrollments(studentId)
    const enrollmentsWithCourses = []

    for (const enrollment of enrollments) {
      const course = await courseService.getCourseById(enrollment.courseId)
      enrollmentsWithCourses.push({ ...enrollment, courseName: course.title })
    }

    studentEnrollments.value = enrollmentsWithCourses
    console.log('Enrollments with courses', enrollmentsWithCourses)
    return enrollmentsWithCourses
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
  } catch (err) {
    notification.notify({
      type: 'error',
      text: 'Failed to load enrollments and courses',
    })
    return []
  } finally {
    loadingEnrollments.value = false
  }
}

const selectStudent = async (student: Student) => {
  selectedStudent.value = student
  await fetchEnrollmentsWithCourses(student.id)
}

const handleSubmit = async () => {
  try {
    if (showEditModal.value && selectedStudent.value) {
      await studentService.updateStudent(selectedStudent.value.id, studentForm.value)
      notification.notify({
        type: 'success',
        text: 'Student updated successfully',
      })
    } else {
      await studentService.createStudent(studentForm.value)
      notification.notify({
        type: 'success',
        text: 'Student created successfully',
      })
    }
    closeModal()
    await fetchStudents()
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
  } catch (err) {
    notification.notify({
      type: 'error',
      text: showEditModal.value ? 'Failed to update student' : 'Failed to create student',
    })
  }
}

const editStudent = (student: Student) => {
  studentForm.value = {
    firstName: student.firstName,
    lastName: student.lastName,
    email: student.email,
  }
  showEditModal.value = true
}

const confirmDelete = (student: Student) => {
  studentToDelete.value = student
  showDeleteModal.value = true
}

const deleteStudent = async () => {
  if (!studentToDelete.value) return

  try {
    await studentService.deleteStudent(studentToDelete.value.id)
    notification.notify({
      type: 'success',
      text: 'Student deleted successfully',
    })
    if (selectedStudent.value?.id === studentToDelete.value.id) {
      selectedStudent.value = null
    }
    showDeleteModal.value = false
    await fetchStudents()
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
  } catch (err) {
    notification.notify({
      type: 'error',
      text: 'Failed to delete student',
    })
  }
}

const closeModal = () => {
  showAddModal.value = false
  showEditModal.value = false
  studentForm.value = {
    firstName: '',
    lastName: '',
    email: '',
  }
}

const clearSelection = () => {
  selectedStudent.value = null
  studentEnrollments.value = []
}

const formatDate = (date: string | Date) => {
  return new Date(date).toLocaleDateString()
}

// Lifecycle
onMounted(fetchStudents)
</script>

<style scoped>
.student-container {
  padding: 20px;
}

.header-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.search-section {
  margin-bottom: 20px;
}

.search-input {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.content-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
}

.student-card {
  padding: 15px;
  border: 1px solid #ddd;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.student-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.student-card.active {
  border-color: #4caf50;
  background-color: #f0f9f0;
}

.student-info h3 {
  margin: 0;
  color: #333;
}

.student-info p {
  margin: 5px 0;
  color: #666;
}

.student-actions {
  display: flex;
  gap: 10px;
  margin-top: 10px;
}

.student-details {
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 8px;
}

.details-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.enrollments-list {
  display: grid;
  gap: 10px;
}

.enrollment-item {
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.status-badge {
  display: inline-block;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.8em;
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

.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
}

.modal-content {
  background-color: white;
  padding: 20px;
  border-radius: 8px;
  min-width: 300px;
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
}

.form-group input {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
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

.delete-button {
  background-color: #f44336;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
}

.edit-button {
  background-color: #2196f3;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
}

.loading-state,
.error-state,
.empty-state {
  text-align: center;
  padding: 20px;
  color: #666;
}
</style>
