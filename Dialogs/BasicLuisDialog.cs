using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [LuisModel("43138e69-323c-4a31-a72b-9705fc767ccd", "31315216ee3a4359abc7d72024f79545")]
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        //public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
        //    ConfigurationManager.AppSettings["LuisAppId"], 
        //    ConfigurationManager.AppSettings["LuisAPIKey"], 
        //    domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        //{
        //}

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("MostVoted")]
        public async Task MostVotedIntent(IDialogContext context, LuisResult result)
        {
            //await context.PostAsync("I don't know, look outside. If that doesn't help visit: http://buienradar.nl");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //Assuming that the api takes the user message as a query paramater
                    string RequestURI = "https://monitored-test.azurewebsites.net/api/LanPartyFinalApi/"+ result.Entities[0].Entity + "/mostvoted";
                    HttpResponseMessage responsemMsg = await client.GetAsync(RequestURI);
                    if (responsemMsg.IsSuccessStatusCode)
                    {
                        var apiResponse = await responsemMsg.Content.ReadAsStringAsync();
                        dynamic date = JObject.Parse(apiResponse);
                        var dateTimeStart = date.dateTimeStart;
                        var dateTimeFinish = date.dateTimeFinish;
                        var votedCount = date.votedCount;


                        //Post the API response to bot again
                        await context.PostAsync($"The most voted date is: Startdate: {dateTimeStart}. Finishdate: {dateTimeFinish}. {votedCount} people voted for this date.");

                    }
                }
            }
            catch (Exception ex)
            {
                await context.PostAsync($"{ex} and the entity is: {result.Entities[0].Entity}");
            }
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("NextLan")]
        public async Task NextLanIntent(IDialogContext context, LuisResult result)
        {
            //await context.PostAsync("I don't know, look outside. If that doesn't help visit: http://buienradar.nl");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //Assuming that the api takes the user message as a query paramater
                    string RequestURI = "https://monitored-test.azurewebsites.net/api/LanPartyFinalApi/next";
                    HttpResponseMessage responsemMsg = await client.GetAsync(RequestURI);
                    if (responsemMsg.IsSuccessStatusCode)
                    {
                        var apiResponse = await responsemMsg.Content.ReadAsStringAsync();
                        dynamic lan = JObject.Parse(apiResponse);
                        DateTime dateTimeStart = lan.lanPartyFinalStartDate;
                        DateTime dateTimeFinish = lan.lanPartyFinalFinishDate;
                        var lanPartyName = lan.lanPartyName;
                        var lanPartyPlace = lan.lanPartyPlace;

                        var datestart = dateTimeStart.Date.ToString("dd'-'MM'-'yyyy"); 
                        var datefinish = dateTimeFinish.Date.ToString("dd'-'MM'-'yyyy");
                        var timestarthour = dateTimeStart.Hour;
                        var timestartminute = dateTimeStart.Minute;
                        var timefinishhour = dateTimeFinish.Hour;
                        var timefinishminute = dateTimeFinish.Minute;
                        DateTime now = DateTime.Today;
                        int leftDate = (dateTimeStart - now).Days; 


                        //Post the API response to bot again
                        await context.PostAsync($"The next lan {lanPartyName} starts on: {datestart} at {timestarthour}:{timestartminute} o'clock. And finishes at {datefinish} at {timefinishhour}:{timefinishminute}.This lan will be held in {lanPartyPlace}. Days until lan: {leftDate}");

                    }
                }
            }
            catch (Exception ex)
            {
                await context.PostAsync($"{ex} and the entity is: {result.Entities[0].Entity}");
            }
            context.Wait(this.MessageReceived);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Gretting" with the name of your newly created intent in the following handler
        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Cancel")]
        public async Task CancelIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Help")]
        public async Task HelpIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result) 
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");
            context.Wait(MessageReceived);
        }
    }
}