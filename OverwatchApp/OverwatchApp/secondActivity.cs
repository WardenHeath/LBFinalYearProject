using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using HtmlAgilityPack;
using Java.Lang;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using Environment = System.Environment;
using String = System.String;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace OverwatchApp
{




    [Activity(Label = "Home", Theme = "@style/AppThemeCustom")]
    
    public class secondActivity : AppCompatActivity
    {
        string[] GameStatArray = new string[4];
        string[] AssistStatArray = new string[4];
        string[] CombatStatArray = new string[13];
        string[] averageStatArray = new string[11];
        string[] matchawardStatArray = new string[5];




        private TextView Mtextview;
        private SupportToolbar mToolbar;
        private MyActionBarDrawerToggle mDrawerToggle;
        private DrawerLayout mDrawerLayout;
        //private ListView mleftDrawer;
        private ArrayAdapter mLeftAdapter;
        private List<string> mLeftDataSet;
        private ListView GameStatlistView;
        private ListView AssiststatListView;
        private ListView CombatstatListView;
        private ListView AvgstatListView;
        private ListView MatchRewardstatListView;
        private Button CoachButton;
        private Button aboutButton;
        private Button performanceButton;
        private ChartView Playerstats;
        private int WinCondition;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            
            SetContentView(Resource.Layout.HomeLayout);
            //gathers Battlenet id from other activity
            String BnetID = this.Intent.Extras.GetString("Token");
            //gathering all of the different fields from activity layout
            Mtextview = FindViewById<TextView>(Resource.Id.textView1);
            CoachButton = FindViewById<Button>(Resource.Id.CoachBtn);
            aboutButton = FindViewById<Button>(Resource.Id.AboutBtn);
            //performanceButton = FindViewById<Button>(Resource.Id.PPerBtn);
            mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_Layout);
            //mleftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
            GameStatlistView = FindViewById<ListView>(Resource.Id.GamestatListView);
            AssiststatListView = FindViewById<ListView>(Resource.Id.AssiststatListView);
            CombatstatListView = FindViewById<ListView>(Resource.Id.CombatstatListView);
            AvgstatListView = FindViewById<ListView>(Resource.Id.AvgstatListView);
            MatchRewardstatListView = FindViewById<ListView>(Resource.Id.MatchRewardstatListView);
            Playerstats = FindViewById<ChartView>(Resource.Id.chartView);
            
            //sets up the navigation bar
            SetSupportActionBar(mToolbar);
           
            mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
            //mleftDrawer.Adapter = mLeftAdapter;


            mDrawerToggle = new MyActionBarDrawerToggle(
                this,                               //host activity
                mDrawerLayout,                      //drawer layout
                Resource.String.openDrawer,         //oepned message
                Resource.String.closeDrawer         //closed message
                );
            mDrawerLayout.AddDrawerListener(mDrawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            mDrawerToggle.SyncState();

            //toggles the drawer message
            if (savedInstanceState != null)
            {
                if (savedInstanceState.GetString("DrawerState") == "opened")
                {
                    SupportActionBar.SetTitle(Resource.String.openDrawer);
                }
                else
                {
                    SupportActionBar.SetTitle(Resource.String.closeDrawer);
                }
            }
            else
            {
                SupportActionBar.SetTitle(Resource.String.closeDrawer);
            }
            //gathers the data for the statistics
            GethtmlASync(BnetID, GameStatlistView, AssiststatListView, CombatstatListView, AvgstatListView, MatchRewardstatListView, Playerstats);

            //coach button event listner 
            CoachButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(Coach));
                var bundle = new Bundle();
                bundle.PutString("Token", BnetID);
                intent.PutExtras(bundle);
                StartActivity(intent);
            };
            //about button lsitner
            aboutButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(AboutActivity));
                var bundle = new Bundle();
                bundle.PutString("Token", BnetID);
                intent.PutExtras(bundle);
                StartActivity(intent);
            };

            //aboutButton.Click += (sender, e) =>
            //{
             //   var intent = new Intent(this, typeof(Performance));
            //    var bundle = new Bundle();
            //    bundle.PutString("Token", BnetID);
           //     intent.PutExtras(bundle);
           //     StartActivity(intent);
           // };


        }
        int PreGameWins;
        /// <summary>
        /// This is the main driving force of the program, it is where the webscraping takes place. all of the varibles that are passed in are the listviews.
        /// </summary>
        /// <param name="bnetID">battle net ID</param>
        /// <param name="l1">Listview container</param>
        /// <param name="l2">Listview container</param>
        /// <param name="l3">Listview container</param>
        /// <param name="l4">Listview container</param>
        /// <param name="l5">Listview container</param>
        /// <param name="chart">Chartview for Radar chart</param>
        private async void GethtmlASync(string bnetID, ListView l1, ListView l2, ListView l3, ListView l4, ListView l5, ChartView chart)
        {

            var url = "https://playoverwatch.com/en-us/career/pc/" + bnetID;

            var httpclient = new HttpClient();
            //this try catch is for private profiles or for if the player is unkown or unavailble
            try
            {
                var html = await httpclient.GetStringAsync(url);

                var htmlDocument = new HtmlDocument();

                htmlDocument.LoadHtml(html);

                var Productlist = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("id", "").Equals("competitive"))
                    .ToList();
                //data-group-id="stats" data-category-id="0x02E00000FFFFFFFF"
                var VarStatlist = Productlist[0].Descendants("div").Where(node => node.GetAttributeValue("data-category-id", "").Equals("0x02E00000FFFFFFFF"))
                .ToList();

                // AllheroStatlists[0] = Best section
                // AllheroStatlists[1] = Assists
                // AllheroStatlists[2] = Combat
                // AllheroStatlists[3] = Game
                // AllheroStatlists[4] = Avg
                // AllheroStatlists[5] = micellanoues
                // AllheroStatlists[6] = match awards
                var AllheroStatlists = VarStatlist[0].Descendants("tbody").Where(node => node.GetAttributeValue("class", "").Equals("DataTable-tableBody"))
                   .ToList();



                //Game stat array 
                //games lost = 0
                //games played = 1
                //games won = 2
                //time played = 3
                GameStatArray[0] = AllheroStatlists[3].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x086000000000042E"))
                    .FirstOrDefault().InnerText.Insert(10, " / ");

                GameStatArray[1] = AllheroStatlists[3].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000385"))
                    .FirstOrDefault().InnerText.Insert(12, " / ");

                GameStatArray[2] = AllheroStatlists[3].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000003F5"))
                    .FirstOrDefault().InnerText.Insert(9, " / ");

                GameStatArray[3] = AllheroStatlists[3].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000026"))
                    .FirstOrDefault().InnerText.Insert(11, " / ");
                //assist stat array
                //defensice assists =0 
                //healing done = 1
                //offensive assists = 2
                //Recon assists = 3
                AssistStatArray[0] = AllheroStatlists[1].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000317"))
                    .FirstOrDefault().InnerText.Insert(17, " / ");
                AssistStatArray[1] = AllheroStatlists[1].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000001D1"))
                    .FirstOrDefault().InnerText.Insert(12, " / ");
                AssistStatArray[2] = AllheroStatlists[1].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000318"))
                    .FirstOrDefault().InnerText.Insert(17, " / ");
                AssistStatArray[3] = AllheroStatlists[1].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x086000000000034A"))
                    .FirstOrDefault().InnerText.Insert(13, " / ");



                //CombatStatArray
                // All damage done = 0        barrier damage done = 1
                // damage done = 2            deaths = 3
                // elims = 4                  final blows = 5
                // hero dmg done = 6          melee final blows = 7
                // multikills = 8             objective kills = 9
                // objective time= 10         solo kills = 11
                // time on fire = 12

                CombatStatArray[0] = AllheroStatlists[2].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000000C9"))
                    .FirstOrDefault().InnerText.Insert(15, " / ");
                CombatStatArray[1] = AllheroStatlists[2].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000516"))
                    .FirstOrDefault().InnerText.Insert(19, " / ");
                CombatStatArray[2] = AllheroStatlists[2].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000621"))
                    .FirstOrDefault().InnerText.Insert(11, " / ");
                CombatStatArray[3] = AllheroStatlists[2].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000029"))
                    .FirstOrDefault().InnerText.Insert(6, " / ");
                CombatStatArray[4] = AllheroStatlists[2].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000025"))
                    .FirstOrDefault().InnerText.Insert(12, " / ");
                CombatStatArray[5] = AllheroStatlists[2].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x086000000000002C"))
                    .FirstOrDefault().InnerText.Insert(11, " / ");
                CombatStatArray[6] = AllheroStatlists[2].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000004B8"))
                    .FirstOrDefault().InnerText.Insert(16, " / ");
                CombatStatArray[7] = AllheroStatlists[2].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000381"))
                    .FirstOrDefault().InnerText.Insert(17, " / ");
                CombatStatArray[8] = AllheroStatlists[2].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000347"))
                    .FirstOrDefault().InnerText.Insert(9, " / ");
                CombatStatArray[9] = AllheroStatlists[2].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000326"))
                    .FirstOrDefault().InnerText.Insert(15, " / ");
                CombatStatArray[10] = AllheroStatlists[2].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000327"))
                    .FirstOrDefault().InnerText.Insert(14, " / ");
                CombatStatArray[11] = AllheroStatlists[2].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x086000000000002E"))
                    .FirstOrDefault().InnerText.Insert(10, " / ");
                CombatStatArray[12] = AllheroStatlists[2].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000003CD"))
                    .FirstOrDefault().InnerText.Insert(18, " / ");
                //averageStatArray
                //All dmg done = 0         Barrier Damage Done = 1
                //Deaths = 2               Eliminations = 3
                //Final Blows = 4          Healing Done = 5
                //Hero Damage Done = 6     Objective Kills = 7
                //Objective Time =8        Solo Kills = 9
                //Time Spent on Fire = 10

                averageStatArray[0] = AllheroStatlists[4].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000386"))
                    .FirstOrDefault().InnerText.Insert(32, " / ");
                averageStatArray[1] = AllheroStatlists[4].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000519"))
                    .FirstOrDefault().InnerText.Insert(36, " / ");
                averageStatArray[2] = AllheroStatlists[4].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000004C3"))
                    .FirstOrDefault().InnerText.Insert(23, " / ");
                averageStatArray[3] = AllheroStatlists[4].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000004B0"))
                    .FirstOrDefault().InnerText.Insert(29, " / ");
                averageStatArray[4] = AllheroStatlists[4].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000004B1"))
                    .FirstOrDefault().InnerText.Insert(28, " / ");
                averageStatArray[5] = AllheroStatlists[4].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000004B2"))
                    .FirstOrDefault().InnerText.Insert(29, " / ");
                averageStatArray[6] = AllheroStatlists[4].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000004C1"))
                    .FirstOrDefault().InnerText.Insert(33, " / ");
                averageStatArray[7] = AllheroStatlists[4].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000004B3"))
                    .FirstOrDefault().InnerText.Insert(32, " / ");
                averageStatArray[8] = AllheroStatlists[4].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000004B4"))
                    .FirstOrDefault().InnerText.Insert(31, " / ");
                averageStatArray[9] = AllheroStatlists[4].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000004B5"))
                    .FirstOrDefault().InnerText.Insert(27, " / ");
                averageStatArray[10] = AllheroStatlists[4].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x08600000000004B6"))
                    .FirstOrDefault().InnerText.Insert(35, " / ");


                //matchawardStatArray
                //cards = 0          medals =1
                //Medals - Gold = 2  Medals - Silver =3
                //Medals - Bronze =4
                matchawardStatArray[0] = AllheroStatlists[6].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000376"))
                    .FirstOrDefault().InnerText.Insert(5, " / ");
                matchawardStatArray[1] = AllheroStatlists[6].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000374"))
                    .FirstOrDefault().InnerText.Insert(6, " / ");
                matchawardStatArray[2] = AllheroStatlists[6].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000372"))
                    .FirstOrDefault().InnerText.Insert(13, " / ");
                matchawardStatArray[3] = AllheroStatlists[6].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000371"))
                    .FirstOrDefault().InnerText.Insert(15, " / ");
                matchawardStatArray[4] = AllheroStatlists[6].Descendants("tr").Where(node => node.GetAttributeValue("data-stat-id", "").Equals("0x0860000000000370"))
                    .FirstOrDefault().InnerText.Insert(15, " / ");


                //string[] GameStatArray = new string[4];
                //string[] AssistStatArray = new string[4];
                //string[] CombatStatArray = new string[13];
                //string[] averageStatArray = new string[11];
                //string[] matchawardStatArray = new string[5];



                //Writes to the Listviews
                ArrayAdapter<String> itemsAdapter =
                new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, GameStatArray);

                l1.Adapter = itemsAdapter;

                ArrayAdapter<String> itemsAdapter2 =
                new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, AssistStatArray);
                l2.Adapter = itemsAdapter2;

                ArrayAdapter<String> itemsAdapter3 =
                new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, CombatStatArray);
                l3.Adapter = itemsAdapter3;

                ArrayAdapter<String> itemsAdapter4 =
                new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, averageStatArray);
                l4.Adapter = itemsAdapter4;

                ArrayAdapter<String> itemsAdapter5 =
                new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, matchawardStatArray);
                l5.Adapter = itemsAdapter5;


                //string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), BnetID + ".txt");
                //file path for previous game stats
                string filePathPreGameStat = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), bnetID + "PreGameStatRelease.txt");
                //file path for game result(output stats)
                string fileGameresults = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), bnetID + "GameresultRelease.txt");
                //file path for all inputs stats
                string fileNametest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), bnetID + "Release.txt");
                int pos7 = GameStatArray[2].IndexOf("/");
                int Gamewins = Int32.Parse(GameStatArray[2].Substring(pos7 + 1).Trim());

                if (File.Exists(filePathPreGameStat))
                {
                    PreGameWins = Int32.Parse(File.ReadAllText(filePathPreGameStat));
                }
                else
                {
                    

                    PreGameWins = Int32.Parse(GameStatArray[2].Substring(pos7+1).Trim());
                }

                if (PreGameWins < Gamewins)
                {
                    WinCondition = 0;
                }
                else
                {
                    WinCondition = 1;
                }

                if (File.Exists(fileGameresults))
                {
                    File.AppendAllText(fileGameresults, ","+WinCondition.ToString());
                }
                else
                {
                    File.WriteAllText(fileGameresults, WinCondition.ToString());
                }

                
                //getting the data only for all of the entries needed for storage.
                //averageStatArray
                //All dmg done = 0         Barrier Damage Done = 1
                //Deaths = 2               Eliminations = 3
                //Final Blows = 4          Healing Done = 5
                //Hero Damage Done = 6     Objective Kills = 7
                //Objective Time =8        Solo Kills = 9
                //Time Spent on Fire = 10

                int pos = averageStatArray[0].IndexOf('/');
                string AllDmgDoneNum = averageStatArray[0].Substring(pos + 1).Trim();

                int pos1 = averageStatArray[1].IndexOf('/');
                string BarrierDmgNum = averageStatArray[1].Substring(pos1 + 1).Trim();

                int pos2 = averageStatArray[2].IndexOf('/');
                string DeathNum = averageStatArray[2].Substring(pos2 + 1).Trim();

                int pos3 = averageStatArray[3].IndexOf('/');
                string ElimNum = averageStatArray[3].Substring(pos3 + 1).Trim();

                int pos4 = averageStatArray[4].IndexOf('/');
                string FinalBlowNum = averageStatArray[4].Substring(pos4 + 1).Trim();

                int pos5 = averageStatArray[5].IndexOf('/');
                string HealingNum = averageStatArray[5].Substring(pos5 + 1).Trim();

                int pos6 = averageStatArray[6].IndexOf('/');
                string HerDmgNum = averageStatArray[6].Substring(pos6 + 1).Trim();

                string dataToBeStored = AllDmgDoneNum + "," + BarrierDmgNum + "," + DeathNum + "," + ElimNum + "," + FinalBlowNum + "," +
                    HealingNum + "," + HerDmgNum + ",";


                var entries = new[]
                {
                    new Entry(Float.ParseFloat( AllDmgDoneNum))
                    {
                        Label = "All Dmg Done",
                        ValueLabel = AllDmgDoneNum,
                        Color = SKColor.Parse("#6412a8")
                    },
                    new Entry(Float.ParseFloat( BarrierDmgNum))
                    {
                        Label = "Barrier Dmg done",
                        ValueLabel = BarrierDmgNum,
                        Color = SKColor.Parse("#6412a8")
                    },
                    new Entry(Float.ParseFloat( DeathNum))
                    {
                        Label = "Deaths",
                        ValueLabel = DeathNum,
                        Color = SKColor.Parse("#6412a8")
                    },
                    new Entry(Float.ParseFloat( ElimNum))
                    {
                        Label = "Eliminations",
                        ValueLabel = ElimNum,
                        Color = SKColor.Parse("#6412a8")
                    },
                    new Entry(Float.ParseFloat( FinalBlowNum))
                    {
                        Label = "Final blows",
                        ValueLabel = FinalBlowNum,
                        Color = SKColor.Parse("#6412a8")
                    },
                    new Entry(Float.ParseFloat( HealingNum))
                    {
                        Label = "Healing",
                        ValueLabel = HealingNum,
                        Color = SKColor.Parse("#6412a8")
                    },
                    new Entry(Float.ParseFloat( HerDmgNum))
                    {
                        Label = "Hero Dmg",
                        ValueLabel = HerDmgNum,
                        Color = SKColor.Parse("#6412a8")
                    }
                };
                var Chart = new RadarChart() { Entries = entries };
                chart.Chart = Chart;
                try
                {
                    if (File.Exists(fileNametest))
                    {
                        File.AppendAllText(fileNametest,  dataToBeStored);
                    }
                    else
                    {
                        File.WriteAllText(fileNametest, dataToBeStored);
                    }

                    String Testtext = File.ReadAllText(fileNametest);
                    System.Diagnostics.Debug.WriteLine(Testtext);
                }
                catch
                {

                }

            }
            catch
            {
                var intent = new Intent(this, typeof(MainActivity));
                String UnknownUser = "Unknown user";
                intent.PutExtra("user_unknown", UnknownUser);

                StartActivity(intent);
            }

        }






        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            mDrawerToggle.OnOptionsItemSelected(item);
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
            {
                outState.PutString("DrawerState", "Opened");
            }
            else
            {
                outState.PutString("DrawerState", "Closed");
            }
            base.OnSaveInstanceState(outState);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            mDrawerToggle.SyncState();
        }
    }
}