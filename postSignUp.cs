using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

/*
 * Team Challenge signups page.
 * */

namespace VolunteeringApp
{
    [Activity(Label = "Submit Your Signups Here", Theme = "@style/White")]
    public class postSignUp : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.postSgnup);
            var label = FindViewById<TextView>(Resource.Id.tv);

            //define layout elements
            label.Text = Intent.GetStringExtra("FirstData") ?? "Data not available";
            List<String> info = new List<String>();
            info = getInfo(label.Text);

            var desc= FindViewById<TextView>(Resource.Id.desc);
            var date= FindViewById<TextView>(Resource.Id.dat);
            var loc= FindViewById<TextView>(Resource.Id.loc);

            desc.Text = "Description: "+info[0];
            date.Text = "Date: " + info[1];
            loc.Text = "Location: " + info[2];

            var sendButton = FindViewById<Button>(Resource.Id.but);

            sendButton.Click += async (sender, e) =>
            {

                var photoAl = FindViewById<ToggleButton>(Resource.Id.photoAl);
                var dataPr = FindViewById<ToggleButton>(Resource.Id.dataPr);
                var emConNam = FindViewById<EditText>(Resource.Id.emConNam);
                var emConPh = FindViewById<EditText>(Resource.Id.emConPh);
                var chalID = info[3];

                
                var errorText = FindViewById<TextView>(Resource.Id.errorText);

                String dataPrRes = "0";
                if (dataPr.Checked)
                {
                    if (dataPrRes == "0")
                    {
                        dataPrRes = "1";
                    }
                    else
                    {
                        dataPrRes = "0";
                    }

                }

                if (emConNam.Text != "" && emConPh.Text != "" && dataPrRes=="1")
                {
                    
                    String photoAlRes= "0";
                    
                    if (photoAl.Checked)
                    {
                        if (photoAlRes == "0")
                        {
                            photoAlRes = "1";
                        }
                        else
                        {
                            photoAlRes = "0";
                        }

                    }

                    await postHTTPSignUp(chalID, photoAlRes, dataPrRes, emConNam.Text, emConPh.Text);

                    errorText.Text = "Successfully Signup was sent!";
                }
                else {
                    
                    errorText.Text = "Please fill all required fields.";
                }
                
            };

           
        }

        //get request to bring challenge details to show
        private List<String> getInfo(String title) {
            List<String> result = new List<String>();

            var request = (HttpWebRequest)HttpWebRequest.Create(new Uri(string.Format(@"URL/XXX/challenge/search?title=" + title)));
            request.ContentType = "application /json";
            request.Method = "GET";
            Uri uri = new Uri("URL/XXX/challenge/search?title=" + title);
            Cookie cookie = new Cookie("access-token", WebUtility.UrlEncode(Intent.GetStringExtra("accessToken")));
            Cookie cookie1 = new Cookie("refresh-token", WebUtility.UrlEncode(Intent.GetStringExtra("refreshToken")));
            Cookie cookie2 = new Cookie("token-expires", WebUtility.UrlEncode(Intent.GetStringExtra("tokenExpires")));
            Cookie cookie3 = new Cookie("user_name", WebUtility.UrlEncode(Intent.GetStringExtra("user_name")));
            Cookie cookie4 = new Cookie("full_name", WebUtility.UrlEncode(Intent.GetStringExtra("full_name")));
            //cookie handler
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(uri, cookie);
            request.CookieContainer.Add(uri, cookie1);
            request.CookieContainer.Add(uri, cookie2);
            request.CookieContainer.Add(uri, cookie3);
            request.CookieContainer.Add(uri, cookie4);
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
            Console.Out.WriteLine(request.GetResponse());

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {//
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        Console.Out.WriteLine("Response contained empty body...");
                        return null;
                    }
                    else
                    {
                        JToken o = JToken.Parse(content);

                        
                        String description= o[0]["description"].ToString();
                        String date = o[0]["date"].ToString();
                        String location = o[0]["location"].ToString();
                        String chalID = o[0]["challenge_id"].ToString();

                        result.Add(description);
                        result.Add(date);
                        result.Add(location);
                        result.Add(chalID);

                        return result;

                    }

                   
                }
            }
        }

        //post request for challenge signups
        private async Task<JsonValue> postHTTPSignUp(String chalID, String photoAlRes, String dataPr, String emConNam, String emConPh)
        {

            var values = new Dictionary<string, string>
            {
               { "user_id", Intent.GetStringExtra("user_name").ToString() },
               { "challenge_id", chalID },
               { "photography", photoAlRes },
               { "data_protection", dataPr },
               { "emergency_contact", emConNam },
               { "emergency_number", emConPh }
            };            

            var cookieContainer = new CookieContainer();
            Uri baseAddress = new Uri("URL/XXX/challenge/sign-up/submit");

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
