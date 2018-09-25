using System;
using Android.App;
using Android.Content;
using System.Net;
using Android.Webkit;

/**
 * 
 * New WebViewClient class to handle page redirections, get cookies and based on success redirect the user to main page.
 * 
 * */
namespace VolunteeringApp
{
    public class WvClient : WebViewClient
    {
        
        public Activity mActivity;
        //get cookie manager
        private static CookieContainer _cookieContainer = new System.Net.CookieContainer();
        private static CookieManager _cookieManager = Android.Webkit.CookieManager.Instance;

        public WvClient(Activity mActivity, WebView wv)
        {
            this.mActivity = mActivity;
            
        }
        //on page finished to get cookies and redirect to main page.
        public override void OnPageFinished(WebView wv, String url)
        {
            
            if (wv.Url == "URL/XXX/auth/app_login")
            {
                String cookies = _cookieManager.GetCookie(wv.Url);
                String[] cookieArr = cookies.Split(";".ToCharArray());
                String[] keyArr = { "accessToken", "refreshToken", "tokenExpires", "user_name", "full_name" };

                var toChal = new Intent(mActivity, typeof(HomeScreen));

                for (var x = 0; x < cookieArr.Length; x++)
                {
                    toChal.PutExtra(keyArr[x], cookieArr[x].Split("=".ToCharArray())[1]);
                }
                mActivity.StartActivity(toChal);
            }
            base.OnPageFinished(wv, url);
        }
        //if error
        public override void OnReceivedSslError(WebView view, SslErrorHandler handler, Android.Net.Http.SslError er)
        {
            handler.Proceed();
        }
    }
}