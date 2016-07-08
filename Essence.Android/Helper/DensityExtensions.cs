using System;
using Android.Hardware;
using Android.Util;
using Android.Content;
using Android.Runtime;
using Android.Views;

namespace EssenceAndroid
{
    public static class DensityExtensions
    {
        //static DisplayMetrics displayMetrics;
        static float density;
        static readonly DisplayMetrics displayMetrics = new DisplayMetrics();

        public static void Initialize(Context ctx)
        {
            var wm = ctx.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            var displayMetrics = new DisplayMetrics();
            wm.DefaultDisplay.GetMetrics(displayMetrics);
            density = displayMetrics.Density;
        }

        public static int ToPixels(this int dp)
        {
            //return (int)TypedValue.ApplyDimension (ComplexUnitType.Dip, dp, displayMetrics);
            return (int)(dp * density + 0.5f);
        }

        public static int ToPixels(this Context ctx, int dp)
        {
            var wm = ctx.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            wm.DefaultDisplay.GetMetrics(displayMetrics);

            var density = displayMetrics.Density;
            return (int)(dp * density + 0.5f);
        }

    }
}

