using Android.App;
using Android.OS;
using System.Net;
using Android.Webkit;


/**
 * first entry page, redirects the user to office 365 page.
 * */
namespace VolunteeringApp
{
    [Activity(Label = "XX", MainLauncher = true, Theme = "@style/White", Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {       

        private static CookieContainer _cookieContainer = new System.Net.CookieContainer();
        private static CookieManager _cookieManager = Android.Webkit.CookieManager.Instance;

        protected override void OnCreate(Bundle savedInstanceState)
        { 
            base.OnCreate(savedInstanceState);
            WebView wv = new WebView(this);
            SetContentView(wv);
            wv.LoadUrl("URL/XXX/auth/sign-in");
            wv.Settings.JavaScriptEnabled = true;
            var client = new WebViewClient();
            wv.SetWebViewClient(new WvClient(this, wv));
            
        }

    }
}

