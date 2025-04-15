namespace Doctor.Communication.Response;

public class ResponseScheduleDoctor
{
    public ResponseScheduleDoctor( 
        string name, 
        decimal consultationPrice,
        IEnumerable<ResponseServiceDay> schedule)
    {
        Schedule = schedule;
        Name = name;
        ConsultationPrice = consultationPrice;
    }

    public ResponseScheduleDoctor()
    {
    }

    public string Name { get; set; }
    public decimal ConsultationPrice { get; set; }
    public IEnumerable<ResponseServiceDay> Schedule { get; set; }
}
