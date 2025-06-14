﻿using Notification.Communication.Request;
using Notification.Domain.Entities;

namespace Notification.Application.Mapping;

public static class MailMapping
{
    public static Mail ToEntity(this RequestSendMail request)
    {
        return new Mail(
            request.Recipients,
            request.CopyRecipients,
            request.HiddenRecipients,
            request.Subject,
            request.Body,
            request.IsHtml
        );
    }
}
