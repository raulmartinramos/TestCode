using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;

using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using Essence.Global;
using Essence.IOC;
using System.Collections.Generic;
using System.Net.Http;
using ModernHttpClient;
using Essence.Injection;
using Essence.ViewModel;
using System.Net;

namespace EssenceAndroid
{
    [Activity(Label = "Essence", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/Icon")]
    public class MainActivity : BaseActivity, IWindow
    {

        DrawerLayout drawerLayout;
        NavigationView navigationView;

        protected override int LayoutResource
        {
            get
            {
                return Resource.Layout.main;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ServiceContainer.Resolve<DataFrameWork>().Uriomdbapi = "http://www.omdbapi.com";
            //ServiceContainer.Resolve<DataFrameWork>().Uribackend = "http://www.mimotool.com//Series";
            ServiceContainer.Resolve<DataFrameWork>().Uribackend = "http://www.mimotool.com";
            //ServiceContainer.Resolve<DataFrameWork>().HttpClientFactory = (() => new HttpClient(new NativeMessageHandler()));

            ServiceContainer.Resolve<DataFrameWork>().HttpClientFactory = (() =>
            {
                var messageHandler = new NativeMessageHandler();
                if (messageHandler.SupportsAutomaticDecompression) // returns true
                    messageHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                return new HttpClient(new NativeMessageHandler());
            });
            DensityExtensions.Initialize(this);

            ServiceContainer.Resolve<GlobalDataFrameWork>().SeriesFavorites.Get(this);
            ServiceContainer.Resolve<GlobalDataFrameWork>().EpisodiesRating.Get(this);

            drawerLayout = this.FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            //Set hamburger items menu
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);

            //setup navigation view
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            //handle navigation
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                e.MenuItem.SetChecked(true);

                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_home_1:
                        ListItemClicked(0);
                        break;
                    case Resource.Id.nav_home_2:
                        ListItemClicked(1);
                        break;
                }

                //Snackbar.Make(drawerLayout, "You selected: " + e.MenuItem.TitleFormatted, Snackbar.LengthLong)
                //    .Show();

                drawerLayout.CloseDrawers();
            };


            //if first time you will want to go ahead and click first item.
            if (savedInstanceState == null)
            {
                ListItemClicked(0);
            }
        }

        int oldPosition = -1;
        private void ListItemClicked(int position)
        {
            //this way we don't load twice, but you might want to modify this a bit.
            if (position == oldPosition)
                return;

            oldPosition = position;

            Android.Support.V4.App.Fragment fragment = null;
            switch (position)
            {
                case 0:
                    fragment = ServiceContainer.Resolve<SerieFragment>(); 
                    break;
                case 1:
                    fragment = ServiceContainer.Resolve<FavoritesFragment>(); 
                    break;
            }

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int fragmentsactivos=0;
            foreach(var fragment in SupportFragmentManager.Fragments)
                if (fragment != null) fragmentsactivos++;

            var fragments = SupportFragmentManager.Fragments.Count;
            //if (fragmentsactivos > 1)
            //{
            //    SupportFragmentManager.PopBackStack();
            //    ////OnBackPressed();
            //    //Android.Support.V4.App.FragmentManager fm = SupportFragmentManager;
            //    //Android.Support.V4.App.FragmentTransaction transaction = fm.BeginTransaction();
            //    //fm.Fragments.RemoveAt(SupportFragmentManager.Fragments.Count - 1);
            //    ////SupportFragmentManager.Fragments.RemoveAt(SupportFragmentManager.Fragments.Count-1) ;
            //    //transaction.Commit();
            //    //SupportFragmentManager.ExecutePendingTransactions();
            //    return false;
            //}

            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #region IWindow
        public Essence.Enums.InterfaceEnums.EssenceWindows GetWindow()
        {
            return Essence.Enums.InterfaceEnums.EssenceWindows.MainActivity;
        }
        public bool ShowWaitIndicator { get; set; }
        public void ShowError(string error)
        {
            Toast.MakeText(this, error, ToastLength.Long).Show();
        }
        public void NotifyChange(Essence.Enums.ModelsEnum.TiposModels TipoModel, Essence.Enums.ModelsEnum.TiposMetodos TipoMetodo = Essence.Enums.ModelsEnum.TiposMetodos.Get)
        {
        }
        public string GetDescripcion() { return "Main Activity"; }
        #endregion

    }
}

