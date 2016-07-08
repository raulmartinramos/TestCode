using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Essence.Injection;
using Essence.IOC;
using Essence.Model;
using Essence.ViewModel;
using PullToRefresharp.Android.Views;
using PullToRefresharp.Android.Widget;
using System;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views.InputMethods;
using Essence.Global;
using Android.Graphics;


namespace EssenceAndroid
{
    public class FavoritesFragment : Fragment, IWindow
    {

        SeriesFavoritesCollection seriesFavoritesCollection;
        
        RecyclerView SeriesView;
        RecyclerView.LayoutManager mLayoutManager;
        FavoritesAdapter mAdapter;
        
        private bool showWaitIndicator = false;
        Android.App.ProgressDialog progressBar;

        public FavoritesFragment()
        {
            this.RetainInstance = true;
			this.HasOptionsMenu = (true);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.FavoritesLayout, null);
            ((AppCompatActivity)Activity).SupportActionBar.Title = "Temporadas Favoritas";
            ((AppCompatActivity)Activity).SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            ((AppCompatActivity)Activity).SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);

            SeriesView = view.FindViewById<RecyclerView>(Resource.Id.SeriesView);
            mLayoutManager = new LinearLayoutManager(SeriesView.Context);
            SeriesView.SetLayoutManager(mLayoutManager);
            seriesFavoritesCollection = ServiceContainer.Resolve<GlobalDataFrameWork>().SeriesFavorites;

            HasOptionsMenu = true;
            return view;
        }



        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        public override void OnResume()
        {
            base.OnResume();
            NotifyChange(Essence.Enums.ModelsEnum.TiposModels.SeriesFavorites, Essence.Enums.ModelsEnum.TiposMetodos.Get);
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        async void OnItemClick(object sender, int position)
        {
            //Si la Serie no está cacheada tenemos que cachearla antes
            var SerieModelCacheada = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Get<string, SerieModel>(seriesFavoritesCollection[position].seriesID);
            if (SerieModelCacheada == null)
                await seriesFavoritesCollection.CargaEnCacheSerieById(this,seriesFavoritesCollection[position].seriesID);
            var intent = new Intent(Activity, typeof(EpisodiosActivity));
            intent.PutExtra(EpisodiosActivity.EXTRA_IMDBID, seriesFavoritesCollection[position].imdbID);
            this.StartActivity(intent);
        }



        #region IWindow
        public Essence.Enums.InterfaceEnums.EssenceWindows GetWindow()
        {
            return Essence.Enums.InterfaceEnums.EssenceWindows.FavoritesFragment;
        }
        public bool ShowWaitIndicator
        {
            get { return showWaitIndicator; }
            set
            {
                if (value)
                {
                    if (progressBar == null)
                        progressBar = new Android.App.ProgressDialog(View.Context);
                    progressBar.SetCancelable(false);
                    progressBar.SetMessage("Cargando Series Favoritas...");
                    progressBar.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
                    progressBar.Indeterminate = true;
                    progressBar.Show();
                }
                else
                {
                    if (progressBar != null)
                        progressBar.Dismiss();
                }
                showWaitIndicator = value;
            }
        }
        public void ShowError(string error)
        {
            Toast.MakeText(Activity.ApplicationContext, error, ToastLength.Long).Show();
        }
        public void NotifyChange(Essence.Enums.ModelsEnum.TiposModels TipoModel, Essence.Enums.ModelsEnum.TiposMetodos TipoMetodo = Essence.Enums.ModelsEnum.TiposMetodos.Get)
        {
            if (TipoModel == Essence.Enums.ModelsEnum.TiposModels.SeriesFavorites)
            {
                if (seriesFavoritesCollection != null)
                {
                    if (mAdapter == null)
                    {
                        mAdapter = new FavoritesAdapter(Activity, seriesFavoritesCollection);
                        mAdapter.ItemClick += OnItemClick;
                        SeriesView.SetAdapter(mAdapter);
                    }
                    else
                    {
                        SeriesView.SetAdapter(mAdapter);
                        mAdapter.SetData(seriesFavoritesCollection);
                    }
                }
            }
        }
        public string GetDescripcion() { return "Lista Series"; }
        #endregion


    }


    public class FavoritesAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        // Underlying data set (a photo album):
        public SeriesFavoritesCollection seriesFavoritesCollection;
        TypedValue typedValue = new TypedValue();
        int background;
        Android.App.Activity parent;

        // Load the adapter with the data set (photo album) at construction time:
        public FavoritesAdapter(Android.App.Activity context, SeriesFavoritesCollection seriesFavoritesCollection)
        {
            parent = context;
            context.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, typedValue, true);
            background = typedValue.ResourceId;
            this.seriesFavoritesCollection = seriesFavoritesCollection;
        }

        public void SetData(SeriesFavoritesCollection seriesFavoritesCollection)
        {
            this.seriesFavoritesCollection = seriesFavoritesCollection;
            this.NotifyDataSetChanged();
        }


        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.FavoritesItemLayout, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            itemView.SetBackgroundResource(background);
            FavoritesViewHolder vh = new FavoritesViewHolder(itemView, OnClick);
            return vh;
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            FavoritesViewHolder vh = holder as FavoritesViewHolder;

            if (seriesFavoritesCollection[position].Poster != "N/A")
            {
                var ImageCacheada=ServiceContainer.Resolve<DataFrameWork>().ImageCache.Get<string,Bitmap>(seriesFavoritesCollection[position].Poster);
                if (ImageCacheada == null)
                {
                    ImageCacheada = BitmapHelper.GetImageBitmapFromUrl(seriesFavoritesCollection[position].Poster);
                    ServiceContainer.Resolve<DataFrameWork>().ImageCache.Add<string, Bitmap>(seriesFavoritesCollection[position].Poster, ImageCacheada, DateTime.Now.AddDays(1));
                }
                vh.posterFavoritesItemView.SetImageBitmap(ImageCacheada);
            }

            vh.seasonFavoritesItemView.Text = seriesFavoritesCollection[position].Season;
            vh.titleFavoritesItemView.Text = seriesFavoritesCollection[position].Title;
            vh.plotFavoritesItemView.Text = seriesFavoritesCollection[position].Plot;
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return seriesFavoritesCollection.Count; }
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);

        }
    }

    public class FavoritesViewHolder : RecyclerView.ViewHolder
    {

        public ImageView posterFavoritesItemView { get; private set; }
        public TextView plotFavoritesItemView { get; private set; }
        public TextView titleFavoritesItemView { get; private set; }
        public TextView seasonFavoritesItemView { get; private set; }
        
        // Get references to the views defined in the CardView layout.
        public FavoritesViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            // Locate and cache view references:
            posterFavoritesItemView = itemView.FindViewById<ImageView>(Resource.Id.PosterFavoritesItemView);
            plotFavoritesItemView = itemView.FindViewById<TextView>(Resource.Id.PlotFavoritesItemView);
            titleFavoritesItemView = itemView.FindViewById<TextView>(Resource.Id.TitleFavoritesItemView);
            seasonFavoritesItemView = itemView.FindViewById<TextView>(Resource.Id.SeasonFavoritesItemView);

            // Detect user clicks on the item view and report which item
            // was clicked (by position) to the listener:
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }

}