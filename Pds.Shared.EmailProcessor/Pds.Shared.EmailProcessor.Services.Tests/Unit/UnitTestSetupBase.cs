using Pds.Admin.Api.Client.Models;
using Pds.Shared.EmailProcessor.Services.Models;
using System;
using System.Collections.Generic;

namespace Pds.Shared.EmailProcessor.Services.Tests.Unit
{
    public class UnitTestSetupBase
    {
        public static readonly SendNotificationResponse FailedResponseWithRateException =
            new SendNotificationResponse
            {
                RateLimitException = true,
                Success = false,
                ErrorMessage = Constants.ServicesConstants.GovUkNotifyRateLimitException
            };

        public static readonly SendNotificationResponse FailedResponse =
            new SendNotificationResponse
            {
                Success = false
            };

        public static readonly SendNotificationResponse FailedWithExceptionResponse =
            new SendNotificationResponse
            {
                Success = false,
                ErrorMessage = nameof(Exception).ToLowerInvariant()
            };

        public static readonly SendNotificationResponse SuccessResponse =
            new SendNotificationResponse
            {
                Success = true
            };

        public static IEnumerable<object[]> GetEmailTemplateDetailsAsyncInput()
        {
            yield return new object[]
            {
                new EmailTemplateRequest
                {
                    RequestingService = "vyf",
                    EmailMessageType = "matNewFunding"
                },
                new NotifyTemplateDetails
                {
                    TemplateId = "5ba6066e-f36f-4d42-a855-01588d99f2f6",
                    NotifyApiKeySecretName = "secret1"
                },
                new EmailTemplateResponse
                {
                    Success = true,
                    EmailTemplateId = "5ba6066e-f36f-4d42-a855-01588d99f2f6",
                    NotifyApiKeySecretName = "secret1"
                }
            };
            yield return new object[]
            {
                new EmailTemplateRequest
                {
                    RequestingService = "non existent",
                    EmailMessageType = "non existent"
                },
                null,
                new EmailTemplateResponse()
            };

            yield return new object[]
            {
                new EmailTemplateRequest
                {
                    RequestingService = "bad request",
                    EmailMessageType = "bad request"
                },
                null,
                new EmailTemplateResponse
                {
                    ErrorMessage = "bad request"
                },
                true
            };
        }

        public static IEnumerable<object[]> SendNotificationInput()
        {
            yield return new object[]
            {
                "1",
                false,
                false,
                SuccessResponse
            };
            yield return new object[]
            {
                "1",
                true,
                true,
                FailedResponseWithRateException
            };
            yield return new object[]
            {
                null,
                true,
                false,
                FailedWithExceptionResponse
            };
            yield return new object[]
            {
                null,
                false,
                false,
                FailedResponse
            };
        }
    }
}