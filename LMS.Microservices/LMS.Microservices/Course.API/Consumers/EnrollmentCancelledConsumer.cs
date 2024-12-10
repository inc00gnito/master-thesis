
// Course.API/Consumers/EnrollmentCancelledConsumer.cs
using Course.API.Data;
using EventBus.Messages.Events;
using MassTransit;

namespace Course.API.Consumers;

public class EnrollmentCancelledConsumer : IConsumer<EnrollmentCancelledEvent>
{
    private readonly CourseContext _context;
    private readonly ILogger<EnrollmentCancelledConsumer> _logger;

    public EnrollmentCancelledConsumer(CourseContext context, ILogger<EnrollmentCancelledConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EnrollmentCancelledEvent> context)
    {
        _logger.LogInformation("Consuming EnrollmentCancelled event: CourseId {CourseId}", context.Message.CourseId);

        var course = await _context.Courses.FindAsync(context.Message.CourseId);
        if (course != null && course.CurrentEnrollment > 0)
        {
            course.CurrentEnrollment--;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Decreased enrollment count for course {CourseId} to {Count}", 
                course.Id, course.CurrentEnrollment);
        }
    }
}