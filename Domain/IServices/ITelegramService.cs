using Domain.Entities;

namespace Domain.IServices;

public interface ITelegramService
{
    Task<Record> SearchRecordByTitle(string title);
    Task CreateRecord(string title, string text, DateTime deadline, bool isPrivate);
    Task<User> GetCurrentUserInfo();
    Task EditUserInfo(User user);
    Task SendFeedback(string contactInfo, string message);
}

