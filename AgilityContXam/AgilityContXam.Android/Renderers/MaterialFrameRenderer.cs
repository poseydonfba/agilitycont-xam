using Android.Content;
using Android.Support.V4.View;
using AgilityContXam.Controls;
using AgilityContXam.Droid.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MaterialFrame), typeof(MaterialFrameRenderer))]

namespace AgilityContXam.Droid.Renderers
{
    public class MaterialFrameRenderer : Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer
    {
        public MaterialFrameRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null)
                return;

            UpdateElevation();
        }


        private void UpdateElevation()
        {
            var materialFrame = (MaterialFrame)Element;

            // we need to reset the StateListAnimator to override the setting of Elevation on touch down and release.
            Control.StateListAnimator = new Android.Animation.StateListAnimator();

            // set the elevation manually
            ViewCompat.SetElevation(this, materialFrame.Elevation);
            ViewCompat.SetElevation(Control, materialFrame.Elevation);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "Elevation")
            {
                UpdateElevation();
            }
        }
    }
}