using System.Runtime.Serialization;

namespace Health.Med.Exceptions.ExceptionBase;

[Serializable]
public class ValidationErrorsException : HealthMedException
{
    public List<string> ErrorMessages { get; set; } = [];
    public ValidationErrorsException(List<string> errorMessages) : base(string.Empty)
    {
        ErrorMessages = errorMessages;
    }

    protected ValidationErrorsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
