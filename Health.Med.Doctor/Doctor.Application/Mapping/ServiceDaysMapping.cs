using Doctor.Communication.Request;
using Doctor.Communication.Response;
using System.Globalization;

namespace Doctor.Application.Mapping;

public static class ServiceDaysMapping
{
    public static IEnumerable<Domain.Entities.ServiceDay> ToListEntity(this RequestServiceDay requestList, Guid doctorId)
    {
        return requestList.ServiceDays.Select(request =>
            new Domain.Entities.ServiceDay(
                doctorId,
                CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetDayName(request.Day),
                request.StartTime,
                request.EndTime)).ToList();
    }

    public static IEnumerable<string> ToDeleteEntity(this RequestDeleteServiceDay requestList)
    {
        return requestList.Days.Select(request =>
                CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetDayName(request.Day)).ToList();
    }

    public static ResponseServiceDay ToRespose(this Domain.Entities.ServiceDay serviceDay)
    {
        return new ResponseServiceDay(
            serviceDay.Day,
            serviceDay.StartTime,
            serviceDay.EndTime);
    }
}
