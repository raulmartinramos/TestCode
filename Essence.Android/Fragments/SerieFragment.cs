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
    public class SerieFragment : Fragment, IWindow
    {

        SerieCollection serieCollection;
        SearchCollection searchCollection;
        
        EditText editTextSerie;
        RecyclerView SeriesView;
        RecyclerView.LayoutManager mLayoutManager;
        SeriesAdapter mAdapter;
        

        private bool showWaitIndicator = false;
        Android.App.ProgressDialog progressBar;

        public SerieFragment()
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
            var view = inflater.Inflate(Resource.Layout.SeriesFindLayout, null);
            ((AppCompatActivity)Activity).SupportActionBar.Title = "Localizar Series";
            ((AppCompatActivity)Activity).SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            ((AppCompatActivity)Activity).SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);

           
            editTextSerie = view.FindViewById<EditText>(Resource.Id.editTextSerie);
            
            var fab = view.FindViewById<FloatingActionButton>(Resource.Id.buttonfindCardView);
            fab.Click += (sender, e) => 
            {
                SystemHelper.HideSoftKeyboard(Activity);
                CargaSearch(); 
            };

            SeriesView = view.FindViewById<RecyclerView>(Resource.Id.SeriesView);
            mLayoutManager = new LinearLayoutManager(SeriesView.Context);
            SeriesView.SetLayoutManager(mLayoutManager);

            

            HasOptionsMenu = true;
            return view;
        }


        private void CargaSearch()
        {
            if (!string.IsNullOrEmpty(editTextSerie.Text))
            {
                if (searchCollection == null) searchCollection = new SearchCollection();
                if (serieCollection == null) serieCollection = new SerieCollection();
                searchCollection.Clear();
                searchCollection.Get(this, editTextSerie.Text);
            }
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        public override void OnResume()
        {
            base.OnResume();
            NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Serie, Essence.Enums.ModelsEnum.TiposMetodos.Get);
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        void OnItemClick(object sender, int position)
        {
            var intent = new Intent(Activity, typeof(TemporadasActivity));
            intent.PutExtra(TemporadasActivity.EXTRA_IMDBID, serieCollection[position].imdbID);
            Activity.StartActivity(intent);
        }



        #region IWindow
        public Essence.Enums.InterfaceEnums.EssenceWindows GetWindow()
        {
            return Essence.Enums.InterfaceEnums.EssenceWindows.SerieFragment;
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
                    progressBar.SetMessage("Cargando Series...");
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
            if (TipoModel == Essence.Enums.ModelsEnum.TiposModels.Serie)
            {
                if (serieCollection != null)
                {
                    if (mAdapter == null)
                    {
                        mAdapter = new SeriesAdapter(Activity, serieCollection);
                        mAdapter.ItemClick += OnItemClick;
                        SeriesView.SetAdapter(mAdapter);
                    }
                    else
                    {
                        SeriesView.SetAdapter(mAdapter);
                        mAdapter.SetData(serieCollection);
                    }
                }
            }

            
            if (TipoModel == Essence.Enums.ModelsEnum.TiposModels.Search)
            {
                if (searchCollection.Count > 0)
                {
                    serieCollection.Clear();
                    serieCollection.GetBySearch(this, searchCollection.FirstOrDefault());
                }
            }
        }
        public string GetDescripcion() { return "Lista Series"; }
        #endregion


    }


    public class SeriesAdapter : RecyclerView.Adapter
    {
        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        // Underlying data set (a photo album):
        public SerieCollection serieCollection;
        TypedValue typedValue = new TypedValue();
        int background;
        Android.App.Activity parent;

        // Load the adapter with the data set (photo album) at construction time:
        public SeriesAdapter(Android.App.Activity context, SerieCollection serieCollection)
        {
            parent = context;
            context.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, typedValue, true);
            background = typedValue.ResourceId;
            this.serieCollection = serieCollection;
        }

        public void SetData(SerieCollection serieCollection)
        {
            this.serieCollection = serieCollection;
            this.NotifyDataSetChanged();
        }


        // Create a new photo CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.SeriesFindItemLayout, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            itemView.SetBackgroundResource(background);
            SeriesViewHolder vh = new SeriesViewHolder(itemView, OnClick);
            return vh;
        }

        // Fill in the contents of the photo card (invoked by the layout manager):
        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            SeriesViewHolder vh = holder as SeriesViewHolder;

            if (serieCollection[position].Poster != "N/A")
            {
                var ImageCacheada=ServiceContainer.Resolve<DataFrameWork>().ImageCache.Get<string,Bitmap>(serieCollection[position].Poster);
                if (ImageCacheada == null)
                {
                    ImageCacheada = BitmapHelper.GetImageBitmapFromUrl(serieCollection[position].Poster);
                    ServiceContainer.Resolve<DataFrameWork>().ImageCache.Add<string, Bitmap>(serieCollection[position].Poster, ImageCacheada, DateTime.Now.AddDays(1));
                }
                vh.PosterSeriesFindItemView.SetImageBitmap(ImageCacheada);
            }
            vh.titleSeriesFindItemView.Text = serieCollection[position].Title;
            vh.plotSeriesFindItemView.Text = serieCollection[position].Plot;
        }

        // Return the number of photos available in the photo album:
        public override int ItemCount
        {
            get { return serieCollection.Count; }
        }

        // Raise an event when the item-click takes place:
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);

        }
    }

    public class SeriesViewHolder : RecyclerView.ViewHolder
    {

        public ImageView PosterSeriesFindItemView { get; private set; }
        public TextView plotSeriesFindItemView { get; private set; }
        public TextView titleSeriesFindItemView { get; private set; }
        
        // Get references to the views defined in the CardView layout.
        public SeriesViewHolder(View itemView, Action<int> listener)
            : base(itemView)
        {
            // Locate and cache view references:
            PosterSeriesFindItemView = itemView.FindViewById<ImageView>(Resource.Id.PosterSeriesFindItemView);
            plotSeriesFindItemView = itemView.FindViewById<TextView>(Resource.Id.PlotSeriesFindItemView);
            titleSeriesFindItemView = itemView.FindViewById<TextView>(Resource.Id.TitleSeriesFindItemView);

            // Detect user clicks on the item view and report which item
            // was clicked (by position) to the listener:
            itemView.Click += (sender, e) => listener(base.Position);
        }
    }

}