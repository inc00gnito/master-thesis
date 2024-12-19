using EventBus.Messages.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Student.API.Data;
using Student.API.Models;

namespace Student.API.Consumers;
public class EnrollmentCreatedConsumer : IConsumer<EnrollmentCreatedEvent>
{
    private readonly StudentContext _context;
    private readonly ILogger<EnrollmentCreatedConsumer> _logger;

    public EnrollmentCreatedConsumer(StudentContext context, ILogger<EnrollmentCreatedConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EnrollmentCreatedEvent> context)
    {
        _logger.LogInformation("Consuming EnrollmentCreated event: StudentId {StudentId}", context.Message.StudentId);

        var student = await _context.Students.FindAsync(context.Message.StudentId);
        if (student != null)
        {
            var enrollment = new StudentEnrollment
            {
                StudentId = context.Message.StudentId,
                CourseId = context.Message.CourseId,
                EnrollmentDate = context.Message.EnrollmentDate,
                Status = EnrollmentStatus.Active
            };

            _context.StudentEnrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Added enrollment for student {StudentId} in course {CourseId}", 
                student.Id, context.Message.CourseId);
        }
    }
}

// Student.API/Consumers/EnrollmentCancelledConsumer.cs
public class EnrollmentCancelledConsumer : IConsumer<EnrollmentCancelledEvent>
{
    private readonly StudentContext _context;
    private readonly ILogger<EnrollmentCancelledConsumer> _logger;

    public EnrollmentCancelledConsumer(StudentContext context, ILogger<EnrollmentCancelledConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EnrollmentCancelledEvent> context)
    {
        _logger.LogInformation("Consuming EnrollmentCancelled event: StudentId {StudentId}", context.Message.StudentId);

        var enrollment = await _context.StudentEnrollments
            .FirstOrDefaultAsync(e => e.StudentId == context.Message.StudentId 
                                 && e.CourseId == context.Message.CourseId);

        if (enrollment != null)
        {
            enrollment.Status = EnrollmentStatus.Cancelled;
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Updated enrollment status to Cancelled for student {StudentId} in course {CourseId}", 
                context.Message.StudentId, context.Message.CourseId);
        }
    }
}