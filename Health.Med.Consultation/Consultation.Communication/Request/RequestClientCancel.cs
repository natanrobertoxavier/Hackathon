namespace Consultation.Communication.Request;


public class RequestClientCancel
{
    public RequestClientCancel(
        Guid pin, 
        string key, 
        string reason)
    {
        Pin = pin;
        Key = key;
        Reason = reason;
    }

    public RequestClientCancel()
    {
    }

    public Guid Pin { get; set; }
    public string Key { get; set; }
    public string Reason { get; set; }
}
