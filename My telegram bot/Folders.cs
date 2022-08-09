using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data.SqlClient;


namespace My_telegram_bot
{
    internal class Folders
    {
        private ITelegramBotClient botClient;
        private Message message;
        private Database database;
        InlineKeyboardMarkup baseInlineKeyboard;

        public Folders(ITelegramBotClient botClient, Message message)
        {
            this.botClient = botClient;
            this.message = message;
            database = new Database();
            baseInlineKeyboard = new
                    (
                    new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text:"See my folders",callbackData: "Folders.See")
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text:"Add",callbackData: "Folders.Add"),
                            InlineKeyboardButton.WithCallbackData(text:"Delete",callbackData: "Folders.Delete")
                        }
                    }

                    );
        }
        public async void MyFolders()
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Folders", replyMarkup: baseInlineKeyboard);
            return;
        }

        private List<string> FolderListFromDB()
        {
            string query = "SELECT DISTINCT FolderName FROM OwnBotFolders";
            SqlCommand command = new SqlCommand(query, database.sqlConnection);

            List<string> result = new List<string>();

            try
            {
                database.OpenConnection();
                SqlDataReader reader = command.ExecuteReader();
                // iterate your results here

                while (reader.Read())
                {
                    result.Add(reader[0].ToString());
                }
                database.CloseConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return result;
        }
        private bool AddToDB(string folderName,string? body)
        {
            string query = body == null ?
                $"INSERT INTO OwnBotFolders( FolderName) VALUES( '{folderName}' )" :
                $"INSERT INTO OwnBotFolders( FolderName,Body ) VALUES( '{folderName}','{body}')";
            try
            {
                database.OpenConnection();
                using (var cmd = new SqlCommand(query, database.sqlConnection))
                {
                    if(cmd.ExecuteNonQuery() == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }


        public async Task HandlerCallbackQueryFolders(CallbackQuery callbackQuery) //inline keyboard click handler
        {
            if (callbackQuery.Data == "Folders.See")
            {
                var foldersName = FolderListFromDB();
                InlineKeyboardMarkup inlineKeyboardMarkup = GetInlineKeyboardCallBackData(foldersName);

                await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, $"All your folders:", replyMarkup: inlineKeyboardMarkup);
                return;
            }
            else 
            if(callbackQuery.Data == "Folders.Back")
            {
                await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, "Folders", replyMarkup: baseInlineKeyboard);
                return;
            }
            else
            if (callbackQuery.Data == "Folders.Add")
            {
                await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, "Add folder");
                AddToDB("TestFolderName",null);
                return;
            }
            else
            if (callbackQuery.Data == "Folders.Delete")
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "TODO: add something :)");
                return;
            }
            else 
            if(callbackQuery.Data == "Folders.NULL")
            {
                botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "They haven't shown up yet <3");
                return;
            }
            else
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Data.Split('.')[1]);
                return;
            }
        }

        private static InlineKeyboardMarkup GetInlineKeyboardCallBackData(List<string> buttonsData) //the method accepts a Dictionary<string text, string callBackData> and returns an inline keyboard
        {
            if (buttonsData.Count > 0)
            {
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

                for (int i = 0; i < buttonsData.Count; i++)
                {
                    keyValuePairs.Add($"Folders.{buttonsData[i]}", buttonsData[i]);
                }
                keyValuePairs.Add("Folders.Back", "◀️ Come back");
                // Rows Count
                int count = keyValuePairs.Count;

                // List of rows 
                // Every 'List<InlineKeyboardButton>' is row
                List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>(count);

                for (int i = 0; i < count; i++)
                {
                    // Add new row with one button capacity
                    buttons.Add(new List<InlineKeyboardButton>(1)
                {
                    new InlineKeyboardButton("some text")
                    {
                       Text = keyValuePairs.Values.ElementAt(i),
                       CallbackData = keyValuePairs.Keys.ElementAt(i),
                    }
                });
                }

                return new InlineKeyboardMarkup(buttons);
            }
            else
            {
                return  new
                    (
                    new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text:"You don't have folders",callbackData: "Folders.NULL")
                        }
                    }
                    );
            }
        }
    }
}
