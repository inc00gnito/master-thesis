<template>
  <div class="app-container">
    <header>
      <!-- <img alt="Vue logo" class="logo" src="@/assets/logo.svg" width="80" height="80" /> -->
      <h1>Learning Management System</h1>
      <nav>
        <RouterLink to="/courses">Courses</RouterLink>
        <RouterLink to="/students">Students</RouterLink>
        <RouterLink to="/enrollments">Enrollments</RouterLink>
      </nav>
    </header>
    <notifications />
    <main>
      <RouterView />
    </main>
  </div>
</template>

<style scoped>
/* ... existing styles ... */

nav {
  display: flex;
  gap: 20px;
  margin-left: auto;
}

nav a {
  text-decoration: none;
  color: var(--color-text);
  padding: 8px 16px;
  border-radius: 4px;
}

nav a:hover {
  background-color: rgba(0, 0, 0, 0.05);
}

nav a.router-link-active {
  background-color: #4caf50;
  color: white;
}
</style>
