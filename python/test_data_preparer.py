import json
import random
from faker import Faker
from datetime import datetime, timedelta
import re
import pandas as pd

fake = Faker()

def generate_course_data(file_name="course_data.json", total=100):
    """
    Generate a JSON file with unique course titles and required fields.
    
    Parameters:
        file_name (str): Name of the output JSON file.
        total (int): Number of courses to generate.
    """
    # Base course titles and levels to ensure unique names
    base_titles = [
    # Mathematics and Sciences
    "Algebra", "Geometry", "Calculus", "Physics", "Trigonometry", "Statistics", 
    "Linear Algebra", "Differential Equations", "Discrete Mathematics", 
    "Applied Mathematics", "Quantum Physics", "Thermodynamics", "Mechanics", 
    "Organic Chemistry", "Inorganic Chemistry", "Biochemistry", 
    "Molecular Biology", "Cell Biology", "Genetics", "Ecology", "Environmental Science", 
    "Astronomy", "Astrophysics", "Geology", "Earth Science", 
    
    # Computer Science and Programming
    "Programming", "Data Structures", "Algorithms", "Operating Systems", 
    "Computer Networks", "Database Management", "Software Engineering", 
    "Artificial Intelligence", "Machine Learning", "Deep Learning", 
    "Cybersecurity", "Cloud Computing", "Internet of Things (IoT)", "Blockchain Technology", 
    "Game Development", "Mobile App Development", "Web Development", 
    "Data Analysis", "Data Science", "Natural Language Processing", 
    "Computer Vision", "Parallel Computing", "Compiler Design", "Big Data Technologies", 
    "Distributed Systems", "Software Testing and QA", 
    
    # Business and Economics
    "Principles of Economics", "Microeconomics", "Macroeconomics", 
    "Business Management 101", "Marketing Strategies", "Financial Accounting", 
    "Corporate Finance", "Investment Strategies", "International Business", 
    "Entrepreneurship 101", "Supply Chain Management", "Operations Management", 
    "Human Resource Management", "Project Management", "Risk Management", 
    "Business Ethics", "Organizational Behavior", "E-commerce Strategies", 
    "Digital Marketing", "Business Analytics", "Financial Modeling", 
    "Cost Accounting", "Behavioral Economics", "International Finance", 

    # Humanities and Arts
    "Art History", "Music Theory", "Introduction to Philosophy", "Ethics and Morality", 
    "Critical Thinking", "Creative Writing", "Literary Analysis", 
    "Modern Art and Culture", "Ancient Civilizations", "Medieval History", 
    "Renaissance Studies", "World History", "Cultural Anthropology", 
    "Sociology 101", "Psychology 101", "Cognitive Psychology", "Social Psychology", 
    "Political Science", "International Relations", "Public Speaking", "Linguistics", 
    "Theatre and Performance", "Film Studies", "Photography Basics", 
    "Introduction to Journalism", "Storytelling Techniques", 

    # Health and Wellness
    "Nutrition and Dietetics", "Health and Fitness", "Human Anatomy", 
    "Physiology 101", "Medical Terminology", "Public Health", "Epidemiology", 
    "Sports Science", "First Aid and Safety", "Mental Health Awareness", 
    "Yoga and Meditation", "Physical Education", "Kinesiology", 

    # Miscellaneous and Emerging Fields
    "Ethical Hacking", "Robotics", "Augmented Reality (AR)", "Virtual Reality (VR)", 
    "Drone Technology", "3D Printing and Modeling", "Sustainable Energy Solutions", 
    "Climate Change Studies", "Urban Planning", "Hospitality Management", 
    "Tourism and Travel Management", "Fashion Design", "Interior Design", 
    "Culinary Arts", "Baking and Pastry", "Personal Finance Management", 
    "Digital Literacy", "Time Management Strategies", "Leadership Skills", 
    "Conflict Resolution", "Team Building Strategies", "Negotiation Skills"
]
    levels = [f"{i}" for i in range(1, 100)]  # Levels 1 to 20
    
    # Generate unique course names
    unique_titles = [f"{base} {level}" for base in base_titles for level in levels][:total]

    # Generate course data
    data = []
    for idx, title in enumerate(unique_titles, start=0):
        course = {
            "Id":idx,
            "Title": title,
            "Description": fake.paragraph(nb_sentences=2),
            "MaxEnrollment": random.randint(10, 30),
            "CurrentEnrollment": 0, 
            "CreatedDate": fake.date_time_between(start_date="-2y", end_date="now").isoformat()
        }
        data.append(course)

    # Save to JSON file
    with open(file_name, "w") as f:
        json.dump(data, f, indent=4)
    print(f"JSON file '{file_name}' with {idx+1} courses has been created successfully!")

