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
using Essence.Model.Model;
using Xciles.PclValueInjecter;

namespace EssenceAndroid
{
    [Activity(Label = "EpisodiosActivity")]
    public class EpisodiosActivity : AppCompatActivity, IWindow
    {

        public const string EXTRA_IMDBID = "imdbID";
        SerieCollection EpisodiosCollection;
        EpisodiosAdapter mAdapter;
        RecyclerView EpisodiosView;
        RecyclerView.LayoutManager mLayoutManager;
        FloatingActionButton IconoAccion;

        SerieModel TemporadaModelCacheada;
        SerieModel SerieModelCacheada;
        SeriesFavoritesCollection seriesFavoritesCollection;

        private bool showWaitIndicator = false;
        Android.App.ProgressDialog progressBar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.EpisodiosLayout);

            var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var imdbID = Intent.GetStringExtra(EXTRA_IMDBID);

            TemporadaModelCacheada = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Get<string, SerieModel>(imdbID);
            SerieModelCacheada = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Get<string, SerieModel>(TemporadaModelCacheada.seriesID);

            var collapsingToolbar = FindViewById<CollapsingToolbarLayout> (Resource.Id.collapsing_toolbar);
            collapsingToolbar.SetTitle(SerieModelCacheada.Title);

            var imageView = FindViewById<ImageView>(Resource.Id.backdrop);
            var ImageCacheada = ServiceContainer.Resolve<DataFrameWork>().ImageCache.Get<string, Bitmap>(TemporadaModelCacheada.Poster);
            imageView.SetImageBitmap(ImageCacheada);

            EpisodiosView = FindViewById<RecyclerView>(Resource.Id.EpisodiosView);
            mLayoutManager = new LinearLayoutManager(EpisodiosView.Context);
            EpisodiosView.SetLayoutManager(mLayoutManager);

            IconoAccion = FindViewById<FloatingActionButton>(Resource.Id.FloatingIcono);
            AjustaIconoFavorite(IconoAccion, TemporadaModelCacheada.Favorite);


            IconoAccion.Click += (sender, e) =>
            {
                if (seriesFavoritesCollection == null) seriesFavoritesCollection = new SeriesFavoritesCollection();
                if (!TemporadaModelCacheada.Favorite)
                    seriesFavoritesCollection.Post(this, TemporadaModelCacheada);
                else if (TemporadaModelCacheada.Id != Guid.Empty)
                    seriesFavoritesCollection.Delete(this, TemporadaModelCacheada);
            };


