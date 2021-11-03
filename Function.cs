using System.Net.Mime;
using System.Threading.Tasks.Dataflow;
using System.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using bot.Entity;
using bot.HttpClients;
using bot.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using bot.Dto.V2;
using System.IO;
using SkiaSharp;
using Topten.RichTextKit;

namespace bot
{
    public class Function
    {
        public static async void TimesWriter(ITelegramBotClient client, Message message, string _language, IStorageService _storage, ICacheService _cache)
        {
            var res = await _storage.GetUserAsync(message.Chat.Id);
            var result = await _cache.GetOrUpdatePrayerTimeAsync(res.ChatId, res.Longitude, res.Latitude);
            var times = result.prayerTime; 

                await client.SendPhotoAsync(
                chatId: message.Chat.Id,
                getImageFile(times, message));

                await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: @$"
                {AladhanClient.GetDateToday(res.Longitude, res.Latitude)}",
                parseMode: ParseMode.Markdown,
                replyMarkup: MessageBuilder.MenuShow(_language));
        }

        public static Stream getImageFile(Models.PrayerTime times, Message message)
        {
            var text = getTimeString(times, message);
            using (var surface = SKSurface.Create(new SKImageInfo(1080, 1080)))
            {
                Draw(surface, text, message);
                
                using var image = surface.Snapshot();
                using var data = image.Encode(SKEncodedImageFormat.Png, 500);
                
                return data.AsStream();
            }
        }

