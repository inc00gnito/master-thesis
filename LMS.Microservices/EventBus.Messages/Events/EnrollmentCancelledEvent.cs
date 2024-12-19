namespace EventBus.Messages.Events
{
    public class EnrollmentCancelledEvent
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
    }
}