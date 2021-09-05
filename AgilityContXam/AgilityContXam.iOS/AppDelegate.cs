using AgilityContXam.iOS.Renderers;
using CarouselView.FormsPlugin.iOS;
using FFImageLoading.Forms.Platform;
using Foundation;
using Prism;
using Prism.Ioc;
using UIKit;


namespace AgilityContXam.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Mudar a cor do backbuttom
            //UINavigationBar.Appearance.TintColor = Color.White.ToUIColor();

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init();

            App.Height = UIScreen.MainScreen.Bounds.Height;
            App.Width = UIScreen.MainScreen.Bounds.Width;

            CachedImageRenderer.Init();
            CarouselViewRenderer.Init();

            MaterialButtonRenderer.Initialize();
            MaterialFrameRenderer.Initialize();

            LoadApplication(new App(new iOSInitializer()));

            return base.FinishedLaunching(app, options);
        }
    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
        }
    }
}
