using System.Threading.Tasks;

namespace Metomarket.Services.Messaging
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}