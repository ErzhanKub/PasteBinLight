using Domain.Entities;
using Domain.IServices;
using Newtonsoft.Json;
using System.Text;
using Telegram.Bot;

namespace Infrastructure.Services.TelegramService
{
    public sealed class TelegramService : ITelegramService
    {
        private const string token = "6783215036:AAGg2pRhnoHwUPzYf0I9z3JSq4N3Gh8Gkyo";
        private readonly TelegramBotClient client;
        private readonly HttpClient httpClient;

        public TelegramService()
        {
            client = new TelegramBotClient(token);
            httpClient = new HttpClient();
        }

        public Task AccountManagement()
        {
            throw new NotImplementedException();
        }

        public Task CreateRecord(string title, string text, DateTime deadline, bool isPrivate)
        {
            throw new NotImplementedException();
        }

        public async Task CreateRecordFromTG()
        {
            try
            {
                var updates = await client.GetUpdatesAsync();
                var chatId = updates.First().Message.Chat.Id;

                var content = new StringContent(JsonConvert.SerializeObject(new { text = updates }),
                    Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://localhost:7056/api/Record/", content);

                if (response.IsSuccessStatusCode)
                {
                    await client.SendTextMessageAsync(chatId, "Запись успешно создана");
                }
                else
                {
                    await client.SendTextMessageAsync(chatId, "Ошибка при создании записи");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        public Task EditUserInfo(User user)
        {
            throw new NotImplementedException();
        }

        public Task Feedback()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetCurrentUserInfo()
        {
            throw new NotImplementedException();
        }

        public Task Notifications()
        {
            throw new NotImplementedException();
        }

        public Task Search()
        {
            throw new NotImplementedException();
        }

        public Task<Record> SearchRecordByTitle(string title)
        {
            throw new NotImplementedException();
        }

        public Task SendFeedback(string contactInfo, string message)
        {
            throw new NotImplementedException();
        }

        public async Task SendMessage(string text)
        {
            try
            {
                var updates = await client.GetUpdatesAsync();
                var id = updates.First().Message.Chat.Id;
                await client.SendTextMessageAsync(id.ToString(), text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}
