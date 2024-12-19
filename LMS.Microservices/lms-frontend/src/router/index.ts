// src/router/router.ts
import { createRouter, createWebHistory } from 'vue-router'
import CourseView from '../views/CourseView.vue'
import StudentView from '../views/StudentView.vue'
import EnrollmentView from '@/views/EnrollmentView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/courses',
    },
    {
      path: '/courses',
      name: 'courses',
      component: CourseView,
    },
    {
      path: '/students',
      name: 'students',
      component: StudentView,
    },
    {
      path: '/enrollments',
      name: 'enrollments',
      component: EnrollmentView,
    },
  ],
})

export default router
