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

using Essence.IOC;
using Essence.Global;
using Essence.ViewModel;

namespace EssenceAndroid.Application
{
    [Application(Label = "Essence", Debuggable = true)]
    public class Application : global::Android.App.Application, global::Android.App.Application.IActivityLifecycleCallbacks
    {
        private Activity _CurrentActivity;
        public Activity CurrentActivity
        {
            get { return _CurrentActivity; }
            set { _CurrentActivity = value; }
        }

        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        { }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                //Log
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                _CurrentActivity.RunOnUiThread(() => Toast.MakeText(_CurrentActivity, "Unhadled Exception was thrown", ToastLength.Short).Show());
            };

            //Registros IOC
            
            ServiceContainer.Register<DataFrameWork>();
            ServiceContainer.Register<SerieFragment>();
            ServiceContainer.Register<GlobalDataFrameWork>();
            ServiceContainer.Register<FavoritesFragment>();
            
        }

        #region IActivityLifecycleCallbacks Members

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            _CurrentActivity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
        }

        public void OnActivityStopped(Activity activity)
        {
        }

        #endregion


    }
}