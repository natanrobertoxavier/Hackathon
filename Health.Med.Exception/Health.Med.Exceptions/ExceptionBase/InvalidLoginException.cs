using System.Runtime.Serialization;

namespace Health.Med.Exceptions.ExceptionBase;

[Serializable]
public class InvalidLoginException : HealthMedException
{
    public InvalidLoginException() : base(ErrorsMessages.InvalidLogin)
    {
    }

    protected InvalidLoginException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
