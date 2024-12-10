```mermaid
graph TB
Client[Frontend Vue.js Application]

    subgraph "Monolithic Backend"
        API[API Layer]
        AppLayer[Application Layer]
        Domain[Domain Layer]
        DataLayer[Infrastructure Layer]
    end

    Database[(Single Database)]

    Client --> API
    API --> AppLayer
    AppLayer --> Domain
    AppLayer --> DataLayer
    DataLayer --> Database

    subgraph "Application Services"
        CourseService[Course Service]
        StudentService[Student Service]
        EnrollmentService[Enrollment Service]
    end

    AppLayer --> CourseService
    AppLayer --> StudentService
    AppLayer --> EnrollmentService
```

Data Flow (enrollment process)

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant EnrollmentService
    participant CourseService
    participant StudentService
    participant Database

    Client->>API: Create Enrollment Request
    API->>EnrollmentService: Process Enrollment
    EnrollmentService->>CourseService: Check Prerequisites
    CourseService-->>EnrollmentService: Prerequisites Status
    EnrollmentService->>StudentService: Verify Student
    StudentService-->>EnrollmentService: Student Status
    EnrollmentService->>Database: Create Enrollment
    EnrollmentService->>CourseService: Update Course Capacity
    CourseService->>Database: Update Course
    EnrollmentService-->>API: Enrollment Created
    API-->>Client: Success Response
```

Deployment arch

```mermaid
graph TB
    subgraph "Container Orchestration"
        subgraph "Frontend Instances"
            FE1[Vue Frontend-1]
            FE2[Vue Frontend-2]
        end

        subgraph "Backend Instances"
            BE1[Monolith Instance-1]
            BE2[Monolith Instance-2]
        end
    end

    subgraph "Database"
        DB1[(Primary DB)]
        DB2[(Replica DB)]
    end

    LB[Load Balancer]
    CDN[CDN]

    Internet[Internet] --> CDN
    CDN --> LB
    LB --> FE1 & FE2
    FE1 & FE2 --> BE1 & BE2
    BE1 & BE2 --> DB1
    DB1 --> DB2
```

Scalling Scenarios:

```mermaid
graph TB
    subgraph "Normal Load"
        FE1[Frontend x1]
        BE1[Backend Instance x2]
    end

    subgraph "Medium Load"
        FE2[Frontend x2]
        BE2[Backend Instance x4]
    end

    subgraph "High Load"
        FE3[Frontend x3]
        BE3[Backend Instance x6]
    end

    NL[Normal Load] --> FE1
    ML[Medium Load] --> FE2
    HL[High Load] --> FE3
```