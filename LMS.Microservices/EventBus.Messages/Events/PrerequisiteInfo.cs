// EventBus.Messages/Events/CourseCreatedEvent.cs
namespace EventBus.Messages.Events
{
    public class PrerequisiteInfo
    {
        public int PrerequisiteCourseId { get; set; }
        public bool IsMandatory { get; set; }
    }
}
