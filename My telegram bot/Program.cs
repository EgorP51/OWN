using My_telegram_bot;
using System.Timers;
using System.Data.SqlClient;


JustMyBot justMyBot = new JustMyBot();
justMyBot.Start();
while (true)
{

}

//using System;
//using System.Speech.Synthesis;


//// Initialize a new instance of the SpeechSynthesizer.  
//SpeechSynthesizer synth = new SpeechSynthesizer();
//synth.Rate = -2;

//// Configure the audio output.   
//synth.SetOutputToDefaultAudioDevice();

//// Speak a string.  
//synth.Speak("Рубрика c# эксперименты ");


//Console.WriteLine();
//Console.WriteLine("Press any key to exit...");
//Console.ReadKey();



































////Console.WriteLine("0");
////Database database = new Database();

////string query = @"select * from OwnBotSpending";
////SqlCommand command = new SqlCommand(query, database.GetSqlConnection());
////database.OpenConnection();
////SqlDataReader reader = command.ExecuteReader();

////if (reader.HasRows) // если есть данные
////{
////    // выводим названия столбцов
////    Console.WriteLine("{0}\t{1}\t{2}", reader.GetName(0), reader.GetName(1), reader.GetName(2));

////    while (reader.Read()) // построчно считываем данные
////    {
////        object id = reader.GetValue(0);
////        object name = reader.GetValue(1);
////        object age = reader.GetValue(2);

////        Console.WriteLine("{0} \t{1} \t{2}", id, name, age);
////    }
////}
////reader.Close();

////Console.WriteLine($"{DateTime.Now} + {DateTime.Now.TimeOfDay} + {DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}");
//string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Egor\source\repos\My telegram bot\My telegram bot\OwnDatabase.mdf;Integrated Security=True";
//SqlConnection connection = new SqlConnection(connectionString);
//Random random = new Random();

////for (int i = 0; i < 20; i++)
////{

////new Database().OpenConnection();
////new Database().CloseConnection();

//Console.WriteLine("_______________________________________");
//if (connection.State == System.Data.ConnectionState.Closed)
//{
//    ReadFromDB(connection);
//}
//else
//{
//    connection.Close();
//    ReadFromDB(connection);
//}
//Console.WriteLine("_______________________________________");

//Console.WriteLine("_______________________________________");
//SqlCommand sqlCommand = new SqlCommand($"INSERT INTO [OwnBotSpending] (Body, Time) VALUES(" +
//                $"55555,GETDATE())", connection);
//Console.WriteLine(sqlCommand.ExecuteNonQuery().ToString());
//if (connection.State == System.Data.ConnectionState.Closed)
//{
//    ReadFromDB(connection);
//}
//else
//{
//    connection.Close();
//    ReadFromDB(connection);
//}
//Console.WriteLine("_______________________________________");

//Console.WriteLine("_______________________________________");
//SqlCommand sqlCommand1 = new SqlCommand($"INSERT INTO [OwnBotSpending] (Body, Time) VALUES(" +
//                $"444444, '{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}')", connection);
//Console.WriteLine(sqlCommand1.ExecuteNonQuery().ToString());
//if (connection.State == System.Data.ConnectionState.Closed)
//{
//    ReadFromDB(connection);
//}
//else
//{
//    connection.Close();
//    ReadFromDB(connection);
//}
//Console.WriteLine("_______________________________________");




















////if (connection.State == System.Data.ConnectionState.Closed)
////{
////    Find(connection);
////}
////else
////{
////    connection.Close();
////    Find(connection);
////}
////AddToDB(connection);
////Console.WriteLine("_______________________________________");
////SqlCommand sqlCommand = new SqlCommand($"INSERT INTO [OwnBotSpending] (Body, Time) VALUES(" +
////                $"55555,GETDATE())", connection);
////Console.WriteLine(sqlCommand.ExecuteNonQuery().ToString());
////    if (connection.State == System.Data.ConnectionState.Closed)
////    {
////        ReadFromDB(connection);
////    }
////    else
////    {
////        connection.Close();
////        ReadFromDB(connection);
////    }
////}

//void ReadFromDB(SqlConnection connection)
//{
//    string sqlExpression = "SELECT * FROM OwnBotSpending";

//    connection.Open();
//    SqlCommand command = new SqlCommand(sqlExpression, connection);
//    SqlDataReader reader = command.ExecuteReader();

//    if (reader.HasRows) // если есть данные
//    {
//        // выводим названия столбцов
//        Console.WriteLine("{0}\t{1}\t{2}", reader.GetName(0), reader.GetName(1), reader.GetName(2));

//        while (reader.Read()) // построчно считываем данные
//        {
//            object id = reader.GetValue(0);
//            object name = reader.GetValue(1);
//            object age = reader.GetValue(2);

//            Console.WriteLine("{0} \t{1} \t{2}", id, name, age);
//        }
//    }

//    reader.Close();
//}

////void AddToDB(SqlConnection sqlConnection)
////{//'{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}'
////    SqlCommand sqlCommand = new SqlCommand(new Spending(44444, true).request, sqlConnection);
////    Console.WriteLine(sqlCommand.ExecuteNonQuery().ToString());

////}
////void Find(SqlConnection sqlConnection)
////{
////    //SqlCommand sqlCommand = new SqlCommand($"SELECT Body FROM OwnBotSpending WHERE Body>15", sqlConnection);
////    string sqlExpression = $"SELECT Id,Body,Time " +
////        $"FROM OwnBotSpending " +
////        $"WHERE DATEDIFF(day,Time,GETDATE())<1";

////    Console.WriteLine($"'{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}'");
////    connection.Open();
////    SqlCommand command = new SqlCommand(sqlExpression, connection);
////    SqlDataReader reader = command.ExecuteReader();

////    if (reader.HasRows) // если есть данные
////    {
////        // выводим названия столбцов
////        //Console.WriteLine("{0}\t\t{2}", reader.GetName(0), reader.GetName(2));
////        Console.WriteLine("{0}\t{1}\t{2}", reader.GetName(0), reader.GetName(1),reader.GetName(2));
////        while (reader.Read()) // построчно считываем данные
////        {
////            object id = reader.GetValue(0);
////            object name = reader.GetValue(1);
////            object age = reader.GetValue(2);

////            Console.WriteLine("{0} \t{1}\t{2} ", id,name,age);// name, age
////        }
////    }

////    reader.Close();
////}
////Console.Read();
