using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Exceptions;

namespace My_telegram_bot
{
    internal class JustMyBot
    {
        private static string BotToken = Environment.GetEnvironmentVariable("BotToken"); 
        private TelegramBotClient botClient = new TelegramBotClient(BotToken);
        private CancellationToken cancellationToken = new CancellationToken();
        private ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { } };
        private ReplyKeyboardMarkup BaseReplyKeyboardMarkup;
        private bool isShoppingListElement = false;
        private bool isNotes = false;
        private bool isSpending = false;
        int database = 0;
        private System.Timers.Timer aTimer = new System.Timers.Timer(60000);
        private int count = 1;
        //private List<string> shoppindList = new List<string>();

        //public JustMyBot()
        //{
        //    BaseReplyKeyboardMarkup =
        //    new
        //    (
        //        new[]
        //        {
        //                new KeyboardButton[] { "Folders", "Notes" },
        //                new KeyboardButton[] { "Shopping list", "Spending" }
        //        }
        //    )
        //    {
        //        ResizeKeyboard = true,
        //        OneTimeKeyboard = true
        //    };

        //}
        public async Task Start()
        {
            botClient.StartReceiving(HandlerUpdateAsync, HandlerError, receiverOptions, cancellationToken);
            var botMe = await botClient.GetMeAsync();
            Console.WriteLine($"Bot is working...");
            while (true)
            {
                Thread.Sleep(240000);
                if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 15 && DateTime.Now.Minute <= 20) // стата за день, бот проверяет время каждую минуту
                {
                    await DayStatistics();
                    return;
                }
            }
            Console.ReadKey();
        }
        public async Task DayStatistics() // дописать стату !
        {
            Console.Beep(344, 2345);
            await botClient.SendTextMessageAsync(783450273, "Ваша статистика");
            return;
        }
        private Task HandlerError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram bot API error:\n {apiRequestException.ErrorCode}" +
                $"\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandlerMessageAsync(botClient, update.Message);
            }
            if (update?.Type == UpdateType.CallbackQuery)
            {
                await HandlerCallbackQuery(botClient, update.CallbackQuery);
            }
            if (update?.Type == UpdateType.Message && update.Message.Type == MessageType.Voice)
            {
                    var message = update.Message;

                //    var file = botClient.GetFileAsync(message.Voice.FileId);
                //    Console.WriteLine(file);
                //    var fileName = @"C:\Users\Egor\OneDrive\Рабочий стол\Fixed Gear\" + file.Result.FileId + "." + file.Result.FilePath.Split('.').Last();
                //    Console.WriteLine(fileName);

                //    using (var saveImageStream = System.IO.File.Open(fileName, FileMode.Create))
                //    {
                //        botClient.DownloadFileAsync(file.Result.FilePath, saveImageStream);
                //    }
                //    botClient.SendTextMessageAsync(message.Chat.Id, "Image save");
                //    return;
                var file = await botClient.GetFileAsync(message.Voice.FileId);
                FileStream fs = new FileStream(@"C:\Users\Egor\OneDrive\Рабочий стол\Fixed Gear\voice1.ogg", FileMode.Create);
                await botClient.DownloadFileAsync(file.FilePath, fs);
                fs.Close();
                fs.Dispose();
            }
        }

        private async Task HandlerCallbackQuery(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
        {
            if (callbackQuery.Data.StartsWith(" "))
            {
            }
        }



        private async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine("___________________________");
            Console.WriteLine("Text: " + message.Text + ", messageId: " + message.MessageId);
            Console.WriteLine(message.From.Username);
            Console.WriteLine("___________________________");

            if (message.Text == "/start")//comand
            {
                //TODO: add info!
                await botClient.SendTextMessageAsync(message.Chat.Id, "List of commands and reminders for the work of the bot");
            }
            else
            if (message.Text == "/toNotes")
            {
                var note = message.ReplyToMessage?.Text;
                Console.WriteLine($"User {message.From.Username} save text {note} to notes");
                await botClient.SendTextMessageAsync(message.Chat.Id, $"You saved text {note} to notes");
                // to database
                return;
            }
            else
            if (int.TryParse(message.Text, out int currentSpending))
            {
                Console.WriteLine($"Current user spending: {currentSpending}");
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Current user spending: {currentSpending}");
                // seave to databese 
                database += currentSpending;
                return;
            }
            else
            if (message.Text == "/all")//TODO: improve comand
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"All spending: {database}");
                Console.WriteLine($"All spending: {database}");
                // get info from database
                return;
            }
            if (message.Text.StartsWith("/list") || message.Text.StartsWith("/Готово"))
            {

                ShoppingList shoppingList = new ShoppingList(botClient,message);
                
                await shoppingList.ListViaCommand();
                return;

                ////int listItemNumber = int.Parse(message.Text.Split(' ')[1]);

                //// not for prod
                //string text = "";                                                                       // wery good !
                ////await botClient.SendTextMessageAsync(message.Chat.Id, "Start with?", replyToMessageId: message.MessageId - (listItemNumber * 2));
                //List<Message> messages = new List<Message>();

                //for (int i = message.MessageId - (listItemNumber * 2); i < message.MessageId; i += 2)
                //{
                //    Message message1 = botClient.ForwardMessageAsync(message.Chat.Id, message.Chat.Id, i).Result ; //message.MessageId - (listItemNumber * 2)
                //    messages.Add(message1);
                //}
                //string list = "Your shopping list: \n";
                //foreach(Message message2 in messages)
                //{
                //    list += "- "+ message2.Text + "\n";
                //}
                //await botClient.SendTextMessageAsync(message.Chat.Id, list);
                //return;
                ////TODO: Решить как правильно организовать список покупок, конкретизируюя, как получать сообщения по Id

                //return;
            }
            if (message.Text == "/voice")
            {
                //await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                //const string filePath = @"C:\Users\Egor\OneDrive\Рабочий стол\Fixed Gear\AwACAgIAAxkBAAIBjGLiNVTL2klfx5Rj8HJ63sUooftoAAIlHAACNfkRS_PoKw8JTPgCKQQ.oga";
                //using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                //var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

                //await botClient.SendVoiceAsync(message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(fileStream, fileName));
                //return;

                //await using Stream stream = System.IO.File.OpenRead(Constants.PathToFile.Documents.Hamlet);
                //Message message = await BotClient.SendDocumentAsync(
                //    chatId: _fixture.SupergroupChat.Id,
                //    document: new InputOnlineFile(content: stream, fileName: "HAMLET.pdf"),
                //    caption: "The Tragedy of Hamlet,\nPrince of Denmark"

                await using Stream stream = System.IO.File.OpenRead(@"C:\Users\Egor\OneDrive\Рабочий стол\Fixed Gear\voice1.mp3");
                await botClient.SendVoiceAsync(message.Chat.Id, voice: new Telegram.Bot.Types.InputFiles.InputOnlineFile(content: stream, fileName: "voice1.ogg"));
                return;
            }
            else
            {
                //await botClient.SendTextMessageAsync(message.Chat.Id, "I don't understand your request");

                await botClient.SendTextMessageAsync(message.Chat.Id,$"{count}) {message.Text}. MessageId = {message.MessageId}");
                //shoppindList.Add(message.Text);
                count++;
                return;
            }
        }
    }
}
