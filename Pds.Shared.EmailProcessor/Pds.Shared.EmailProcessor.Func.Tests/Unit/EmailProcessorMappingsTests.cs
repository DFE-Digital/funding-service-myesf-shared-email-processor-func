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
        public void ToEmailNotification_ValidNotificationMessage_ReturnsExpectedEmailNotification()
        {
            //Arrange
            NotificationMessage inputnotifiction = new NotificationMessage
            {
                EmailAddresses = null,
                RequestingService = null,
                EmailMessageType = null,
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
                EmailAddress = null,
                NotifyApiKeySecretName = null,
                TemplateId = null,
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
            Assert.AreEqual(expected.EmailPersonalisation.Personalisation.Count(), actual.EmailPersonalisation.Personalisation.Count());
            Assert.AreEqual(expected.EmailAddress, actual.EmailAddress);
            Assert.AreEqual(expected.NotifyApiKeySecretName, actual.NotifyApiKeySecretName);
            Assert.AreEqual(expected.TemplateId, actual.TemplateId);
        }

        [TestMethod]
        public void ToEmailTemplateRequest_ValidNotificationMessage_ReturnsExpectedEmailNotification()
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
            Assert.AreEqual(expected.MetaData.Count(), actual.MetaData.Count());
        }
    }
}
