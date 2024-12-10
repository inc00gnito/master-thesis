// EventBus.Messages/Events/CourseCreatedEvent.cs
namespace EventBus.Messages.Events
{
    public class CourseCreatedEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxEnrollment { get; set; }
    }
}
