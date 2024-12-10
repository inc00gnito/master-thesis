namespace Student.API.Data
{
    public class DataSeeder
    {
        private readonly StudentContext _context;

        public DataSeeder(StudentContext context)
        {
            _context = context;
        }

        public async Task SeedData()
        {
            if (!_context.Students.Any())
            {
                var students = new List<Models.Student>
            {
                new Models.Student
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com"
                },
                new Models.Student
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com"
                },
                new Models.Student
                {
                    FirstName = "Michael",
                    LastName = "Johnson",
                    Email = "michael.johnson@example.com"
                },
                new Models.Student
                {
                    FirstName = "Sarah",
                    LastName = "Williams",
                    Email = "sarah.williams@example.com"
                },
                new Models.Student
                {
                    FirstName = "Robert",
                    LastName = "Brown",
                    Email = "robert.brown@example.com"
                },
                new Models.Student
                {
                    FirstName = "Emily",
                    LastName = "Davis",
                    Email = "emily.davis@example.com"
                }
            };

                await _context.Students.AddRangeAsync(students);
                await _context.SaveChangesAsync();
            }
        }
    }
}
