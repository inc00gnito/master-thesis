<!-- src/components/courses/CourseForm.vue -->
<template>
  <div class="course-form">
    <h2>Create New Course</h2>
    <form @submit.prevent="handleSubmit">
      <div class="form-group">
        <label for="title">Title</label>
        <input id="title" v-model="form.title" type="text" required />
      </div>

      <div class="form-group">
        <label for="description">Description</label>
        <textarea id="description" v-model="form.description" required></textarea>
      </div>

      <div class="form-group">
        <label for="maxEnrollment">Maximum Enrollment</label>
        <input
          id="maxEnrollment"
          v-model.number="form.maxEnrollment"
          type="number"
          min="1"
          required
        />
      </div>

      <button type="submit" :disabled="loading">
        {{ loading ? 'Creating...' : 'Create Course' }}
      </button>
    </form>
    <!-- Notification Component -->
    <notifications />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useCourseStore } from '@/stores/courseStore'
import { useNotification } from '@kyvg/vue3-notification'

const router = useRouter()
const courseStore = useCourseStore()
const { notify } = useNotification()

const loading = ref(false)
const form = ref({
  title: '',
  description: '',
  maxEnrollment: 30,
})

const handleSubmit = async () => {
  loading.value = true
  try {
    await courseStore.createCourse(form.value)

    // Show success notification
    notify({
      title: 'Success',
      text: 'Course created successfully!',
      type: 'success',
    })

    // Reset form
    form.value = {
      title: '',
      description: '',
      maxEnrollment: 30,
    }

    // Navigate to courses page
    router.push('/courses')
  } catch (err) {
    // Show error notification
    notify({
      title: 'Error',
      text: 'Failed to create course',
      type: 'error',
    })
    console.error('Error creating course:', err)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.course-form {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
}

.form-group {
  margin-bottom: 20px;
}

label {
  display: block;
  margin-bottom: 5px;
}

input,
textarea {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

button {
  background-color: #4caf50;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 4px;
  cursor: pointer;
}

button:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}
</style>
