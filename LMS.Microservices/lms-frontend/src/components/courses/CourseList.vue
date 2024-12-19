<!-- src/components/courses/CourseList.vue -->
<template>
  <div class="course-list">
    <h2>Courses</h2>
    <div v-if="loading">Loading...</div>
    <div v-else-if="error">{{ error }}</div>
    <div v-else class="courses-grid">
      <div v-for="course in courses" :key="course.id" class="course-card">
        <h3>{{ course.title }}</h3>
        <p>{{ course.description }}</p>
        <div class="enrollment-info">
          <span>{{ course.currentEnrollment }}/{{ course.maxEnrollment }} enrolled</span>
        </div>
        <button @click="viewDetails(course.id)">View Details</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCourseStore } from '@/stores/courseStore'
import { storeToRefs } from 'pinia'

const router = useRouter()
const courseStore = useCourseStore()

const { courses, loading, error } = storeToRefs(courseStore)

onMounted(() => {
  courseStore.fetchCourses()
})

const viewDetails = (id: number) => {
  router.push(`/courses/${id}`)
}
</script>

<style scoped>
.course-list {
  padding: 20px;
}

.courses-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
  margin-top: 20px;
}

.course-card {
  border: 1px solid #ddd;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.enrollment-info {
  margin: 10px 0;
  color: #666;
}

button {
  background-color: #4caf50;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
}

button:hover {
  background-color: #45a049;
}
</style>
