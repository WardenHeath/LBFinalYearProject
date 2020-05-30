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
using Android.Support.V7.App;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Android.Support.V4.Widget;

namespace OverwatchApp
{
    public class MyActionBarDrawerToggle : SupportActionBarDrawerToggle
    {
        private AppCompatActivity MhostActivity;
        private int mOpenedResource;
        private int mClosedResource;
        public MyActionBarDrawerToggle(AppCompatActivity host, DrawerLayout drawerLayout, int OpenedResource, int ClosedResource)
            : base(host, drawerLayout, OpenedResource, ClosedResource)
        {
            MhostActivity = host;
            mOpenedResource = OpenedResource;
            mClosedResource = ClosedResource;
        }
        public override void OnDrawerOpened(View drawerView)
        {
            base.OnDrawerOpened(drawerView);
            MhostActivity.SupportActionBar.SetTitle(mOpenedResource);
        }

        public override void OnDrawerClosed(View drawerView)
        {
            base.OnDrawerClosed(drawerView);
            MhostActivity.SupportActionBar.SetTitle(mClosedResource);
        }
        public override void OnDrawerSlide(View drawerView, float slideOffset)
        {
            base.OnDrawerSlide(drawerView, slideOffset);
        }

    }
}