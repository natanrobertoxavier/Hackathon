namespace Client.Communication.Response;

public class ResponseClientBasicInfo
{
    public ResponseClientBasicInfo(Guid id)
    {
        Id = id;
    }

    public ResponseClientBasicInfo()
    {
    }


    public Guid Id { get; set; }
}
