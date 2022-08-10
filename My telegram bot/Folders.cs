using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data.SqlClient;


namespace My_telegram_bot
{
    internal class Folders
    {
        public InlineKeyboardMarkup BaseInlineKeyboard { get;private set; }
        private ITelegramBotClient botClient;
        private string newFolderName;
        private Database database;
        private Message message;
        public bool IsNewFolder { get; private set; } = false;
        public bool IsAddingToFolder { get; set; } = false;
        public int MessageId { get;set; }

        public Folders(ITelegramBotClient botClient, Message message)
        {
            this.botClient = botClient;
            this.message = message;
            database = new Database();
            BaseInlineKeyboard = new
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
                        }
                    }

                    );
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

        public bool AddToDB(string folderName, string? body)
        {
            string query = body == null ?
                $"INSERT INTO OwnBotFolders( FolderName) VALUES( '{folderName}' )" :
                $"INSERT INTO OwnBotFolders( FolderName,Body ) VALUES( '{folderName}','{body}')";
            try
            {
                database.OpenConnection();
                using (var cmd = new SqlCommand(query, database.sqlConnection))
                {
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }

        public async void GetNewFolderName(Message mess)
        {
            IsNewFolder = false;
            newFolderName = mess.Text;
            InlineKeyboardMarkup InlineKeyboard = new
                (
                new[]
                {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text:"Yes",callbackData: "Folders.addNewFolder")
                        }
                }

                );
            await botClient.EditMessageTextAsync(
                mess.Chat.Id,
                mess.MessageId-1,
                $"Create new folder \"{newFolderName}\"?",
                replyMarkup: InlineKeyboard);
            IsNewFolder = true;

            return;
        }

        public async Task GetMessagesFromDB(string folderName)
        {
            string query = $"SELECT Body FROM OwnBotFolders WHERE FolderName='{folderName}'";
            SqlCommand command = new SqlCommand(query, database.sqlConnection);

            List<object> result = new List<object>();

            try
            {
                database.OpenConnection();
                SqlDataReader reader = command.ExecuteReader();
                // iterate your results here

                while (reader.Read())
                {
                    result.Add(reader[0]);
                }
                database.CloseConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            foreach(var item in result)
            {
                Thread.Sleep(100);
                if (item != null)
                {
                    try
                    {
                        await botClient.ForwardMessageAsync(message.Chat.Id, message.Chat.Id, int.Parse(item.ToString()));
                        Console.WriteLine("OK => " + item.ToString());
                        //return;

                    }catch (Exception ex)
                    {
                        //await botClient.SendTextMessageAsync(message.Chat.Id, "Error");
                        Console.WriteLine("Error Folder.cs line 159 => "+item.ToString());
                    }
                }
            }
        }

        public InlineKeyboardMarkup MyFolders()
        {
            var foldersName = FolderListFromDB();
            InlineKeyboardMarkup inlineKeyboardMarkup = GetInlineKeyboardCallBackData(foldersName);
            return inlineKeyboardMarkup;
        }
        public async Task HandlerCallbackQueryFolders(CallbackQuery callbackQuery) //inline keyboard click handler
        {
            if (callbackQuery.Data == "Folders.See")
            {
                InlineKeyboardMarkup inlineKeyboardMarkup = MyFolders();

                await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, $"All your folders:", replyMarkup: inlineKeyboardMarkup);
                return;
            }
            else
            if (callbackQuery.Data == "Folders.Back")
            {
                await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, "Folders", replyMarkup: BaseInlineKeyboard);
                return;
            }
            else
            if (callbackQuery.Data == "Folders.Add")
            {
                await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, "Enter folder name");
                IsNewFolder = true;
                return;
            }
            else if (callbackQuery.Data == "Folders.addNewFolder")
            {
                if (AddToDB(newFolderName, null))
                {
                    await botClient.SendTextMessageAsync(
                        message.Chat.Id,
                        $"New folder has successfully created");
                }
                else
                {
                    await botClient.EditMessageTextAsync(message.Chat.Id, message.MessageId, "Problem with adding");
                }
            }
            else
            if (callbackQuery.Data == "Folders.NULL")
            {
                botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "They haven't shown up yet <3");
                return;
            }
            else
            {
                string foderName = callbackQuery.Data.Split('.')[1];

                if (IsAddingToFolder)
                {
                    if(AddToDB(foderName, MessageId.ToString()))
                    {
                        await botClient.EditMessageTextAsync(message.Chat.Id, message.MessageId+1,$"Successfully added to database");
                        IsAddingToFolder = false;
                        return;
                    }
                    else
                    {
                        await botClient.EditMessageTextAsync(message.Chat.Id, message.MessageId, "Problem with adding");
                        return;
                    }
                }
                else
                {
                    GetMessagesFromDB(foderName);
                }
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

                int count = keyValuePairs.Count;

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
                return new
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