            CargaEpisodios(SerieModelCacheada.imdbID, TemporadaModelCacheada.Season);

        }

        protected override void OnResume()
        {
            base.OnResume();
            NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Espisodies, Essence.Enums.ModelsEnum.TiposMetodos.Get);
        }


        private void AjustaIconoFavorite(FloatingActionButton IconoAccion, bool Favorite)
        {
            IconoAccion.SetImageResource(Favorite ? Resource.Drawable.ic_action_good_dark : Resource.Drawable.ic_action_bad_dark);
        }

        private void CargaEpisodios(string imdbID,string Season)
        {
            if (EpisodiosCollection == null) EpisodiosCollection = new SerieCollection();
            EpisodiosCollection.GetEpisodiesById(this, imdbID, Season);
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
            var intent = new Intent(this, typeof(FichaSerieActivity));
            intent.PutExtra(FichaSerieActivity.EXTRA_IMDBID, EpisodiosCollection[position].imdbID);
            intent.PutExtra(FichaSerieActivity.EXTRA_TIPOOPERACION, (int)AndroidEnum.TipoOperacion.Episodio);
            this.StartActivity(intent);
        }



        #region IWindow
        public Essence.Enums.InterfaceEnums.EssenceWindows GetWindow()
        {
            return Essence.Enums.InterfaceEnums.EssenceWindows.EpisodiosActivity;
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
                    progressBar.SetMessage("Cargando Episodios...");
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
            if (TipoModel == Essence.Enums.ModelsEnum.TiposModels.Espisodies)
            {
                if (mAdapter == null)
                {
                    mAdapter = new EpisodiosAdapter(this, EpisodiosCollection);
                    mAdapter.ItemClick += OnItemClick;
                    EpisodiosView.SetAdapter(mAdapter);
                }
                else
                {
                    mAdapter.SetData(EpisodiosCollection);
                }
            }

            if (TipoModel == Essence.Enums.ModelsEnum.TiposModels.SeriesFavorites)
            {
                switch (TipoMetodo)
                {
                    case Essence.Enums.ModelsEnum.TiposMetodos.Post:
                        TemporadaModelCacheada.Favorite = true;
                        SeriesFavoritesModel SeriesFavoritesModel = new SeriesFavoritesModel();
                        SeriesFavoritesModel.InjectFrom(TemporadaModelCacheada);
                        ServiceContainer.Resolve<GlobalDataFrameWork>().SeriesFavorites.Add(SeriesFavoritesModel);
                        break;
                    case Essence.Enums.ModelsEnum.TiposMetodos.Delete:
                        TemporadaModelCacheada.Favorite = false;
                        TemporadaModelCacheada.Id = Guid.Empty;
                        var SerieFavorite = ServiceContainer.Resolve<GlobalDataFrameWork>().SeriesFavorites.Where(c => c.imdbID == TemporadaModelCacheada.imdbID).FirstOrDefault();
                        if (SerieFavorite != null)
                            ServiceContainer.Resolve<GlobalDataFrameWork>().SeriesFavorites.Remove(SerieFavorite);
                        break;
                }
                AjustaIconoFavorite(IconoAccion, TemporadaModelCacheada.Favorite);
            }
        }
        public string GetDescripcion() { return "Lista Episodios"; }
        #endregion
    }

    public class EpisodiosAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        // Underlying data set (a photo album):
        public SerieCollection temporadaCollection;
        TypedValue typedValue = new TypedValue();
        int background;
        Android.App.Activity parent;

        // Load the adapter with the data set (photo album) at construction time:
        public EpisodiosAdapter(Android.App.Activity context, SerieCollection temporadaCollection)
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
                        Inflate(Resource.Layout.EpisodiosItemLayout, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            itemView.SetBackgroundResource(background);
            EpisodiosViewHolder vh = new EpisodiosViewHolder(itemView, OnClick);
            return vh;
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            EpisodiosViewHolder vh = holder as EpisodiosViewHolder;

            if (temporadaCollection[position].Poster != "N/A")
            {
                var ImageCacheada = ServiceContainer.Resolve<DataFrameWork>().ImageCache.Get<string, Bitmap>(temporadaCollection[position].Poster);
                if (ImageCacheada == null)
                {
                    ImageCacheada = BitmapHelper.GetImageBitmapFromUrl(temporadaCollection[position].Poster);
                    ServiceContainer.Resolve<DataFrameWork>().ImageCache.Add<string, Bitmap>(temporadaCollection[position].Poster, ImageCacheada, DateTime.Now.AddDays(1));
                }
                vh.PosterEpisodiosItemView.SetImageBitmap(ImageCacheada);
            }
            vh.titleEpisodiosItemView.Text = temporadaCollection[position].Episode;
            vh.plotEpisodiosItemView.Text = temporadaCollection[position].Plot;
            vh.ratingbar.Rating = temporadaCollection[position].Rating;
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

    public class EpisodiosViewHolder : RecyclerView.ViewHolder
    {

        public ImageView PosterEpisodiosItemView { get; private set; }
        public TextView plotEpisodiosItemView { get; private set; }
        public TextView titleEpisodiosItemView { get; private set; }
        public RatingBar ratingbar { get; private set; }
        

        // Get references to the views defined in the CardView layout.
        public EpisodiosViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            // Locate and cache view references:
            PosterEpisodiosItemView = itemView.FindViewById<ImageView>(Resource.Id.PosterEpisodiosItemView);
            plotEpisodiosItemView = itemView.FindViewById<TextView>(Resource.Id.PlotEpisodiosItemView);
            titleEpisodiosItemView = itemView.FindViewById<TextView>(Resource.Id.TitleEpisodiosItemView);
            ratingbar = itemView.FindViewById<RatingBar>(Resource.Id.ratingbar);
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }

}
