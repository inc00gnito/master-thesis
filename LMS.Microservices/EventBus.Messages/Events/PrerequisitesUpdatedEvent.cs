// EventBus.Messages/Events/CourseCreatedEvent.cs
namespace EventBus.Messages.Events
{
    // EventBus.Messages/Events/PrerequisitesUpdatedEvent.cs
    public class PrerequisitesUpdatedEvent
    {
        public int CourseId { get; set; }
        public List<PrerequisiteInfo> Prerequisites { get; set; }
    }
}
