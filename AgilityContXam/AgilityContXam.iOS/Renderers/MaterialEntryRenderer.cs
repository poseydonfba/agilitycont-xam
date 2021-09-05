using AgilityContXam.Controls;
using AgilityContXam.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MaterialEntry), typeof(MaterialEntryRenderer))]
namespace AgilityContXam.iOS.Renderers
{
    public class MaterialEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {

                Control.BorderStyle = UITextBorderStyle.None;
                //Control.Layer.CornerRadius = 10;
                //Control.TextColor = UIColor.White;

            }
        }
    }
}