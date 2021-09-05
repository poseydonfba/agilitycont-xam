using AgilityContXam.Droid.Renderers;
using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

//[assembly: ExportRenderer(typeof(ViewCell), typeof(ViewCellTransparent))]
namespace AgilityContXam.Droid.Renderers
{
    public class ViewCellTransparent : ViewCellRenderer
    {
        protected override View GetCellCore(Cell item, View convertView, ViewGroup parent, Context context)
        {
            var cell = base.GetCellCore(item, convertView, parent, context);
            var listView = parent as Android.Widget.ListView;

            if (listView != null)
            {
                // Disable native cell selection color style - set as *Transparent*
                listView.SetSelector(Android.Resource.Color.Transparent);
                listView.CacheColorHint = Android.Graphics.Color.Transparent;
            }

            return cell;
        }
    }
}