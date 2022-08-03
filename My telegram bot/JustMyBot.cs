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
using System.Speech.Synthesis;
using System.Speech.AudioFormat;

namespace My_telegram_bot
{
    internal class JustMyBot
    {
        private static string BotToken = Environment.GetEnvironmentVariable("BotToken");
        private TelegramBotClient botClient = new TelegramBotClient(BotToken);
        private CancellationToken cancellationToken = new CancellationToken();
        private ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { } };
        
        private System.Timers.Timer aTimer = new System.Timers.Timer(60000);
        private int count = 1;
        private ShoppingList shoppingList;
        private List<string> list = new List<string>();
        int database = 0;
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
                if (update.Message.From.Username == "egorpustovoit")
                {
                    await HandlerMessageAsync(botClient, update.Message);
                }
                else
                {
                    await using Stream stream = System.IO.File.OpenRead(@"C:\\Users\\Egor\\OneDrive\\Рабочий стол\\Fixed Gear\\notmevoice.wav");
                    await botClient.SendVoiceAsync(update.Message.Chat.Id, voice: new Telegram.Bot.Types.InputFiles.InputOnlineFile(content: stream, fileName: "test.wav"));
                    return;
                }
            }
            if (update?.Type == UpdateType.CallbackQuery)
            {
                await HandlerCallbackQuery(botClient, update.CallbackQuery);
            }
        }

        private async Task HandlerCallbackQuery(ITelegramBotClient botClient, CallbackQuery? callbackQuery)
        {
            if (callbackQuery.Data.StartsWith("ShoppingList"))
            {
                shoppingList.HandlerCallbackQueryShopping(callbackQuery);
                return;
            }
        }



        private async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine("___________________________");
            Console.WriteLine("Text: " + message.Text + ", messageId: " + message.MessageId);
            Console.WriteLine("___________________________");

            if (message.Text == "/start")//comand
            {
                //TODO: add info!
                await botClient.SendTextMessageAsync(message.Chat.Id, "List of commands and reminders for the work of the bot");
            }
            else
            if (message.Text == "/toNotes")
            {
                // заметки вынести в отдельную папу (не делать отдельную фитчу)
                var note = message.ReplyToMessage?.Text;

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
            if (message.Text.StartsWith("/list")) // парсить и получать N (/list N) последних сообщений 
            {
                shoppingList = new ShoppingList(botClient, message);

                await shoppingList.CreateListCommand(list);
                list.Clear();
                count = 1;
                return;
            }
            if(Char.TryParse(message.Text, out char point)) // получать сообщения начиная с сообщения на которое ответили  
            {
                shoppingList = new ShoppingList(botClient, message);

                await shoppingList.CreateListReplay(list);
                list.Clear();
                count = 1;
                return;
            }
            if (message.Text.StartsWith("/voice"))
            {
                var name = message.ReplyToMessage?.Text;

                using (SpeechSynthesizer synth = new SpeechSynthesizer())
                {
                    synth.Volume = 100;
                    synth.Rate = 3;

                    // Configure the audio output.   
                    synth.SetOutputToWaveFile(@"C:\Users\Egor\OneDrive\Рабочий стол\Fixed Gear\test.wav",
                      new SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Mono));

                    // Create a SoundPlayer instance to play output audio file.  
                    //System.Media.SoundPlayer m_SoundPlayer =
                    //  new System.Media.SoundPlayer(@"C:\Users\Egor\OneDrive\Рабочий стол\Fixed Gear\synthBody.mp3");

                    // Build a prompt.  
                    PromptBuilder builder = new PromptBuilder();
                    builder.AppendText(name);

                    // Speak the prompt.  
                    synth.Speak(builder);
                    //m_SoundPlayer.Play();
                }

                //using (var fs = System.IO.File.OpenWrite(Path.Combine(path, "FULL.mp3")))
                //{
                //    var buffer = System.IO.File.ReadAllBytes(Path.Combine(path, "YOUSPENTpart.mp3"));
                //    fs.Write(buffer, 0, buffer.Length);
                //    buffer = System.IO.File.ReadAllBytes(Path.Combine(path, "synthBody.mp3"));
                //    fs.Write(buffer, 0, buffer.Length);
                //    fs.Flush();
                //}



                //string[] vs = new string[]
                //{
                //    "C:\\Users\\Egor\\OneDrive\\Рабочий стол\\Fixed Gear\\synthBody.mp3",
                //    "C:\\Users\\Egor\\OneDrive\\Рабочий стол\\Fixed Gear\\YOUSPENTpart.mp3"

                //};
                //Stream output = System.IO.File.OpenWrite("C:\\Users\\Egor\\OneDrive\\Рабочий стол\\Fixed Gear\\YOUSPENTpart.mp3");

                //foreach (string file in vs)
                //{
                //    Mp3FileReader reader = new Mp3FileReader(file);
                //    if ((output.Position == 0) && (reader.Id3v2Tag != null))
                //    {
                //        output.Write(reader.Id3v2Tag.RawData, 0, reader.Id3v2Tag.RawData.Length);
                //    }
                //    Mp3Frame frame;
                //    while ((frame = reader.ReadNextFrame()) != null)
                //    {
                //        output.Write(frame.RawData, 0, frame.RawData.Length);
                //    }
                //}
                //Console.WriteLine("okkk");

                await using Stream stream = System.IO.File.OpenRead(@"C:\\Users\\Egor\\OneDrive\\Рабочий стол\\Fixed Gear\\test.wav");
                await botClient.SendVoiceAsync(message.Chat.Id, voice: new Telegram.Bot.Types.InputFiles.InputOnlineFile(content: stream, fileName: "test.wav"));
                return;
            }
            else
            {
                if (message.Text != "---")
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{count}) {message.Text}. MessageId = {message.MessageId}");
                list.Add(message.Text);
                //shoppindList.Add(message.Text);
                count++;
                return;
            }
        }
    }
}
