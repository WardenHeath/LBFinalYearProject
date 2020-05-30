using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Android.Support.Design.Widget;

namespace OverwatchApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/LoginTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            EditText PlayerIDtext = FindViewById<EditText>(Resource.Id.BNetID);
            Button SubmitBtn = FindViewById<Button>(Resource.Id.SubmitBtn);
            CoordinatorLayout view = FindViewById<CoordinatorLayout>(Resource.Id.myCoordinatorLayout);
            try
            {
            string unknownUser = this.Intent.Extras.GetString("user_unknown");

            if (unknownUser == "Unknown user")
            {

                Toast.MakeText(this, "User was not found, player lookup tool unavaible or Profile is private", ToastLength.Short).Show();

            }
            }
            catch
            {

            }
           

            SubmitBtn.Click += (sender, e) =>
            {

                string BnetIDCom = PlayerIDtext.Text.ToString();

                if (BnetIDCom.Equals("")){
                    Toast.MakeText(this, "You did not enter a username", ToastLength.Short).Show();
                }
                else
                {
                    if (BnetIDCom.Contains('#'))
                    {
                        Toast.MakeText(this, "Please use a - instead of a #" , ToastLength.Short).Show();
                    }
                    else { 
                        var intent = new Intent(this, typeof(secondActivity));

                        var bundle = new Bundle();
                        bundle.PutString("Token", BnetIDCom);
                        intent.PutExtras(bundle);
                    
                        StartActivity(intent);
                    }
                }
                
                


               
            };
        }
    }
}