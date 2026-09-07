using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notify.Models;
using Pds.Core.Notification.Models;
using Pds.Shared.EmailProcessor.Func.Extensions;
using Pds.Shared.EmailProcessor.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Func.Tests.Unit
{
    [TestClass, TestCategory("Unit")]
    public class EmailProcessorMappingsTests
    {
        [TestMethod]
        public void TestToEmailNotification_ValidNotificationMessage_ReturnsExpectedEmailNotification()
        {
            //Arrange
            NotificationMessage inputnotifiction = new NotificationMessage
            {
                EmailAddresses = new List<string> { },
                RequestingService = "test",
                EmailMessageType = "test",
                EmailPersonalisation = new Core.Notification.Models.GovUkNotifyPersonalisation
                {
                    Personalisation = new Dictionary<string, object>
                    {
                        { "test", "testobject" },
                        { "test2", "testobject2" }
                    }
                },
            };

            EmailNotification expected = new EmailNotification
            {
                EmailAddress = "email",
                NotifyApiKeySecretName = "test",
                TemplateId = "test",
                EmailPersonalisation = new Services.Models.GovUkNotifyPersonalisation
                {
                    Personalisation = new Dictionary<string, object>
                    {
                        { "test", "testobject" },
                        { "test2", "testobject2" }
                    }
                },
            };

            //Act
            EmailNotification actual = EmailProcessorMappings.ToEmailNotifiction(inputnotifiction);

            //Assert
            Assert.AreEqual(expected.EmailPersonalisation.Personalisation.First(), actual.EmailPersonalisation.Personalisation.First());
        }

        [TestMethod]
        public void TestToEmailTemplateRequest_ValidNotificationMessage_ReturnsExpectedEmailNotification()
        {
            //Arrange
            NotificationMessage inputnotifiction = new NotificationMessage
            {
                MetaData = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("test1", "test2")
                },
                EmailMessageType = "test",
                RequestingService = "test",
            };

            EmailTemplateRequest expected = new EmailTemplateRequest
            {
                MetaData = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("test1", "test2")
                },
                EmailMessageType = "test",
                RequestingService = "test",
            };

            //Act
            EmailTemplateRequest actual = EmailProcessorMappings.ToEmailTemplateRequest(inputnotifiction);

            //Assert
            Assert.AreEqual(expected.EmailMessageType, actual.EmailMessageType);
            Assert.AreEqual(expected.RequestingService, actual.RequestingService);
            Assert.AreEqual(expected.MetaData.First(), actual.MetaData.First());
        }
    }
}
