namespace Exceptions
{
    // Course.API/Exceptions/NotFoundException.cs
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }

    // Course.API/Exceptions/InvalidOperationException.cs
    public class PrerequisiteAlreadyExistsException : Exception
    {
        public PrerequisiteAlreadyExistsException(string message) : base(message)
        {
        }
    }

    // Course.API/Exceptions/ServiceUnavailableException.cs
    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException(string message) : base(message)
        {
        }
    }

    // Course.API/Exceptions/PrerequisitesNotMetException.cs
    public class PrerequisitesNotMetException : Exception
    {
        public PrerequisitesNotMetException(string message) : base(message)
        {
        }
    }
}
