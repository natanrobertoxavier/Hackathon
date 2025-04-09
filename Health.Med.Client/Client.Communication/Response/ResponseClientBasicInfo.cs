namespace Client.Communication.Response;

public class ResponseClientBasicInfo
{
    public ResponseClientBasicInfo(
        Guid id, 
        string preferredName, 
        string email)
    {
        Id = id;
        PreferredName = preferredName;
        Email = email;
    }

    public ResponseClientBasicInfo()
    {
    }


    public Guid Id { get; set; }
    public string PreferredName { get; set; }
    public string Email { get; set; }
}
