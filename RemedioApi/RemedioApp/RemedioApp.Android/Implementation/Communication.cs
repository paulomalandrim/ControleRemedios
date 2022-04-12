using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RemedioApp.Interfaces;
using RemedioApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(RemedioApp.Droid.Implementation.Communication))]
namespace RemedioApp.Droid.Implementation
{
    public class Communication : ICommunication
    {
        public async Task<ApiReturn> GetMethod(string url)
        {
            ApiReturn ret = new ApiReturn();

            try
            {
                using (var handle = new HttpClientHandler())
                {
                    HttpClient client = new HttpClient(handle);
                    Uri uri = new Uri(url);


                    HttpResponseMessage response = await client.GetAsync(uri);

                    ret.JsonObject = await response.Content.ReadAsStringAsync();

                    ret.Success = true;
                }
            }
            catch (Exception ex)
            {
                ret.Success = false;
            }

            return ret;
        }

        public async Task<ApiReturn> PostMethod(string url, string json, Dictionary<string, string> header)
        {
            ApiReturn ret = new ApiReturn();

            try
            {
                using (var handle = new HttpClientHandler())
                {

                    HttpClient client = new HttpClient(handle);
                    Uri uri = new Uri(url);

                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    if (header != null)
                    {
                        foreach (var item in header)
                        {
                            client.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }

                    HttpResponseMessage response = await client.PostAsync(uri, content);

                    ret.JsonObject = await response.Content.ReadAsStringAsync();

                    response.EnsureSuccessStatusCode();

                    ret.Success = true;
                }
            }
            catch (Exception ex)
            {
                ret.Success = false;
            }

            return ret;
        }
    }
}