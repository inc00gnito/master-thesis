﻿namespace EventBus.Messages.Events
{
    public class EnrollmentCreatedEvent
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}