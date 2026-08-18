using Newtonsoft.Json;
using Pds.Core.Notification.Models;
using Pds.Shared.EmailProcessor.Services.Models;
using System.Collections.Generic;

namespace Pds.Shared.EmailProcessor.Func.Tests.Unit
{
    public class UnitTestSetupBase
    {
        private const string TestEmailAddress = "test@mail.com";
        private const string TestEmailAddressDuplicate = "test@mail.com";
        private const string TestEmailAddress2 = "test2@mail.com";

        public static IEnumerable<object[]> RunInput()
        {
            yield return new object[]
            {
                new NotificationMessage(),
                new EmailTemplateResponse(),
                0,
                new SendNotificationResponse(),
                1,
                new NotificationAuditEntry
                {
                    SendNotificationResponse = new SendNotificationResponse(),
                    NotificationMessage = JsonConvert.SerializeObject(new NotificationMessage()),
                    NotificationErrors = GetNotificationErrors(true),
                    RowKey = "NotificationMessage"
                }
            };

            yield return new object[]
            {
                GetNotificationMessage(),
                GetEmailTemplateResponse(true, string.Empty),
                1,
                GetApiSendNotificationResponse(),
                0,
                new NotificationAuditEntry()
            };
        }

        public static IEnumerable<object[]> SendEmailNotificationInput()
        {
            yield return new object[]
            {
                GetNotificationMessage(),
                GetEmailTemplateResponse(true, string.Empty),
                1,
                GetApiSendNotificationResponse(),
                GetAggregateSendNotificationResponse()
            };

            yield return new object[]
            {
                GetNotificationMessage(),
                GetEmailTemplateResponse(true, string.Empty),
                1,
                GetApiSendNotificationResponse(false),
                GetAggregateSendNotificationResponse(false)
            };

            yield return new object[]
            {
                GetNotificationMessageMultipleEmails(),
                GetEmailTemplateResponse(true, string.Empty),
                2,
                GetApiSendNotificationResponse(),
                GetAggregateSendNotificationResponse()
            };
        }

        //NotificationMessage notificationMessage,
        //    EmailTemplateResponse emailTemplateApiResponse,
        //int emailTemplateServiceInvocationCount,
        //    EmailTemplateResponse emailTemplateResponse
        public static IEnumerable<object[]> ProcessEmailTemplateResponse()
        {
            yield return new object[]
            {
                GetNotificationMessage(),
                GetEmailTemplateApiResponse(true, string.Empty),
                1,
                GetEmailTemplateResponse(),
            };

            yield return new object[]
            {
                GetNotificationMessage(),
                GetEmailTemplateApiResponse(false, "admin api down"),
                1,
                GetEmailTemplateResponse(false, "admin api down"),
            };
        }

        public static NotificationMessage GetNotificationMessage()
        {
            return new NotificationMessage
            {
                EmailAddresses = new List<string> { TestEmailAddress },
                EmailMessageType = nameof(NotificationMessage.EmailMessageType),
                RequestingService = nameof(NotificationMessage.RequestingService)
            };
        }

        public static NotificationMessage GetNotificationMessageMultipleEmails()
        {
            return new NotificationMessage
            {
                EmailAddresses = new List<string> { TestEmailAddress, TestEmailAddress2, TestEmailAddressDuplicate }
            };
        }

        private static EmailTemplateResponse GetEmailTemplateResponse(bool valid = true, string errorMessage = "")
        {
            return new EmailTemplateResponse
            {
                Success = valid,
                EmailTemplateId = valid ? "id" : string.Empty,
                NotifyApiKeySecretName = valid ? "myesf" : string.Empty,
                ErrorMessage = errorMessage
            };
        }

        private static EmailTemplateResponse GetEmailTemplateApiResponse(bool valid, string errorMessage)
        {
            return new EmailTemplateResponse
            {
                Success = valid,
                EmailTemplateId = valid ? "id" : string.Empty,
                NotifyApiKeySecretName = valid ? "myesf" : string.Empty,
                ErrorMessage = errorMessage
            };
        }

        private static SendNotificationResponse GetApiSendNotificationResponse(
            bool valid = true,
            string emailAddressErrorMessage = "")
        {
            return new SendNotificationResponse
            {
                EmailAddress = TestEmailAddress,
                Success = valid,
                ErrorMessage = emailAddressErrorMessage
            };
        }

        private static SendNotificationResponse GetAggregateSendNotificationResponse(
            bool valid = true,
            string emailAddressErrorMessage = "")
        {
            var errorMessages = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(emailAddressErrorMessage))
            {
                errorMessages.Add(TestEmailAddress, emailAddressErrorMessage);
            }

            return new SendNotificationResponse
            {
                Success = valid,
                EmailErrorMessages = errorMessages
            };
        }

        private static Dictionary<string, string> GetNotificationErrors(
            bool missingEmailAddress = false,
            bool missingEmailTemplateId = false,
            bool missingNotifyApiKeySecretName = false,
            string emailTemplateError = "",
            string sendNotificationEmailsErrorMessage = "")
        {
            var errorNotifications = new Dictionary<string, string>();

            if (missingEmailAddress)
            {
                errorNotifications.Add(nameof(NotificationMessage.EmailAddresses), $"Missing {nameof(NotificationMessage.EmailAddresses)}");
            }

            if (missingEmailTemplateId)
            {
                errorNotifications.Add(
                    $"{nameof(EmailTemplateResponse)}-{nameof(EmailTemplateResponse.EmailTemplateId)}",
                    $"Missing {nameof(EmailTemplateResponse.EmailTemplateId)}");
            }

            if (missingNotifyApiKeySecretName)
            {
                errorNotifications.Add(
                    $"{nameof(EmailTemplateResponse)}-{nameof(EmailTemplateResponse.NotifyApiKeySecretName)}",
                    $"Missing {nameof(EmailTemplateResponse.NotifyApiKeySecretName)}");
            }

            if (!string.IsNullOrWhiteSpace(emailTemplateError))
            {
                errorNotifications.Add(
                    $"{nameof(EmailTemplateResponse)}-{nameof(EmailTemplateResponse.ErrorMessage)}",
                    emailTemplateError);
            }

            if (!string.IsNullOrWhiteSpace(sendNotificationEmailsErrorMessage))
            {
                var errorMessages = new Dictionary<string, string>
                {
                    { TestEmailAddress, sendNotificationEmailsErrorMessage }
                };

                errorNotifications.Add(
                    $"{nameof(SendNotificationResponse)}-{nameof(SendNotificationResponse.EmailErrorMessages)}",
                    JsonConvert.SerializeObject(errorMessages));
            }

            return errorNotifications;
        }
    }
}