<!-- eslint-disable @typescript-eslint/no-unused-vars -->
<!-- eslint-disable @typescript-eslint/no-unused-vars -->
<!-- src/views/CourseView.vue -->
<template>
  <div class="course-container">
    <!-- Course List Section -->
    <div class="course-list-section">
      <h2>Courses</h2>
      <button @click="showAddCourse = true" class="add-button">Add New Course</button>

      <!-- Course List -->
      <div v-if="loading" class="loading">Loading courses...</div>
      <div v-else-if="error" class="error">{{ error }}</div>
      <div v-else class="course-grid">
        <div
          v-for="course in courses"
          :key="course.id"
          class="course-card"
          :class="{ selected: selectedCourse?.id === course.id }"
          @click="selectCourse(course)"
        >
          <h3>{{ course.title }}</h3>
          <p class="description">{{ course.description }}</p>
          <div class="enrollment-info">
            <span>Enrolled: {{ course.currentEnrollment }}/{{ course.maxEnrollment }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Course Details Section -->
    <div v-if="selectedCourse" class="course-details-section">
      <h2>Course Details</h2>
      <div class="details-card">
        <h3>{{ selectedCourse.title }}</h3>
        <p>{{ selectedCourse.description }}</p>
        <div class="enrollment-stats">
          <p>Maximum Enrollment: {{ selectedCourse.maxEnrollment }}</p>
          <p>Current Enrollment: {{ selectedCourse.currentEnrollment }}</p>
        </div>

        <!-- Prerequisites Section -->
        <div class="prerequisites-section">
          <h4>Prerequisites</h4>
          <div v-if="loadingPrerequisites" class="loading">Loading prerequisites...</div>
          <div v-else>
            <div v-if="prerequisites.length === 0" class="no-prerequisites">
              No prerequisites set
            </div>
            <ul v-else class="prerequisites-list">
              <li v-for="prereq in prerequisites" :key="prereq.prerequisiteCourseId">
                {{ prereq.prerequisiteCourseName }}
                <span class="prerequisite-type">{{
                  prereq.isMandatory ? '(Required)' : '(Optional)'
                }}</span>
                <button @click="removePrerequisite(prereq)" class="remove-button">Remove</button>
              </li>
            </ul>
            <button @click="showAddPrerequisite = true" class="add-prerequisite-button">
              Add Prerequisite
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Add Course Modal -->
    <div v-if="showAddCourse" class="modal">
      <div class="modal-content">
        <h3>Add New Course</h3>
        <form @submit.prevent="createCourse">
          <div class="form-group">
            <label for="title">Title:</label>
            <input v-model="newCourse.title" id="title" required />
          </div>
          <div class="form-group">
            <label for="description">Description:</label>
            <textarea v-model="newCourse.description" id="description" required></textarea>
          </div>
          <div class="form-group">
            <label for="maxEnrollment">Maximum Enrollment:</label>
            <input
              v-model.number="newCourse.maxEnrollment"
              type="number"
              id="maxEnrollment"
              required
            />
          </div>
          <div class="button-group">
            <button type="submit">Create</button>
            <button type="button" @click="showAddCourse = false">Cancel</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Add Prerequisite Modal -->
    <div v-if="showAddPrerequisite" class="modal">
      <div class="modal-content">
        <h3>Add Prerequisite</h3>
        <form @submit.prevent="addPrerequisite">
          <div class="form-group">
            <label for="prerequisiteCourse">Prerequisite Course:</label>
            <select v-model="newPrerequisite.prerequisiteCourseId" id="prerequisiteCourse" required>
              <option v-for="course in availablePrerequisites" :key="course.id" :value="course.id">
                {{ course.title }}
              </option>
            </select>
          </div>
          <div class="form-group">
            <label>
              <input type="checkbox" v-model="newPrerequisite.isMandatory" />
              Is Mandatory
            </label>
          </div>
          <div class="button-group">
            <button type="submit">Add</button>
            <button type="button" @click="showAddPrerequisite = false">Cancel</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import type { Course, Prerequisite, CreateCourseDto, CreatePrerequisiteDto } from '@/types/course'
import { courseService } from '@/services/courseService'
import { useNotification } from '@kyvg/vue3-notification'

const loading = ref(false)
const loadingPrerequisites = ref(false)
const error = ref<string | null>(null)
const showAddCourse = ref(false)
const showAddPrerequisite = ref(false)
const selectedCourse = ref<Course | null>(null)
const prerequisites = ref<Prerequisite[]>([])
const courses = ref<Course[]>([])
const notification = useNotification()

// Form state with proper types
const newCourse = ref<CreateCourseDto>({
  title: '',
  description: '',
  maxEnrollment: 30,
})

const newPrerequisite = ref<CreatePrerequisiteDto>({
  prerequisiteCourseId: 0,
  isMandatory: true,
})

// Computed with proper types
const availablePrerequisites = computed(() => {
  if (!selectedCourse.value) return [] as Course[]
  return courses.value.filter(
    (course) =>
      course.id !== selectedCourse.value?.id &&
      !prerequisites.value.some((p) => p.prerequisiteCourseId === course.id),
  )
})

// Add this with your other functions
const selectCourse = async (course: Course): Promise<void> => {
  selectedCourse.value = course
  await fetchPrerequisites(course.id)
}
// Replace all other toast calls in the code:
const fetchCourses = async (): Promise<void> => {
  loading.value = true
  try {
    courses.value = await courseService.getAllCourses()
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
  } catch (err) {
    error.value = 'Failed to fetch courses'
    notification.notify({
      type: 'error',
      text: 'Failed to load courses',
    })
  } finally {
    loading.value = false
  }
}

const fetchPrerequisites = async (courseId: number): Promise<void> => {
  loadingPrerequisites.value = true
  try {
    prerequisites.value = await courseService.getPrerequisites(courseId)
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
  } catch (err) {
    notification.notify({
      type: 'error',
      text: 'Failed to load prerequisites',
    })
  } finally {
    loadingPrerequisites.value = false
  }
}

const createCourse = async (): Promise<void> => {
  try {
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    const created = await courseService.createCourse(newCourse.value)
    notification.notify({
      type: 'success',
      text: 'Course created successfully',
    })
    showAddCourse.value = false
    await fetchCourses()
    newCourse.value = { title: '', description: '', maxEnrollment: 30 }
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
  } catch (err) {
    notification.notify({
      type: 'error',
      text: 'Failed to create course',
    })
  }
}

const addPrerequisite = async (): Promise<void> => {
  if (!selectedCourse.value || !newPrerequisite.value.prerequisiteCourseId) return

  try {
    const prerequisiteCourse = courses.value.find(
      (c) => c.id === newPrerequisite.value.prerequisiteCourseId,
    )

    // Create the prerequisite with course name
    const prerequisiteToAdd = {
      ...newPrerequisite.value,
      prerequisiteCourseName: prerequisiteCourse?.title || '',
    }
    await courseService.addPrerequisite(selectedCourse.value.id, prerequisiteToAdd)
    notification.notify({
      type: 'success',
      text: 'Prerequisite added successfully',
    })
    showAddPrerequisite.value = false
    await fetchPrerequisites(selectedCourse.value.id)
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
  } catch (err) {
    notification.notify({
      type: 'error',
      text: 'Failed to add prerequisite',
    })
  }
}

const removePrerequisite = async (prerequisite: Prerequisite): Promise<void> => {
  if (!selectedCourse.value) return

  try {
    await courseService.removePrerequisite(
      selectedCourse.value.id,
      prerequisite.prerequisiteCourseId,
    )
    notification.notify({
      type: 'success',
      text: 'Prerequisite removed successfully',
    })
    await fetchPrerequisites(selectedCourse.value.id)
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
  } catch (err) {
    notification.notify({
      type: 'error',
      text: 'Failed to remove prerequisite',
    })
  }
}
defineExpose({
  selectCourse,
  fetchCourses,
  fetchPrerequisites,
  createCourse,
  addPrerequisite,
  removePrerequisite,
})
// Lifecycle
onMounted(fetchCourses)
</script>

<style scoped>
.course-container {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
  padding: 20px;
}

.course-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 20px;
  margin-top: 20px;
}

.course-card {
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 15px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.course-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.course-card.selected {
  border-color: #4caf50;
  background-color: #f0f9f0;
}

.details-card {
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 20px;
}

.prerequisites-section {
  margin-top: 20px;
  padding-top: 20px;
  border-top: 1px solid #eee;
}

.prerequisites-list {
  list-style: none;
  padding: 0;
}

.prerequisites-list li {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 0;
  border-bottom: 1px solid #eee;
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

.form-group input,
.form-group textarea,
.form-group select {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.button-group {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
}

button {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  background-color: #4caf50;
  color: white;
}

button:hover {
  background-color: #45a049;
}

.add-button {
  margin-bottom: 20px;
}

.loading,
.error {
  text-align: center;
  padding: 20px;
}

.prerequisite-type {
  color: #666;
  font-size: 0.9em;
}

.remove-button {
  background-color: #ff4444;
}

.remove-button:hover {
  background-color: #cc0000;
}
</style>
