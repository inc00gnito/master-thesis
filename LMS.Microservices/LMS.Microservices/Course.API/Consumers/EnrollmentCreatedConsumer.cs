using Course.API.Data;
using EventBus.Messages.Events;
using MassTransit;

namespace Course.API.Consumers;
public class EnrollmentCreatedConsumer : IConsumer<EnrollmentCreatedEvent>
{
    private readonly CourseContext _context;
    private readonly ILogger<EnrollmentCreatedConsumer> _logger;

    public EnrollmentCreatedConsumer(CourseContext context, ILogger<EnrollmentCreatedConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EnrollmentCreatedEvent> context)
    {
        _logger.LogInformation("Consuming EnrollmentCreated event: CourseId {CourseId}", context.Message.CourseId);
        
        var course = await _context.Courses.FindAsync(context.Message.CourseId);
        if (course != null)
        {
            course.CurrentEnrollment++;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated enrollment count for course {CourseId} to {Count}", 
                course.Id, course.CurrentEnrollment);
        }
    }
}
