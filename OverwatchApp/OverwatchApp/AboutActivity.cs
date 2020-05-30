using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OverwatchApp
{
    [Activity(Label = "AboutActivity", Theme = "@style/AppThemeCustom")]
    public class AboutActivity : Activity
    {
        /// <summary>
        /// Sets the content view tot he about layout
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.AboutLayout);
        }
    }
}