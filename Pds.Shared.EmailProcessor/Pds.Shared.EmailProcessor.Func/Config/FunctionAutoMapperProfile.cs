using AutoMapper;
using Pds.Core.Notification.Models;
using Pds.Shared.EmailProcessor.Services.Models;
using GovUkNotifyPersonalisationCore = Pds.Core.Notification.Models.GovUkNotifyPersonalisation;

namespace Pds.Shared.EmailProcessor.Func.Config
{
    /// <summary>
    /// The Function AutoMapper Profile.
    /// </summary>
    /// <seealso cref="Profile" />
    public class FunctionAutoMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionAutoMapperProfile"/> class.
        /// </summary>
        public FunctionAutoMapperProfile()
        {
            CreateMap<NotificationMessage, EmailNotification>()
                .ReverseMap();
            CreateMap<GovUkNotifyPersonalisationCore, Services.Models.GovUkNotifyPersonalisation>()
                .ReverseMap();
            CreateMap<NotificationMessage, EmailTemplateRequest>()
                .ReverseMap();
        }
    }
}