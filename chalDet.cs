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
 * Each past challenge name's detail page.
 */

namespace DurhamUniversityVolunteeringApp
{
    [Activity(Label = "Challenge Details", Theme = "@style/DurWhite")]//
    public class chalDet : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.chalDetLay);
            var label = FindViewById<TextView>(Resource.Id.cn); 
            label.Text = Intent.GetStringExtra("FirstData") ?? "Data not available";

            List<String> info = new List<String>();
            info = getInfo(Intent.GetStringExtra("user_name"));

            //define layout elements
            var volDec = FindViewById<TextView>(Resource.Id.volDec);
            var freq = FindViewById<TextView>(Resource.Id.freq);
            var stDat = FindViewById<TextView>(Resource.Id.stDat);
            var edDat = FindViewById<TextView>(Resource.Id.edDat);
            var hPs = FindViewById<TextView>(Resource.Id.hPs);
            var dWt = FindViewById<TextView>(Resource.Id.dWt);
            var pd = FindViewById<TextView>(Resource.Id.pd);

            volDec.Text = info[0];
            freq.Text = info[1];
            stDat.Text = info[2];
            edDat.Text = info[3];
            hPs.Text = info[4];
            if (info[5] == "1")
            {
                dWt.Text = "Yes";
            }
            else {
                dWt.Text = "No";
            }
            if (info[6] == "1")
            {
                pd.Text = "Yes";
            }
            else
            {
                pd.Text = "No";
            }
        
        }

        //gets challenge info
        private List<String> getInfo(String title)
        {
            List<String> result = new List<String>();

            var request = (HttpWebRequest)HttpWebRequest.Create(new Uri(string.Format(@"https://amecentral.co.uk:443/sca/form/search?user_id=" + title)));

            //encodes cookies
            Console.Out.WriteLine(title);
            request.ContentType = "application /json";
            request.Method = "GET";
            Uri uri = new Uri("https://amecentral.co.uk:443/sca/form/search?user_id=" + title);
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

                        foreach (JToken item in o)
                        {
                            
                            if (item["organisation_name"].ToString() == Intent.GetStringExtra("FirstData"))
                            {

                                String volDec = item["volunteering_description"].ToString();
                                String freq = item["frequency"].ToString();
                                String stDat = item["start_date"].ToString();
                                String edDat = item["end_date"].ToString();
                                String hPs = item["hours_per_session"].ToString();
                                String dWt = item["during_work_time"].ToString();
                                String pd = item["personal_declaration"].ToString();

                                result.Add(volDec);
                                result.Add(freq);
                                result.Add(stDat);
                                result.Add(edDat);
                                result.Add(hPs);
                                result.Add(dWt);
                                result.Add(pd);

                            }

                        }

                        return result;

                    }

                    
                }
            }
        }
    }
}