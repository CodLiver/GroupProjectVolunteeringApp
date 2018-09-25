using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

/**
 * User profile page
 * */
namespace VolunteeringApp
{
    [Activity(Label = "Profile", Theme = "@style/White")]
    public class HomeScreen : Activity
    {
        private static CookieContainer _cookieContainer = new System.Net.CookieContainer();
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);
        
            SetContentView(Resource.Layout.NewF);
            //define layout elements
            TextView dispN = FindViewById<TextView>(Resource.Id.dispN);
            dispN.Text = Intent.GetStringExtra("full_name");

            Toolbar toolbar = (Toolbar)FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "Profile";
            

            //main buttons to redirect user to appropriate pages.
            var addEnts = FindViewById<Button>(Resource.Id.adEnt);
            addEnts.Click += (sender, e) =>
            {
   

                var chalReq = new Intent(this, typeof(SubmissionPage));
                chalReq.PutExtra("accessToken", Intent.GetStringExtra("accessToken"));
                chalReq.PutExtra("refreshToken", Intent.GetStringExtra("refreshToken"));
                chalReq.PutExtra("tokenExpires", Intent.GetStringExtra("tokenExpires"));
                chalReq.PutExtra("user_name", Intent.GetStringExtra("user_name"));
                chalReq.PutExtra("full_name", Intent.GetStringExtra("full_name"));

                StartActivity(chalReq);
            };
            var reqVolBut = FindViewById<Button>(Resource.Id.reqVol);
            reqVolBut.Click += (sender, e) =>
            {

                var chalReq = new Intent(this, typeof(ReqVolunteering));
                chalReq.PutExtra("accessToken", Intent.GetStringExtra("accessToken"));
                chalReq.PutExtra("refreshToken", Intent.GetStringExtra("refreshToken"));
                chalReq.PutExtra("tokenExpires", Intent.GetStringExtra("tokenExpires"));
                chalReq.PutExtra("user_name", Intent.GetStringExtra("user_name"));
                chalReq.PutExtra("full_name", Intent.GetStringExtra("full_name"));

                StartActivity(chalReq);
            };
            var chal = FindViewById<Button>(Resource.Id.chalBut);
            chal.Click += (sender, e) =>
            {
                
                var chalReq = new Intent(this, typeof(Challenge));
                    chalReq.PutExtra("accessToken", Intent.GetStringExtra("accessToken"));
                    chalReq.PutExtra("refreshToken", Intent.GetStringExtra("refreshToken"));
                    chalReq.PutExtra("tokenExpires", Intent.GetStringExtra("tokenExpires"));
                    chalReq.PutExtra("user_name", Intent.GetStringExtra("user_name"));
                    chalReq.PutExtra("full_name", Intent.GetStringExtra("full_name"));

                StartActivity(chalReq);

            };
            var update = FindViewById<Button>(Resource.Id.update);
            update.Click += (sender, e) =>
            {
            

                var chalReq = new Intent(this, typeof(UpdateDetails));
                chalReq.PutExtra("accessToken", Intent.GetStringExtra("accessToken"));
                chalReq.PutExtra("refreshToken", Intent.GetStringExtra("refreshToken"));
                chalReq.PutExtra("tokenExpires", Intent.GetStringExtra("tokenExpires"));
                chalReq.PutExtra("user_name", Intent.GetStringExtra("user_name"));
                chalReq.PutExtra("full_name", Intent.GetStringExtra("full_name"));

                StartActivity(chalReq);
            };

     


            var totH = FindViewById<TextView>(Resource.Id.toth);

            totH.Text = getTotalHours(Intent.GetStringExtra("user_name"));

            LinearLayout layout = (LinearLayout)FindViewById<LinearLayout>(Resource.Id.layout2);


            List<String> expList = getExperiences(Intent.GetStringExtra("user_name"));

