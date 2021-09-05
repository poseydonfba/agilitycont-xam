using Android.Content;
using Android.Graphics.Drawables;
using Android.Text;
using AgilityContXam.Controls;
using AgilityContXam.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MaterialEntry), typeof(MaterialEntryRenderer))]
namespace AgilityContXam.Droid.Renderers
{
    public class MaterialEntryRenderer : EntryRenderer
    {
        public MaterialEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(global::Android.Graphics.Color.Transparent);
                this.Control.SetBackgroundDrawable(gd);
                this.Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
                //Control.SetHintTextColor(ColorStateList.ValueOf(global::Android.Graphics.Color.White));
            }
        }
    }
}