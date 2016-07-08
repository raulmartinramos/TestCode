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
using Android.Views.InputMethods;

namespace EssenceAndroid
{
    public  class SystemHelper
    {
        public static void HideSoftKeyboard(Activity activity)
        {
            new Handler().Post(delegate
            {
                var view = activity.CurrentFocus;
                if (view != null)
                {
                    InputMethodManager manager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
                    manager.HideSoftInputFromWindow(view.WindowToken, 0);
                }
            });
        }

    }
}