        public static void Draw(SKSurface surface, string text, Message message)
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.ForestGreen);

            // Find the canvas bounds
            var canvasBounds = canvas.DeviceClipBounds;
            
            // Create the text block
            var tb = new TextBlock();

            // Configure layout properties
            tb.MaxWidth = canvasBounds.Width * 1f;
            tb.MaxHeight = canvasBounds.Height * 1f;
            tb.Alignment = TextAlignment.Left;

            var style = new Style()
            {
                FontFamily = "Bahnschrift",
                TextColor = SKColors.White,
                FontSize = 90
            };

            // Add text to the text block
            tb.AddText(text, style);

            // Paint the text block
            tb.Paint(canvas, new SKPoint(canvasBounds.Width * 0.19f, canvasBounds.Height * 0.17f));
        }

        public static string getTimeString(Models.PrayerTime times, Message message)
        {

            if(message.Text == "Today")
            {
                var Text = $"🌙 Fajr : {times.Fajr}\n";
                var Text1 = $"🔆 Sunrise : {times.Sunrise}\n";
                var Text2 = $"🔆 Dhuhr : {times.Dhuhr}\n";
                var Text3 = $"🔆 Asr : {times.Asr}\n";
                var Text4 = $"🌙 Maghrib : {times.Maghrib}\n";
                var Text5 = $"🌙 Isha : {times.Isha}";
                return Text + Text1 + Text2 + Text3 + Text4 + Text5;
            }

            // if(message.Text == "Today")
            // {
            //     var Text = $"Fajr        Sunrise      Dhuhr\n";
            //     var Text1 = $"{times.Fajr}      {times.Sunrise}          {times.Dhuhr}\n\n";
            //     var Text2 = $"Asr         Maghrib      Isha\n";
            //     var Text3 = $"{times.Asr}       {times.Maghrib}            {times.Isha}\n\n\n";
            //     return Text + Text1 + Text2 + Text3;
            // }

            if(message.Text == "Сегодня")
            {
                var Text = $"🌙 Фаджр : {times.Fajr}\n";
                var Text1 = $"🔆 Восход : {times.Sunrise}\n";
                var Text2 = $"🔆 Зухр : {times.Dhuhr}\n";
                var Text3 = $"🔆 Аср : {times.Asr}\n";
                var Text4 = $"🌙 Магриб : {times.Maghrib}\n";
                var Text5 = $"🌙 Иша : {times.Isha}";
                return Text + Text1 + Text2 + Text3 + Text4 + Text5;
            }

            // if(message.Text == "Сегодня")
            // {
            //     var Text = $"Фаджр     Восход      Зухр\n";
            //     var Text1 = $"{times.Fajr}        {times.Sunrise}          {times.Dhuhr}\n\n";
            //     var Text2 = $"Аср          Магриб       Иша\n";
            //     var Text3 = $"{times.Asr}         {times.Maghrib}            {times.Isha}\n\n\n";
            //     return Text + Text1 + Text2 + Text3;
            // }

            if(message.Text == "Bugun")
            {
                var Text = $"🌙 Bomdod : {times.Fajr}\n";
                var Text1 = $"🔆 Quyosh : {times.Sunrise}\n";
                var Text2 = $"🔆 Peshin : {times.Dhuhr}\n";
                var Text3 = $"🔆 Asr : {times.Asr}\n";
                var Text4 = $"🌙 Shom : {times.Maghrib}\n";
                var Text5 = $"🌙 Xufton : {times.Isha}";
                return Text + Text1 + Text2 + Text3 + Text4 + Text5;
            }

            // if(message.Text == "Bugun")
            // {
            //     var Text = $"Bomdod     Quyosh      Peshin\n";
            //     var Text1 = $"{times.Fajr}          {times.Sunrise}         {times.Dhuhr}\n\n";
            //     var Text2 = $"Asr             Shom       Xufton\n";
            //     var Text3 = $"{times.Asr}           {times.Maghrib}         {times.Isha}\n\n\n";
            //     return Text + Text1 + Text2 + Text3;
            // }

            return string.Empty;
        }

        public static async void TimesWriterTomorrow(ITelegramBotClient client, Message message, string _language, IStorageService _storage, ICacheService _cache)
        {
            var res = await _storage.GetUserAsync(message.Chat.Id);
            var result = await _cache.GetOrUpdatePrayerTimeAsyncTomorrow(res.ChatId, res.Longitude, res.Latitude, 1);
            var times = result.prayerTime; 

                await client.SendPhotoAsync(
                chatId: message.Chat.Id,
                getImageFile1(times, message),
                parseMode: ParseMode.Markdown,
                replyMarkup: MessageBuilder.MenuShow(_language));

                await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: @$"
                {AladhanClient.GetDateTomorrow(res.Longitude, res.Latitude)}",
                parseMode: ParseMode.Markdown,
                replyMarkup: MessageBuilder.MenuShow(_language));
        }

        public static Stream getImageFile1(Models.PrayerTime times, Message message)
        {
            var text = getTimeString1(times, message);
            using (var surface = SKSurface.Create(new SKImageInfo(1080, 1080)))
            {
                Draw1(surface, text, message);
                
                using var image = surface.Snapshot();
                using var data = image.Encode(SKEncodedImageFormat.Png, 500);
                
                return data.AsStream();
            }
        }

        public static void Draw1(SKSurface surface, string text, Message message)
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.ForestGreen);

            // Find the canvas bounds
            var canvasBounds = canvas.DeviceClipBounds;
            
            // Create the text block
            var tb = new TextBlock();

            // Configure layout properties
            tb.MaxWidth = canvasBounds.Width * 1f;
            tb.MaxHeight = canvasBounds.Height * 1f;
            tb.Alignment = TextAlignment.Left;

            var style = new Style()
            {
                FontFamily = "Bahnschrift",
                TextColor = SKColors.White,
                FontSize = 90
            };

            // Add text to the text block
            tb.AddText(text, style);

            // Paint the text block
            tb.Paint(canvas, new SKPoint(canvasBounds.Width * 0.19f, canvasBounds.Height * 0.17f));
        }

        public static string getTimeString1(Models.PrayerTime times, Message message)
        {

            if(message.Text == "Tomorrow")
            {
                var Text = $"🌙 Fajr : {times.Fajr}\n";
                var Text1 = $"🔆 Sunrise : {times.Sunrise}\n";
                var Text2 = $"🔆 Dhuhr : {times.Dhuhr}\n";
                var Text3 = $"🔆 Asr : {times.Asr}\n";
                var Text4 = $"🌙 Maghrib : {times.Maghrib}\n";
                var Text5 = $"🌙 Isha : {times.Isha}";
                return Text + Text1 + Text2 + Text3 + Text4 + Text5;
            }

            // if(message.Text == "Today")
            // {
            //     var Text = $"Fajr        Sunrise      Dhuhr\n";
            //     var Text1 = $"{times.Fajr}      {times.Sunrise}          {times.Dhuhr}\n\n";
            //     var Text2 = $"Asr         Maghrib      Isha\n";
            //     var Text3 = $"{times.Asr}       {times.Maghrib}            {times.Isha}\n\n\n";
            //     return Text + Text1 + Text2 + Text3;
            // }

            if(message.Text == "Завтра")
            {
                var Text = $"🌙 Фаджр : {times.Fajr}\n";
                var Text1 = $"🔆 Восход : {times.Sunrise}\n";
                var Text2 = $"🔆 Зухр : {times.Dhuhr}\n";
                var Text3 = $"🔆 Аср : {times.Asr}\n";
                var Text4 = $"🌙 Магриб : {times.Maghrib}\n";
                var Text5 = $"🌙 Иша : {times.Isha}";
                return Text + Text1 + Text2 + Text3 + Text4 + Text5;
            }

            // if(message.Text == "Сегодня")
            // {
            //     var Text = $"Фаджр     Восход      Зухр\n";
            //     var Text1 = $"{times.Fajr}        {times.Sunrise}          {times.Dhuhr}\n\n";
            //     var Text2 = $"Аср          Магриб       Иша\n";
            //     var Text3 = $"{times.Asr}         {times.Maghrib}            {times.Isha}\n\n\n";
            //     return Text + Text1 + Text2 + Text3;
            // }

            if(message.Text == "Ertangi")
            {
                var Text = $"🌙 Bomdod : {times.Fajr}\n";
                var Text1 = $"🔆 Quyosh : {times.Sunrise}\n";
                var Text2 = $"🔆 Peshin : {times.Dhuhr}\n";
                var Text3 = $"🔆 Asr : {times.Asr}\n";
                var Text4 = $"🌙 Shom : {times.Maghrib}\n";
                var Text5 = $"🌙 Xufton : {times.Isha}";
                return Text + Text1 + Text2 + Text3 + Text4 + Text5;
            }

            // if(message.Text == "Bugun")
            // {
            //     var Text = $"Bomdod     Quyosh      Peshin\n";
            //     var Text1 = $"{times.Fajr}          {times.Sunrise}         {times.Dhuhr}\n\n";
            //     var Text2 = $"Asr             Shom       Xufton\n";
            //     var Text3 = $"{times.Asr}           {times.Maghrib}         {times.Isha}\n\n\n";
            //     return Text + Text1 + Text2 + Text3;
            // }

            return string.Empty;
        }
    }
}