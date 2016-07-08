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
using Android.Support.V7.Widget;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.Design.Widget;
using Essence.IOC;
using Essence.Global;
using Essence.Model;
using Android.Graphics;
using Essence.ViewModel;
using Android.Util;
using Essence.Injection;

namespace EssenceAndroid
{
    [Activity(Label = "TemporadasActivity")]
    public class TemporadasActivity : AppCompatActivity, IWindow
    {

        public const string EXTRA_IMDBID = "imdbID";
        SerieCollection temporadasCollection;
        TemporadasAdapter mAdapter;
        RecyclerView TemporadasView;
        RecyclerView.LayoutManager mLayoutManager;

        private bool showWaitIndicator = false;
        Android.App.ProgressDialog progressBar;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.TemporadasLayout);

            var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var imdbID = Intent.GetStringExtra(EXTRA_IMDBID);
            var SerieModelCacheada = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Get<string,SerieModel>(imdbID);
            var collapsingToolbar = FindViewById<CollapsingToolbarLayout> (Resource.Id.collapsing_toolbar);
            collapsingToolbar.SetTitle(SerieModelCacheada.Title);

            var imageView = FindViewById<ImageView>(Resource.Id.backdrop);
            var ImageCacheada=ServiceContainer.Resolve<DataFrameWork>().ImageCache.Get<string,Bitmap>(SerieModelCacheada.Poster);
            imageView.SetImageBitmap(ImageCacheada);

            TemporadasView = FindViewById<RecyclerView>(Resource.Id.TemporadasView);
            mLayoutManager = new LinearLayoutManager(TemporadasView.Context);
            TemporadasView.SetLayoutManager(mLayoutManager);

            FloatingActionButton IconoAccion = FindViewById<FloatingActionButton>(Resource.Id.FloatingIcono);
            IconoAccion.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(FichaSerieActivity));
                intent.PutExtra(FichaSerieActivity.EXTRA_IMDBID, SerieModelCacheada.imdbID);
                intent.PutExtra(FichaSerieActivity.EXTRA_TIPOOPERACION, (int)AndroidEnum.TipoOperacion.Ficha);
                this.StartActivity(intent);
            };

            CargaTemporadas(imdbID);

        }

        protected override void OnResume()
        {
            base.OnResume();
            NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Serie, Essence.Enums.ModelsEnum.TiposMetodos.Get);
        }

        private void CargaTemporadas(string imdbID)
        {
            if (temporadasCollection == null) temporadasCollection = new SerieCollection();
            temporadasCollection.GetSeasonsById(this,imdbID);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.sample_actions, menu);
            return true;
        }


        void OnItemClick(object sender, int position)
        {
            var intent = new Intent(this, typeof(EpisodiosActivity));
            intent.PutExtra(EpisodiosActivity.EXTRA_IMDBID, temporadasCollection[position].imdbID);
            this.StartActivity(intent);
        }



        #region IWindow
        public Essence.Enums.InterfaceEnums.EssenceWindows GetWindow()
        {
            return Essence.Enums.InterfaceEnums.EssenceWindows.TemporadasActivity;
        }
        public bool ShowWaitIndicator
        {
            get { return showWaitIndicator; }
            set
            {
                if (value)
                {
                    if (progressBar == null)
                        progressBar = new Android.App.ProgressDialog(this);
                    progressBar.SetCancelable(false);
                    progressBar.SetMessage("Cargando Temporadas...");
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
            Toast.MakeText(this, error, ToastLength.Long).Show();
        }
        public void NotifyChange(Essence.Enums.ModelsEnum.TiposModels TipoModel, Essence.Enums.ModelsEnum.TiposMetodos TipoMetodo = Essence.Enums.ModelsEnum.TiposMetodos.Get)
        {
            if (TipoModel == Essence.Enums.ModelsEnum.TiposModels.Serie)
            {
                if (mAdapter == null)
                {
                    mAdapter = new TemporadasAdapter(this, temporadasCollection);
                    mAdapter.ItemClick += OnItemClick;
                    TemporadasView.SetAdapter(mAdapter);
                }
                else
                {
                    mAdapter.SetData(temporadasCollection);
                }

            }
        }
        public string GetDescripcion() { return "Lista Temporadas"; }
        #endregion
    }

    public class TemporadasAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        // Underlying data set (a photo album):
        public SerieCollection temporadaCollection;
        TypedValue typedValue = new TypedValue();
        int background;
        Android.App.Activity parent;

        // Load the adapter with the data set (photo album) at construction time:
        public TemporadasAdapter(Android.App.Activity context, SerieCollection temporadaCollection)
        {
            parent = context;
            context.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, typedValue, true);
            background = typedValue.ResourceId;
            this.temporadaCollection = temporadaCollection;
        }

        public void SetData(SerieCollection temporadaCollection)
        {
            this.temporadaCollection = temporadaCollection;
            this.NotifyDataSetChanged();
        }


        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.TemporadasItemLayout, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            itemView.SetBackgroundResource(background);
            TemporadasViewHolder vh = new TemporadasViewHolder(itemView, OnClick);
            return vh;
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            TemporadasViewHolder vh = holder as TemporadasViewHolder;

            if (temporadaCollection[position].Poster != "N/A")
            {
                var ImageCacheada = ServiceContainer.Resolve<DataFrameWork>().ImageCache.Get<string, Bitmap>(temporadaCollection[position].Poster);
                if (ImageCacheada == null)
                {
                    ImageCacheada = BitmapHelper.GetImageBitmapFromUrl(temporadaCollection[position].Poster);
                    ServiceContainer.Resolve<DataFrameWork>().ImageCache.Add<string, Bitmap>(temporadaCollection[position].Poster, ImageCacheada, DateTime.Now.AddDays(1));
                }
                vh.PosterTemporadasItemView.SetImageBitmap(ImageCacheada);
            }
            vh.titleTemporadasItemView.Text = temporadaCollection[position].Season;
            vh.plotTemporadasItemView.Text = temporadaCollection[position].Plot;

            if (temporadaCollection[position].Favorite)
                vh.favoriteTemporadasItemView.SetImageResource(Resource.Drawable.ic_action_good);
            else
                vh.favoriteTemporadasItemView.SetImageResource(Resource.Drawable.ic_action_bad);
        }


        public override int ItemCount
        {
            get { return temporadaCollection.Count; }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

    }

    public class TemporadasViewHolder : RecyclerView.ViewHolder
    {

        public ImageView PosterTemporadasItemView { get; private set; }
        public TextView plotTemporadasItemView { get; private set; }
        public TextView titleTemporadasItemView { get; private set; }
        public ImageView favoriteTemporadasItemView { get; private set; }
        

        // Get references to the views defined in the CardView layout.
        public TemporadasViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            // Locate and cache view references:
            PosterTemporadasItemView = itemView.FindViewById<ImageView>(Resource.Id.PosterTemporadasItemView);
            plotTemporadasItemView = itemView.FindViewById<TextView>(Resource.Id.PlotTemporadasItemView);
            titleTemporadasItemView = itemView.FindViewById<TextView>(Resource.Id.TitleTemporadasItemView);
            favoriteTemporadasItemView = itemView.FindViewById<ImageView>(Resource.Id.FavoriteTemporadasItemView);
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }

}