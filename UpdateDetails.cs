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
 * 
 * "Update Line Manager Details" page.
 * 
 * */

namespace VolunteeringApp
{
    [Activity(Label = "Update Details", Theme = "@style/White")]
    public class UpdateDetails : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.updateLM);

            //layout elements define.
            var hc = FindViewById<EditText>(Resource.Id.hc);
            var lm = FindViewById<EditText>(Resource.Id.lm);
            var upd = FindViewById<Button>(Resource.Id.upd);

            var errorText = FindViewById<TextView>(Resource.Id.errTxtLM);

            //post request button func.
            upd.Click += async (sender, e) =>
            {
                if (lm.Text != "" && hc.Text != "") {
                    await postUpdate(hc.Text, lm.Text);

                    errorText.Text = "Update request was sent successfully!";

                }
                else
                {
                    errorText.Text = "Please fill all the forms.";
                }

            };
        }

        //post line manager request func
        private async Task<JsonValue> postUpdate(String healthCond, String lineMan)
        {
          
            var values = new Dictionary<string, string>
            {
               { "line_manager", lineMan },
               { "med_details", healthCond },
            };

            //cookie adding and post request.
            var cookieContainer = new CookieContainer();
            Uri baseAddress = new Uri("URL/XXX/auth/register/submit");

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