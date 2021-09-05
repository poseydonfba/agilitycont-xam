using Android.Content;
using Android.Graphics;
using Android.Support.V4.View;
using AgilityContXam.Controls;
using AgilityContXam.Droid.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

//[assembly: ExportRenderer(typeof(Xamarin.Forms.Button), typeof(MaterialButtonRenderer))]
[assembly: ExportRenderer(typeof(MaterialButton), typeof(MaterialButtonRenderer))]

namespace AgilityContXam.Droid.Renderers
{
    //public class ElevatedButtonRenderer : Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer
    public class MaterialButtonRenderer : Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer
    {
        public MaterialButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null)
                return;

            var materialButton = (MaterialButton)Element;

            // we need to reset the StateListAnimator to override the setting of Elevation on touch down and release.
            Control.StateListAnimator = new Android.Animation.StateListAnimator();

            // set the elevation manually
            ViewCompat.SetElevation(this, materialButton.Elevation);
            ViewCompat.SetElevation(Control, materialButton.Elevation);
        }

        public override void Draw(Canvas canvas)
        {
            var materialButton = (MaterialButton)Element;
            Control.Elevation = materialButton.Elevation;
            base.Draw(canvas);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "Elevation")
            {
                var materialButton = (MaterialButton)Element;
                ViewCompat.SetElevation(this, materialButton.Elevation);
                ViewCompat.SetElevation(Control, materialButton.Elevation);
                UpdateLayout();
            }
        }
    }
}