namespace Kingpinbot
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	using NetTelegramBotApi.Types;

	using Telegram.Bot;
	using Telegram.Bot.Args;
	using Telegram.Bot.Types.InlineQueryResults;

	internal class Program
	{
		private static readonly List<InlineQueryResult> _queryResults 
			= new List<InlineQueryResult>();
		
		private static void Main(string[] args)
		{
			Console.WriteLine("Kingpin bot starting...");

			var key = "...";

			var sounds = new Dictionary<string, string>();
			
			sounds.Add("Что за фигня?", "https://dl.dropbox.com/s/34slcu9lvd23124/ChtoZaHuinya.mp3");
			sounds.Add("Что тут творится?", "https://dl.dropbox.com/s/cw5cn61t1vicyuk/ChtoTutTvoritsya.mp3");
			sounds.Add("Голова болит :(", "https://dl.dropbox.com/s/stxq68sr73olcgp/BashkaBolit.mp3");
			sounds.Add("Что тебе нужно?", "https://dl.dropbox.com/s/vhxg1ca86g2mq1e/CheNadoSuka.mp3");
			sounds.Add("Я вами пренебрегаю.", "https://dl.dropbox.com/s/vys3van6w5leis3/ZhopaUmeetRazgovarivat.ogg");
			sounds.Add("Я вами сильно пренебрегаю.", "https://dl.dropbox.com/s/hx1zszhpbwpxhxl/ZasunRukuVZhopu.mp3");
			sounds.Add("Отстань.", "https://dl.dropbox.com/s/vxu7y4ygk1p5pjq/BlyaOtebis.mp3");
			sounds.Add("Оплата вперёд.", "https://dl.dropbox.com/s/9djkck9b5hbfnfs/DengiTashi.mp3");
			sounds.Add("Плохо себя чувствую :(", "https://dl.dropbox.com/s/b81uhwqm3pwlx2q/MneHuiovo.mp3");
			sounds.Add("Я устал.", "https://dl.dropbox.com/s/mttjad7abxbuecn/VseZaeblo.mp3");
			sounds.Add("Пьёте?", "https://dl.dropbox.com/s/fd853sn2kcmoceh/Alkashi.mp3");
			sounds.Add("Сколько?", "https://dl.dropbox.com/s/2h7fjfzscadojwk/Skolko.mp3");
			sounds.Add("Немало.", "https://dl.dropbox.com/s/fl7upzk3x9hwr3c/Nemalo.mp3");
			sounds.Add("Я спрашиваю, сколько?!", "https://dl.dropbox.com/s/icjiaru5x4ufpsf/YaSprashivauSkolko.mp3");
			sounds.Add("Вот так дела!", "https://dl.dropbox.com/s/vd1sl4x0jhyck04/PizdetsKotenku.mp3");
			sounds.Add("Неплохо.", "https://dl.dropbox.com/s/qtbmvdhmm4z7rv2/DaEtoKruto.mp3");
			sounds.Add("Спасибо!", "https://dl.dropbox.com/s/lsz2ovopt5chi89/ZaebisSpasibo.mp3");

			foreach (var sound in sounds)
			{
				var voice = new InlineQueryResultVoice();
				voice.Url = sound.Value;
				voice.Title = sound.Key;
				voice.Id = sound.Key;
				voice.Duration = 3;
				_queryResults.Add(voice);
			}

			BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += ProcessBot;

			if (bw.IsBusy == false)
			{
				bw.RunWorkerAsync(key);
			}

			Console.WriteLine("Kingpin bot started");
			Console.WriteLine("Press any key to exit.");
			Console.ReadLine();
		}

		private static void ProcessBot(object sender, DoWorkEventArgs e)
		{
			try
			{
				var key = e.Argument as string;
				var bot = new TelegramBotClient(key);
				
				bot.OnInlineQuery += AnswerInlineQuery;
				bot.OnInlineResultChosen += InlineResultChosen;
				bot.OnReceiveError += ErrorRecieved;
				bot.OnReceiveGeneralError += GeneralErrorReceived;
				
				bot.StartReceiving();

				Console.WriteLine($"Started message recieving.");
				Console.ReadLine();

				bot.StopReceiving();
			}
			catch (Telegram.Bot.Exceptions.ApiRequestException ex)
			{
				Console.WriteLine(ex.Message);
				Console.ReadLine();
			}
		}

		private static void GeneralErrorReceived(object sender, ReceiveGeneralErrorEventArgs e)
		{
			Console.WriteLine("Error catched: " + e.Exception.Message);
		}

		private static void ErrorRecieved(object sender, ReceiveErrorEventArgs e)
		{
			Console.WriteLine("Error catched: " + e.ApiRequestException.Message);
		}

		private static void InlineResultChosen(object sender, ChosenInlineResultEventArgs e)
		{
			var result = e.ChosenInlineResult;
			
			Console.WriteLine($"Sent a voice message by {UserInfo(result.From)}");
		}

		private static async void AnswerInlineQuery(
			object sender,
			InlineQueryEventArgs queryEventArgs)
		{
			var bot = sender as TelegramBotClient;
			var query = queryEventArgs.InlineQuery;

			try
			{
				var ok =
					await
					bot.AnswerInlineQueryAsync(
						query.Id,
						_queryResults.ToArray(),
						0);

				if (ok)
					Console.WriteLine(
						$"User {UserInfo(query.From)} choosing message to send.");
			}
			catch(Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);	
			}
		}

		private static string UserInfo(Telegram.Bot.Types.User user)
		{
			var result = user.Username;
			result += " (" + user.FirstName + " " + user.LastName + ")";
			return result;
		}
	}
}