            for (var i = 0; i < expList.Count; i++)
            {

                TextView texter = new TextView(this);
                texter.Text = expList[i];
                texter.SetTextColor(Android.Graphics.Color.Black);
                layout.AddView(texter);

                
                texter.Click += (sender1, e1) =>
                {

                    var chalReq = new Intent(this, typeof(chalDet));
                    chalReq.PutExtra("FirstData", texter.Text);
                    chalReq.PutExtra("accessToken", Intent.GetStringExtra("accessToken"));
                    chalReq.PutExtra("refreshToken", Intent.GetStringExtra("refreshToken"));
                    chalReq.PutExtra("tokenExpires", Intent.GetStringExtra("tokenExpires"));
                    chalReq.PutExtra("user_name", Intent.GetStringExtra("user_name"));
                    chalReq.PutExtra("full_name", Intent.GetStringExtra("full_name"));
                    StartActivity(chalReq);
                    
                };
            }
        }

        //get request for ILM hours
        private String getTotalHours(String user_id)
        {
            
          
            var request = (HttpWebRequest)HttpWebRequest.Create(new Uri(string.Format(@"URL/XXX/form/get/total_hours?user_id=" + user_id)));//et.Text));/10.245.101.218
            request.ContentType = "application /json";
            request.Method = "GET";
      
            Uri uri = new Uri("URL/XXX/form/get/total_hours?user_id=" + user_id);
            Cookie cookie = new Cookie("access-token", WebUtility.UrlEncode(Intent.GetStringExtra("accessToken")));
            Cookie cookie1 = new Cookie("refresh-token", WebUtility.UrlEncode(Intent.GetStringExtra("refreshToken")));
            Cookie cookie2 = new Cookie("token-expires", WebUtility.UrlEncode(Intent.GetStringExtra("tokenExpires")));
            Cookie cookie3 = new Cookie("user_name", WebUtility.UrlEncode(Intent.GetStringExtra("user_name")));
            Cookie cookie4 = new Cookie("full_name", WebUtility.UrlEncode(Intent.GetStringExtra("full_name")));

            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(uri, cookie);
            request.CookieContainer.Add(uri, cookie1);
            request.CookieContainer.Add(uri, cookie2);
            request.CookieContainer.Add(uri, cookie3);
            request.CookieContainer.Add(uri, cookie4);
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        Console.Out.WriteLine("Response contained empty body...");
                        return "Response contained empty body...";
                    }
                    else
                    {
                        JToken o = JToken.Parse(content);
                        o = o["volunteering_hours"];
                        String res = o.ToString();

                        return "Out of 35 hours: " + res + " hours completed!";
                    }

                }
            }

        }

        //get past volunteerings
        private List<String> getExperiences(String user_id)
        {
            List<String> totalExp = new List<String>();

            var request = (HttpWebRequest)HttpWebRequest.Create(new Uri(string.Format(@"URL/XXX/form/search?user_id=" + user_id)));
            request.ContentType = "application /json";
            request.Method = "GET";
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;

            Uri uri = new Uri("URL/XXX/form/search?user_id=" + user_id);
            Cookie cookie = new Cookie("access-token", WebUtility.UrlEncode(Intent.GetStringExtra("accessToken")));
            Cookie cookie1 = new Cookie("refresh-token", WebUtility.UrlEncode(Intent.GetStringExtra("refreshToken")));
            Cookie cookie2 = new Cookie("token-expires", WebUtility.UrlEncode(Intent.GetStringExtra("tokenExpires")));
            Cookie cookie3 = new Cookie("user_name", WebUtility.UrlEncode(Intent.GetStringExtra("user_name")));
            Cookie cookie4 = new Cookie("full_name", WebUtility.UrlEncode(Intent.GetStringExtra("full_name")));

            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(uri, cookie);
            request.CookieContainer.Add(uri, cookie1);
            request.CookieContainer.Add(uri, cookie2);
            request.CookieContainer.Add(uri, cookie3);
            request.CookieContainer.Add(uri, cookie4);

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
                        foreach (JToken item in o)
                        {
                            totalExp.Add(item["organisation_name"].ToString());

                        }

                        return totalExp;
                    }

                }
            }

        }
    }

}