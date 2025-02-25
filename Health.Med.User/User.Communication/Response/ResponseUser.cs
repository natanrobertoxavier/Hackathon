namespace User.Communication.Response;

public class ResponseUser
{
    public ResponseUser(
        Guid id, 
        DateTime registrationDate, 
        string name, 
        string email)
    {
        Id = id;
        RegistrationDate = registrationDate;
        Name = name;
        Email = email;
    }

    public ResponseUser()
    {
    }

    public Guid Id { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