def generate_student_data(file_name="student_data.json", total=100):
    unique_names = set()  # To ensure unique (FirstName, LastName) pairs
    data = []

    while len(data) < total:
        first_name = fake.first_name()
        last_name = fake.last_name()
        
        # Ensure the name is unique
        if (first_name, last_name) not in unique_names:
            unique_names.add((first_name, last_name))
            email = f"{first_name.lower()}.{last_name.lower()}@example.com"
            
            student = {
                "Id": len(data) + 1,
                "FirstName": first_name,
                "LastName": last_name,
                "Email": email,
                "DateOfBirth": fake.date_of_birth(minimum_age=18, maximum_age=25).isoformat(),
                "CreatedDate": fake.date_time_between(start_date="-2y", end_date="now").isoformat(),
                "UpdatedDate": None  
            }
            data.append(student)

    # Save to a JSON file
    with open(file_name, "w") as f:
        json.dump(data, f, indent=4)
    print(f"JSON file '{file_name}' with {total} unique student records has been created successfully!")

def generate_prerequisite_relationships(input_file="course_data.json", output_file="course_prerequisites.json"):
    """
    Generate stepwise prerequisite relationships for courses grouped by their base names.
    The first course in a group has PrerequisiteCourseId = 0.

    Parameters:
        input_file (str): Input JSON file with course data.
        output_file (str): Output JSON file with prerequisites.
    """
    # Load the course data
    with open(input_file, "r") as f:
        courses = json.load(f)

    prerequisites = []

    # Group courses by base name (e.g., Algebra, Geometry)
    course_groups = {}
    for course in courses:
        base_name = re.match(r"^[A-Za-z\s]+", course["Title"]).group().strip()  # Extract base name
        if base_name not in course_groups:
            course_groups[base_name] = []
        course_groups[base_name].append(course)

    # Sort each group by Id and create prerequisites within the group
    id_counter = 0
    for group in course_groups.values():
        group.sort(key=lambda x: x["Id"])  # Ensure courses are ordered by their Id
        # Create sequential prerequisites for the rest of the group
        for i in range(1, len(group)):
            current_course = group[i]
            previous_course = group[i - 1]

            prerequisite_entry = {
                "Id": id_counter,
                "CourseId": current_course["Id"],
                "PrerequisiteCourseId": previous_course["Id"],
                "IsMandatory": True,
                "PrerequisiteCourseName": previous_course["Title"]
            }
            prerequisites.append(prerequisite_entry)
            id_counter += 1

    # Save the generated prerequisites to a new JSON file
    with open(output_file, "w") as f:
        json.dump(prerequisites, f, indent=4)

    print(f"Prerequisite relationships saved to '{output_file}'.")


def generate_enrollments(students_file="student_data.json", courses_file="course_data.json", output_file="enrollment_data.json", min=3, max=6):
    """
    Generate enrollments for students and courses.
    Each student will be enrolled in random courses with a random EnrollmentDate.

    Parameters:
        students_file (str): Input JSON file with student data.
        courses_file (str): Input JSON file with course data.
        output_file (str): Output JSON file with generated enrollments.
    """
    # Load students and courses data
    with open(students_file, "r") as f:
        students = json.load(f)
    with open(courses_file, "r") as f:
        courses = json.load(f)

    enrollments = []
    enrollment_id = 1

    # Generate enrollments
    for student in students:
        num_courses = random.randint(min,max)  # Each student takes 3-6 courses
        selected_courses = random.sample(courses, num_courses)
        for course in selected_courses:
            enrollment = {
                "Id": enrollment_id,
                "StudentId": student["Id"],
                "CourseId": course["Id"],
                "EnrollmentDate": (datetime.now() - timedelta(days=random.randint(30, 365))).strftime("%Y-%m-%dT%H:%M:%S")
            }
            enrollments.append(enrollment)
            enrollment_id += 1

    # Save enrollments to file
    with open(output_file, "w") as f:
        json.dump(enrollments, f, indent=4)

    print(f"Enrollments saved to '{output_file}'.")

def visulize_enrollments():
    fp="enrollment_data.json"
    with open(fp, 'r') as file:
        data=json.load(file)
    
    df = pd.DataFrame(data)

    grouped_by_course = df.groupby('CourseId').size().reset_index(name='Enrollments')
    grouped_by_student = df.groupby('StudentId').size().reset_index(name='Enrollments')

    # # Display grouped data
    # print("Grouped by CourseId:")
    # print(grouped_by_course)

    # print("\nGrouped by StudentId:")
    # print(grouped_by_student)
    # Call the function to generate data
    # grouped_by_course.to_csv('grouped_by_course.csv', index=False)
    # grouped_by_student.to_csv('grouped_by_student.csv', index=False)


if __name__ == "__main__":
    total = 10000
    generate_course_data(total=total)
    generate_student_data(total=total)
    generate_prerequisite_relationships()
    generate_enrollments()
    visulize_enrollments()