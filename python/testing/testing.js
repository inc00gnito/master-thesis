import http from "k6/http";
import { sleep, check } from "k6";
import { Trend, Rate } from "k6/metrics";

// Define endpoints
const ENDPOINTS = {
  courses: `http://localhost:5110/api/course`,
  students: "http://localhost:5110/api/student",
  enrollments: `http://localhost:5110/api/enrollment`,
  coursePrerequisites: (id) => `http://localhost:5110/api/${id}/prerequisites`,
};

const studentsPayloads = JSON.parse(open("student_data.json"));
const coursesPayloads = JSON.parse(open("course_data.json"));
const coursePrerequsitesPayloads = JSON.parse(open("prerequisites.json"));
const enrollmentsPayloads = JSON.parse(open("enrollment_data.json"));

// Define metrics
let postResponseTime = new Trend("POST_Response_Time");
let getResponseTime = new Trend("GET_Response_Time");
let postRequestCount = new Trend("POST_Request_Count");
let getRequestCount = new Trend("GET_Request_Count");
let postErrorRate = new Rate("POST_Error_Rate");
let getErrorRate = new Rate("GET_Error_Rate");
let dataReceived = new Trend("Data_Received");
let successRate = new Rate("Success_Rate");
// Sequential counters
let studentIndex = 0;
let courseIndex = 0;
let enrollmentIndex = 0;
let prerequisiteIndex = 0;
let postLimit = 2000;

// Load testing configuration
export let options = {
  stages: [
    { duration: "2m", target: 2 }, // Ramp-up to 100 VUs over 1 minute
    { duration: "4m", target: 2 }, // Steady load for 3 minutes
    { duration: "40s", target: 0 }, // Ramp-down over 30 seconds
  ],
};

export default function () {
  // POST students
  if (studentIndex < postLimit) {
    const studentPayload =
      studentsPayloads[studentIndex % studentsPayloads.length];
    let studentRes = http.post(
      ENDPOINTS.students,
      JSON.stringify(studentPayload),
      {
        headers: { "Content-Type": "application/json" },
      }
    );
    postRequestCount.add(1);
    postResponseTime.add(studentRes.timings.duration);
    postErrorRate.add(studentRes.status !== 201);
    successRate.add(studentRes.status === 201);
    studentIndex++;
  }
  // POST courses
  if (courseIndex < postLimit) {
    const coursePayload = coursesPayloads[courseIndex % coursesPayloads.length];
    let courseRes = http.post(
      ENDPOINTS.courses,
      JSON.stringify(coursePayload),
      {
        headers: { "Content-Type": "application/json" },
      }
    );
    postRequestCount.add(1);
    postResponseTime.add(courseRes.timings.duration);
    postErrorRate.add(courseRes.status !== 201);
    successRate.add(courseRes.status === 201);
    courseIndex++;
  }
  // POST course prerequisites
  if (prerequisiteIndex < postLimit) {
    const prerequisitePayload =
      coursePrerequsitesPayloads[
        prerequisiteIndex % coursePrerequsitesPayloads.length
      ];
    console.log(prerequisitePayload);
    let prerequisiteRes = http.post(
      ENDPOINTS.coursePrerequisites(prerequisitePayload.CourseId),
      JSON.stringify({
        prerequisiteCourseId: prerequisitePayload.PrerequisiteCourseId,
        prerequisiteCourseName: prerequisitePayload.PrerequisiteCourseName,
        isMandatory: prerequisitePayload.IsMandatory,
      }),
      { headers: { "Content-Type": "application/json" } }
    );
    postRequestCount.add(1);
    postResponseTime.add(prerequisiteRes.timings.duration);
    postErrorRate.add(prerequisiteRes.status !== 201);
    successRate.add(prerequisiteRes.status === 201);
    prerequisiteIndex++;
  }

  // POST enrollments
  if (enrollmentIndex < postLimit) {
    const enrollmentPayload =
      enrollmentsPayloads[enrollmentIndex % enrollmentsPayloads.length];
    let enrollmentRes = http.post(
      ENDPOINTS.enrollments,
      JSON.stringify(enrollmentPayload),
      { headers: { "Content-Type": "application/json" } }
    );
    postRequestCount.add(1);
    postResponseTime.add(enrollmentRes.timings.duration);
    postErrorRate.add(enrollmentRes.status !== 201);
    successRate.add(enrollmentRes.status === 201);
    enrollmentIndex++;
  }

  // GET individual requests
  let randomStudentID = Math.floor(Math.random() * studentIndex) + 1;
  let randomCourseID = Math.floor(Math.random() * courseIndex) + 1;

  let getStudentsRes = http.get(`${ENDPOINTS.students}/${randomStudentID}`);
  getRequestCount.add(1);
  getResponseTime.add(getStudentsRes.timings.duration);
  getErrorRate.add(getStudentsRes.status !== 200);
  successRate.add(getStudentsRes.status === 200);

  let getCoursesRes = http.get(`${ENDPOINTS.courses}/${randomCourseID}`);
  getRequestCount.add(1);
  getResponseTime.add(getCoursesRes.timings.duration);
  getErrorRate.add(getCoursesRes.status !== 200);
  successRate.add(getCoursesRes.status === 200);

  let getEnrollmentsRes = http.get(
    `${ENDPOINTS.enrollments}?student_id=${randomStudentID}`
  );
  getRequestCount.add(1);
  getResponseTime.add(getEnrollmentsRes.timings.duration);
  getErrorRate.add(getEnrollmentsRes.status !== 200);
  successRate.add(getEnrollmentsRes.status === 200);

  // GET all data
  let getAllStudents = http.get(ENDPOINTS.students);
  getRequestCount.add(1);
  dataReceived.add(getAllStudents.body.length);
  successRate.add(getAllStudents.status === 200);

  let getAllCourses = http.get(ENDPOINTS.courses);
  getRequestCount.add(1);
  dataReceived.add(getAllCourses.body.length);
  successRate.add(getAllCourses.status === 200);

  let getAllEnrollments = http.get(ENDPOINTS.enrollments);
  getRequestCount.add(1);
  dataReceived.add(getAllEnrollments.body.length);
  successRate.add(getAllEnrollments.status === 200);

  sleep(1); // Wait 1 second between iterations
}
