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
using Essence.Injection;
using Essence.ViewModel;
using Essence.Model.Model;
using Xciles.PclValueInjecter;
using Essence.Dto;

namespace EssenceAndroid
{
    [Activity(Label = "FichaSerieActivity")]
    public class FichaSerieActivity : AppCompatActivity, IWindow
    {

        public const string EXTRA_IMDBID = "imdbID";
        public const string EXTRA_TIPOOPERACION = "TipoOperacion";
        AndroidEnum.TipoOperacion TipoOperacion;

        SeriesFavoritesCollection seriesFavoritesCollection;

        SerieModel SerieModelCacheada;

        RatingBar ratingbar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.fichaSerieLayout);

            var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var imdbID = Intent.GetStringExtra(EXTRA_IMDBID);
            TipoOperacion = (AndroidEnum.TipoOperacion)(int)Intent.GetIntExtra(EXTRA_TIPOOPERACION,1);

            SerieModelCacheada = ServiceContainer.Resolve<DataFrameWork>().SeriesCache.Get<string,SerieModel>(imdbID);
            var collapsingToolbar = FindViewById<CollapsingToolbarLayout> (Resource.Id.collapsing_toolbar);
            collapsingToolbar.SetTitle(SerieModelCacheada.Title);

            var imageView = FindViewById<ImageView>(Resource.Id.backdrop);
            var ImageCacheada=ServiceContainer.Resolve<DataFrameWork>().ImageCache.Get<string,Bitmap>(SerieModelCacheada.Poster);
            imageView.SetImageBitmap(ImageCacheada);
            
            ratingbar = FindViewById<RatingBar>(Resource.Id.ratingbar);
            ratingbar.Rating = SerieModelCacheada.Rating;
            ratingbar.RatingBarChange += (o, e) =>
            {
                EpisodiesRatingDTO EpisodiesRatingDTO = new EpisodiesRatingDTO() { imdbID = SerieModelCacheada.imdbID, Rating =  (int) Math.Round (ratingbar.Rating)};
                ServiceContainer.Resolve<GlobalDataFrameWork>().EpisodiesRating.Post(this, EpisodiesRatingDTO);
                SerieModelCacheada.Rating = (int)Math.Round(ratingbar.Rating);
            };

            if (TipoOperacion == AndroidEnum.TipoOperacion.Ficha)
                ratingbar.Visibility = ViewStates.Gone;

            AjustaView(SerieModelCacheada);
        }


        private void AjustaView(SerieModel SerieModel)
        {
            FindViewById<TextView>(Resource.Id.Year).Text = SerieModel.Year;
            FindViewById<TextView>(Resource.Id.Released).Text = SerieModel.Released;
            FindViewById<TextView>(Resource.Id.Plot).Text = SerieModel.Plot;
            FindViewById<TextView>(Resource.Id.Actors).Text = SerieModel.Actors;
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



        #region IWindow
        public Essence.Enums.InterfaceEnums.EssenceWindows GetWindow()
        {
            return Essence.Enums.InterfaceEnums.EssenceWindows.SerieFragment;
        }
        public bool ShowWaitIndicator { get; set; }
        public void ShowError(string error)
        {
            Toast.MakeText(this, error, ToastLength.Long).Show();
        }
        public void NotifyChange(Essence.Enums.ModelsEnum.TiposModels TipoModel, Essence.Enums.ModelsEnum.TiposMetodos TipoMetodo = Essence.Enums.ModelsEnum.TiposMetodos.Get)
        {
            switch (TipoMetodo)
            {
                case Essence.Enums.ModelsEnum.TiposMetodos.Post:
                    
                    EpisodiesRatingModel episodierating = ServiceContainer.Resolve<GlobalDataFrameWork>().EpisodiesRating.Where(c => c.imdbID == SerieModelCacheada.imdbID).FirstOrDefault();
                    if (episodierating == null)
                    {
                        episodierating = new EpisodiesRatingModel() { imdbID = SerieModelCacheada.imdbID, Rating = (int)Math.Round(ratingbar.Rating) };
                        ServiceContainer.Resolve<GlobalDataFrameWork>().EpisodiesRating.Add(episodierating);
                    }
                    episodierating.Rating = (int)Math.Round(ratingbar.Rating);
                    SerieModelCacheada.Rating = episodierating.Rating;
                    break;
            }

        }

        public string GetDescripcion() { return "Ficha Serie"; }
        #endregion

    }
}