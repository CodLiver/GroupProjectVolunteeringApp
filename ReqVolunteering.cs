using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Net;
using System.Net.Http;
using System.Json;
using System.Threading.Tasks;


/**
 * Request volunteering time form page.
 * */
namespace VolunteeringApp
{
    [Activity(Label = "Volunteering Time Request", Theme = "@style/White")]
    public class ReqVolunteering : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.reqVolLay);

            //define layout elements
            var dp = FindViewById<DatePicker>(Resource.Id.dp);
            var desc = FindViewById<TextView>(Resource.Id.descr);
            var sendButton = FindViewById<Button>(Resource.Id.req);
            var errorText = FindViewById<TextView>(Resource.Id.errTxt);

            //to post the requests
            sendButton.Click += async (sender, e) =>
            {
                if (desc.Text!="") {
                    await postHTTPRequestTime(dp.DateTime, desc.Text);
                    errorText.Text = "E-mail was sent was successfully!";
                } else {
                    errorText.Text = "Please choose a Date and fill the Description!";
                }
                


            };
               
        }
        private async Task<JsonValue> postHTTPRequestTime(DateTime date,String desc)
        {
           
            var values = new Dictionary<string, string>
            {
               { "user_id", Intent.GetStringExtra("user_name").ToString() },
               { "name", Intent.GetStringExtra("full_name").ToString() },
               { "date", date.ToString() }
            };
            //cookie handler and post request
            Uri baseAddress = new Uri("URL/XXX/mail/send");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (HttpClient client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                var content = new FormUrlEncodedContent(values);
                cookieContainer.Add(baseAddress, new Cookie("access-token", WebUtility.UrlEncode(Intent.GetStringExtra("accessToken"))));
                cookieContainer.Add(baseAddress, new Cookie("refresh-token", WebUtility.UrlEncode(Intent.GetStringExtra("refreshToken"))));
                cookieContainer.Add(baseAddress, new Cookie("token-expires", WebUtility.UrlEncode(Intent.GetStringExtra("tokenExpires"))));
                cookieContainer.Add(baseAddress, new Cookie("user_name", WebUtility.UrlEncode(Intent.GetStringExtra("user_name"))));
                cookieContainer.Add(baseAddress, new Cookie("full_name", WebUtility.UrlEncode(Intent.GetStringExtra("full_name"))));

                var response = await client.PostAsync(baseAddress, content);

                var responseString = await response.Content.ReadAsStringAsync();


            }

            return "{successful:successful}";
        }
    }
}