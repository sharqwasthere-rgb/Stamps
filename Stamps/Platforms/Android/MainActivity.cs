using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;

namespace Stamps
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Hide the action bar/title bar
            if (ActionBar != null)
            {
                ActionBar.Hide();
            }
            
            // Enable edge-to-edge display
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R && Window != null)
            {
                Window.SetDecorFitsSystemWindows(false);
                
                // Make status bar transparent
                Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
                
                // Light status bar icons for dark backgrounds
                var windowInsetsController = WindowCompat.GetInsetsController(Window, Window.DecorView);
                if (windowInsetsController != null)
                {
                    windowInsetsController.AppearanceLightStatusBars = false;
                }
            }
        }
    }
}
