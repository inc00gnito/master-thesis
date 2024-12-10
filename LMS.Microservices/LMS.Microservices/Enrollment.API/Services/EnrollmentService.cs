using AutoMapper;
using Enrollment.API.Data;
using Enrollment.API.DTOs;
using Enrollment.API.Models;
using EventBus.Messages.Events;
using Exceptions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Enrollment.API.Services;
public class EnrollmentService : IEnrollmentService
{
    private readonly EnrollmentContext _context;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EnrollmentService> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public EnrollmentService(
        EnrollmentContext context,
        IMapper mapper,
        IPublishEndpoint publishEndpoint,
        ILogger<EnrollmentService> logger,
        IHttpClientFactory clientFactory)
    {
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public async Task<EnrollmentDto> CreateEnrollmentAsync(CreateEnrollmentDto dto)
    {
        _logger.LogInformation("Creating enrollment for student {StudentId} in course {CourseId}",
            dto.StudentId, dto.CourseId);
        // 1. Check prerequisites via HTTP (need immediate response)
        var prerequisitesMet = await CheckPrerequisites(dto.StudentId, dto.CourseId);
        if (!prerequisitesMet)
            throw new PrerequisitesNotMetException("Not all prerequisites are met");

        // 2. Create enrollment
        var enrollment = new Models.Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            EnrollmentDate = DateTime.UtcNow,
            Status = EnrollmentStatus.Pending
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Creating enrollment for student {StudentId} in course {CourseId}",
            enrollment.StudentId, enrollment.CourseId);
        await _publishEndpoint.Publish(new EnrollmentCreatedEvent
        {
            Id = enrollment.Id,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            EnrollmentDate = enrollment.EnrollmentDate
        });
        _logger.LogInformation("Published EnrollmentCreated event for enrollment {EnrollmentId}", enrollment.Id);

        return _mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByStudentAsync(int studentId)
    {
        var enrollments = await _context.Enrollments
            .Where(e => e.StudentId == studentId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
    }

    public async Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByCourseAsync(int courseId)
    {
        var enrollments = await _context.Enrollments
            .Where(e => e.CourseId == courseId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
    }

    public async Task<EnrollmentDto> GetEnrollmentAsync(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        return _mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task<bool> CancelEnrollmentAsync(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);
        if (enrollment == null) return false;

        enrollment.Status = EnrollmentStatus.Cancelled;
        await _context.SaveChangesAsync();

        await _publishEndpoint.Publish(new EnrollmentCancelledEvent
        {
            Id = enrollment.Id,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId
        });

        return true;
    }
    private async Task<bool> CheckPrerequisites(int studentId, int courseId)
    {
        try
        {
            // Get prerequisites from Course.API
            var courseClient = _clientFactory.CreateClient("CourseAPI");
            var prerequisitesResponse = await courseClient
                .GetAsync($"/api/course/{courseId}/prerequisites");

            if (!prerequisitesResponse.IsSuccessStatusCode)
                throw new ServiceUnavailableException("Could not get prerequisites");

            var prerequisites = await prerequisitesResponse.Content
                .ReadFromJsonAsync<List<PrerequisiteDto>>();

            // Get completed courses from Student.API
            var studentClient = _clientFactory.CreateClient("StudentAPI");
            var completedCoursesResponse = await studentClient
                .GetAsync($"/api/student/{studentId}/completed-courses");

            if (!completedCoursesResponse.IsSuccessStatusCode)
                throw new ServiceUnavailableException("Could not get completed courses");

            var completedCourses = await completedCoursesResponse.Content
                .ReadFromJsonAsync<List<int>>();

            // Check if all mandatory prerequisites are met
            return prerequisites
                .Where(p => p.IsMandatory)
                .All(p => completedCourses.Contains(p.PrerequisiteCourseId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking prerequisites");
            throw;
        }
    }

    public async Task<IEnumerable<EnrollmentDto>> GetAllEnrollmentsAsync()
    {
        var enrollments = await _context.Enrollments.ToListAsync();

        return _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
    }
}

public class PrerequisiteDto
{
    public int CourseId { get; set; }
    public int PrerequisiteCourseId { get; set; }
    public string PrerequisiteCourseName { get; set; }
    public bool IsMandatory { get; set; }
}