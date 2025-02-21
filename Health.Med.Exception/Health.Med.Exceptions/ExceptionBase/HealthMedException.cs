using System.Runtime.Serialization;

namespace Health.Med.Exceptions.ExceptionBase;

public class HealthMedException : SystemException
{
    public HealthMedException(string mensagem) : base(mensagem)
    {
    }

    protected HealthMedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}