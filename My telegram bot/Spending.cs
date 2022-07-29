using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_telegram_bot
{
    internal class Spending
    {

        private int currentSpending;
        private int recentSpending;
        public string request { get; private set; }
        public Spending(int daysOrSpending, bool isSpending)
        {
            if(isSpending)
                request = $"INSERT INTO [OwnBotSpending] (Body, Time) VALUES(" +
                $"{daysOrSpending},GETDATE())";
            else
                request = $"SELECT Body,Time FROM OwnBotSpending WHERE Time>'{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}'";
        }
        public Spending(bool isDaySpending)
        {
            if (isDaySpending)
                request = $"SELECT Body,Time FROM OwnBotSpending WHERE Time=GETDATE()";
            else
                request = null;
        }
        
    }
}
