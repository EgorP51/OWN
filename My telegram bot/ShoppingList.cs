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
        List<string> shoppingList = new List<string>();
        private string likeSticker = "✅";
        private string beforeLikeSticker = "⬜️";
        private string caption = "Your shopping list 🗓: \n";

        public ShoppingList(ITelegramBotClient botClient, Message message)
        {
            this.botClient = botClient;
            this.message = message;
            listItemNumber = int.Parse(message.Text.Split(' ')[1]);
        }
        private Dictionary<string, string> CreateDictionary(List<string> shoppingList)
        {
            for (int i = 0; i < shoppingList.Count; i++)
            {
                callbackQueryList.Add($"{beforeLikeSticker} {shoppingList[i].ToUpper()}", $"ShoppingList.{shoppingList[i]}.false");
            }
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
                       Text = buttonsData.Keys.ElementAt(i),
                       CallbackData = buttonsData.Values.ElementAt(i),
                    }
                });
            }

            return new InlineKeyboardMarkup(buttons);
        }

        public async Task ListViaCommand()
        {
            for (int i = message.MessageId - (listItemNumber * 2); i < message.MessageId; i += 2)
            {
                Message tempMessage = botClient.ForwardMessageAsync(message.Chat.Id, message.Chat.Id, i).Result; //message.MessageId - (listItemNumber * 2)
                shoppingList.Add(tempMessage.Text);
            }

            InlineKeyboardMarkup inlineKeyboardMarkup = GetInlineKeyboardCallBackData(CreateDictionary(shoppingList));

            await botClient.SendTextMessageAsync(message.Chat.Id, caption, replyMarkup: inlineKeyboardMarkup);
            return;
        }
        public void ListViaReplay()
        {

        }
        //public async Task HandlerCallbackQueryShopping(CallbackQuery callbackQuery)
        //{
        //    bool isItBought = bool.Parse(callbackQuery.Data.Split('.')[2]);
        //    if (isItBought)
        //    {
        //        for (int i = 0; i < callbackQueryList.Count; i++)
        //        {
        //            if (callbackQueryList.TryGetValue($"{beforeLikeSticker} {callbackQuery.Data.Split('.')[1]}",out string value))
        //            {
        //                if(value == callbackQuery.Data)
        //                {
        //                    callbackQueryList[$"{beforeLikeSticker} {callbackQuery.Data.Split('.')[1]}"] = callbackQueryList.Add($"{likeSticker} {callbackQuery.Data.Split('.')[1]}", callbackQuery.Data.Replace("false","true"));
        //                }
        //            }
        //        }
        //        //foreach (KeyValuePair<string, string> item in callbackQueryList)
        //        //{
        //        //    if (item.Value == callbackQuery.Data)
        //        //    {
        //        //        item.
        //        //    }
        //        //}
        //    }

            //TODO:дописать обработчик, продумать сохранение списков, продумать получение сообщений по из id
            //await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, caption);
            //return;
        //}
    }
}
