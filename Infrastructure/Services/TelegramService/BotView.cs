using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Services.TelegramService
{
    public class BotView
    {
        private readonly string botToken = Environment.GetEnvironmentVariable("6783215036:AAGg2pRhnoHwUPzYf0I9z3JSq4N3Gh8Gkyo");

        public async Task View(string chatId)
        {
            var botClient = new TelegramBotClient(botToken);

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("AccountManagement", "callback_data_AccountManagement"),
                    InlineKeyboardButton.WithCallbackData("CreateRecord", "callback_data_CreateRecord")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Feedback", "callback_data_Feedback"),
                    InlineKeyboardButton.WithCallbackData("Notifications", "callback_data_Notifications")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Search", "callback_data_Search")
                }
            });

            await botClient.SendTextMessageAsync(chatId, "Выберите одну из кнопок:", replyMarkup: keyboard);
        }
    }
}
