using Pds.Shared.EmailProcessor.Services.Models;
using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Services.Interfaces
{
    /// <summary>
    /// The ISendNotification interface.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    public interface ISendNotificationService<in T>
        where T : class
    {
        /// <summary>
        /// Sends the notification asynchronous.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns>The SendNotificationResponse.</returns>
        Task<SendNotificationResponse> SendNotificationAsync(T notification);
    }
}