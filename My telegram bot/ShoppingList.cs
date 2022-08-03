using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace My_telegram_bot
{
    internal class ShoppingList
    {
        private ITelegramBotClient botClient;
        private Message message;
        int listItemNumber;
        Dictionary<string, string> callbackQueryList = new Dictionary<string, string>();
        List<Message> messages = new List<Message>();
        //List<string> shoppingList = new List<string>();
        private string likeSticker = "✅";
        private string beforeLikeSticker = "⬜️";
        private string caption = $"Your shopping list 🗓:\n({DateTime.Now})\n";


        public ShoppingList(ITelegramBotClient botClient, Message message)
        {
            this.botClient = botClient;
            this.message = message;
        }

        private Dictionary<string, string> CreateDictionary(List<string> shoppingList)
        {
            for (int i = 0; i < shoppingList.Count; i++)
            {
                callbackQueryList.Add($"ShoppingList.{shoppingList[i]}",$"{beforeLikeSticker} {shoppingList[i].ToUpper()}");
            }
            callbackQueryList.Add("ShoppingList.Delete", "❌ Delete list");

            return callbackQueryList;
        }
        private static InlineKeyboardMarkup GetInlineKeyboardCallBackData(Dictionary<string, string> buttonsData)
        {
            //the method accepts a Dictionary<string text, string callBackData>
            // and returns an inline keyboard

            // Rows Count
            int count = buttonsData.Count;

            // List of rows 
            // Every 'List<InlineKeyboardButton>' is row
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>(count);

            for (int i = 0; i < count; i++)
            {
                // Add new row with one button capacity
                buttons.Add(new List<InlineKeyboardButton>(1)
                {
                    new InlineKeyboardButton("some tex")
                    {
                       Text = buttonsData.Values.ElementAt(i),
                       CallbackData = buttonsData.Keys.ElementAt(i),
                    }
                });
            }

            return new InlineKeyboardMarkup(buttons);
        }

        public async Task CreateListReplay(List<string> list)
        {
            string startWord = message.ReplyToMessage.Text;

            for (int i = 0; i < list.Count; i++)
            {
                if(list[i] == startWord)
                {
                    list.RemoveRange(0,i);
                }
            }

            InlineKeyboardMarkup inlineKeyboardMarkup = GetInlineKeyboardCallBackData(CreateDictionary(list));

            await botClient.SendTextMessageAsync(message.Chat.Id, caption, replyMarkup: inlineKeyboardMarkup);
            return;
        }
        public async Task CreateListCommand(List<string> list)
        {
            int count = int.Parse(message.Text.Split(' ')[1]);
            Console.WriteLine(string.Join(", ", list));

            if (count > list.Count)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Не правильное количиство элементов списка");
                return;
            }
            else
            if(count < list.Count)
            {
                list.RemoveRange(count,list.Count - count);
            }

            InlineKeyboardMarkup inlineKeyboardMarkup = GetInlineKeyboardCallBackData(CreateDictionary(list));

            await botClient.SendTextMessageAsync(message.Chat.Id, caption, replyMarkup: inlineKeyboardMarkup);
            return;
        }

        public async Task HandlerCallbackQueryShopping(CallbackQuery callbackQuery) //TODO: Добавитиь Сохранение??
        {
            if (callbackQuery.Data == "ShoppingList.Delete")
            {
                await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, "* * *");
                return;
            }
            else
            {
                foreach (KeyValuePair<string, string> pair in callbackQueryList)
                {
                    if (pair.Key == callbackQuery.Data)
                    {
                        if (pair.Value.StartsWith(beforeLikeSticker))
                            callbackQueryList[pair.Key] = pair.Value.Replace(beforeLikeSticker, likeSticker);
                        else if (pair.Value.StartsWith(likeSticker))
                            callbackQueryList[pair.Key] = pair.Value.Replace(likeSticker, beforeLikeSticker);

                        break;
                    }
                }
                InlineKeyboardMarkup inlineKeyboardMarkup = GetInlineKeyboardCallBackData(callbackQueryList);
                await botClient.EditMessageTextAsync(
                    callbackQuery.Message.Chat.Id,
                    callbackQuery.Message.MessageId,
                    text: caption,
                    replyMarkup: inlineKeyboardMarkup);
            }
            return;
        }
    }
}
