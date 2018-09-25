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
 * Send volunteering form page.
 * */
namespace VolunteeringApp
{
    [Activity(Label = "Fill Volunteering Form", Theme = "@style/White")]
    public class SubmissionPage : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SubLay);

            //layour element declaration
            var orgNam = FindViewById<EditText>(Resource.Id.orgNam);
            var volDec = FindViewById<EditText>(Resource.Id.volDec);
            var noSesh =FindViewById<EditText>(Resource.Id.noSesh);
            var startD = FindViewById<DatePicker>(Resource.Id.startD);
            var endD = FindViewById<DatePicker>(Resource.Id.endD);
            var perSesh = FindViewById<EditText>(Resource.Id.perSesh);
            var errSubtext = FindViewById<TextView>(Resource.Id.errSubtext);
            var sendButton = FindViewById<Button>(Resource.Id.submitBut);
            var tb = FindViewById<ToggleButton>(Resource.Id.tb);
            var perTog = FindViewById<ToggleButton>(Resource.Id.perTog);

            //send details button
            sendButton.Click += async (sender, e) =>
            {
                String pDes = "0";
                if (perTog.Checked)
                    {
                        if (pDes == "0")
                        {
                            pDes = "1";
                        }
                        else
                        {
                            pDes = "0";
                        }

                    }

                //checks required fields or throw "please fill all required" error.
                if (orgNam.Text != "" && volDec.Text != "" && noSesh.Text != "" && perSesh.Text != "" && pDes=="1")
                {

                    
                    String wTime = "0";
                    
              

                        if (tb.Checked)
                        {
                            if (wTime == "0")
                            {
                                wTime = "1";
                            }
                            else
                            {
                                wTime = "0";
                            }

                        }


                    String stmont = "";
                    String stdy = "";
                    String edmont = "";
                    String eddy = "";

                    if (startD.DateTime.Month < 10)
                    {
                        stmont = "0" + startD.DateTime.Month;
                    }
                    else
                    {
                        stmont = ""+startD.DateTime.Month;
                    }

                    if (startD.DateTime.Day < 10)
                    {
                        stdy = "0" + startD.DateTime.Day;
                    }
                    else
                    {
                        stdy = "" + startD.DateTime.Day;
                    }


                    if (endD.DateTime.Month < 10)
                    {
                        edmont = "0" + endD.DateTime.Month;
                    }
                    else
                    {
                        edmont = ""+endD.DateTime.Month;
                    }

                    if (endD.DateTime.Day < 10)
                    {
                        eddy = "0" + endD.DateTime.Day;
                    }
                    else
                    {
                        eddy = "" + endD.DateTime.Day;
                    }
                    String sd = startD.DateTime.Year + "-"+stmont + "-" + stdy + " 14:00:00";
                    String ed = endD.DateTime.Year + "-" + edmont + "-" + eddy + " 14:00:00";

                    var values = new Dictionary<string, string>
                    {
                       { "user_id", Intent.GetStringExtra("user_name").ToString() },
                       { "organisation_name", orgNam.Text },
                       { "volunteering_description", volDec.Text },
                       { "frequency", noSesh.Text },
                       { "start_date", sd },
                       { "end_date", ed  },
                       { "hours_per_session", perSesh.Text },
                       { "during_work_time", wTime },
                       { "personal_declaration", pDes }
                    };

                    
                    await postHTTPVolForm(values);
                }
                else
                {
                    errSubtext.Text = "Please fill all of the fields!";
                }

            };
         
        }

        //post request to send the data
        private async Task<JsonValue> postHTTPVolForm(Dictionary<string, string> values)
        {
            
            Uri baseAddress = new Uri("URL/XXX/form/submit");
            //cookie handler
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

                Console.Out.WriteLine(responseString);
            }

            return "{successful:successful}";
        }
    }
}