using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

/**
 * Searching team challenges page.
 * */
namespace DurhamUniversityVolunteeringApp
{
    [Activity(Label = "Challenge Search", Theme = "@style/DurWhite")]
    public class Challenge : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.chalLay);
            Toolbar toolbar = (Toolbar)FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "Challenge Search";

            //define layout elements
            EditText et = FindViewById<EditText>(Resource.Id.getChal);

            LinearLayout layout = (LinearLayout)FindViewById<LinearLayout>(Resource.Id.linearLayout2);

            List<TextView> tvList = new List<TextView>();

            List<String> matched = getAllChallenges();


            if (matched != null)
            {
                for (var i = 0; i < matched.Count; i++)
                {
                    TextView texter = new TextView(this);
                    texter.Text = matched[i];

                    texter.SetTextColor(Android.Graphics.Color.Black);
                    layout.AddView(texter);
                    tvList.Add(texter);
                    texter.Click += (sender, e) =>
                    {

                        var chalReq = new Intent(this, typeof(postSignUp));
                        chalReq.PutExtra("FirstData", texter.Text);
                        chalReq.PutExtra("accessToken", Intent.GetStringExtra("accessToken"));
                        chalReq.PutExtra("refreshToken", Intent.GetStringExtra("refreshToken"));
                        chalReq.PutExtra("tokenExpires", Intent.GetStringExtra("tokenExpires"));
                        chalReq.PutExtra("user_name", Intent.GetStringExtra("user_name"));
                        chalReq.PutExtra("full_name", Intent.GetStringExtra("full_name"));
                        StartActivity(chalReq);
                    };

                }
            }else {
                TextView texter = new TextView(this);
                texter.Text = "There is no challenge available yet.";

                texter.SetTextColor(Android.Graphics.Color.Black);
                layout.AddView(texter);
            }


            var getChalbut = FindViewById<Button>(Resource.Id.gBut);
            getChalbut.Click += (sender, e) =>
            {
                for (var i = 0; i < matched.Count; i++)
                {

                    if (isMatched(et.Text, tvList[i].Text))
                    {
                        if (layout.IndexOfChild(tvList[i]) == -1)
                        {
                            layout.AddView(tvList[i]);
                        }
                    }
                    else
                    {
                        layout.RemoveView(tvList[i]);

                    }

                }

            };

        }

        //matching with regex for appropriate key/result func
        private bool isMatched(String filter, String tbFiltered)
        {

            Regex r = new Regex(filter, RegexOptions.IgnoreCase);
            Match chalReg = r.Match(tbFiltered);

            return chalReg.Success;

        }

        //gets all challenges to be filtered
        private List<String> getAllChallenges()
        {
            List<String> chalList = new List<string>();

            var request = (HttpWebRequest)HttpWebRequest.Create(new Uri(string.Format(@"https://amecentral.co.uk/sca/challenge/search?")));
            request.ContentType = "application /json";
            request.Method = "GET";
            Uri uri = new Uri("https://amecentral.co.uk/sca/challenge/search?");
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
                        String chal = "";
                        JToken o = JToken.Parse(content);
                        foreach (JToken item in o)
                        {
                            if (item["is_old"].ToString() == "0") { 
                            chal = item["title"].ToString();


                            Console.Out.WriteLine(chal);
                            chalList.Add(chal);
                            }
                        }
                        return chalList;
                    }
                }
            }
        }
    }
}
