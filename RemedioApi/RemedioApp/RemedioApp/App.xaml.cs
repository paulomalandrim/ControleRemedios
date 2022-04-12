using Newtonsoft.Json;
using Plugin.FirebasePushNotification;
using RemedioApp.Interfaces;
using RemedioApp.Model;
using System;
using System.Net.Http;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemedioApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            CrossFirebasePushNotification.Current.Subscribe("all");
            CrossFirebasePushNotification.Current.OnTokenRefresh += OnTokenRefresh;


            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private async void OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {
            try
            {
                RequestSendToken sendToken = new RequestSendToken
                {
                    FcmToken = e.Token,
                };

                string json = JsonConvert.SerializeObject(sendToken);

                string url = "https://remediounisal.wsilveirait.com.br/api/remedioapi/salvartoken/";
                ApiReturn apiReturn = await DependencyService.Get<ICommunication>()
                        .PostMethod(url, json, null);

                if (apiReturn != null)
                {
                    string intReturned = apiReturn.JsonObject;

                    if (apiReturn.Success == true && intReturned == "1")
                    {
                        await SecureStorage.SetAsync("NotificationToken", e.Token);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
