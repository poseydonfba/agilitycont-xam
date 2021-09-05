using System;
using System.ComponentModel;
using System.Reflection;
using Android.Content;
using Android.Support.V4.App;
using AgilityContXam.Controls;
using AgilityContXam.Droid.Renderers;
using AgilityContXam.Enums;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;
using AToolbar = Android.Support.V7.Widget.Toolbar;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(TransitionNavigationPage), typeof(TransitionNavigationPageRenderer))]

namespace AgilityContXam.Droid.Renderers
{
    public class TransitionNavigationPageRenderer : NavigationPageRenderer, Android.Views.View.IOnClickListener
    {
        private TransitionType _transitionType = TransitionType.SlideFromRight;

        public TransitionNavigationPageRenderer(Context context) : base(context)
        {
        }

        #region TRANSITION

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Controls.TransitionNavigationPage.TransitionTypeProperty.PropertyName)
                UpdateTransitionType();
        }

        protected override void SetupPageTransition(FragmentTransaction transaction, bool isPush)
        {
            switch (_transitionType)
            {
                case TransitionType.None:
                    return;
                case TransitionType.Default:
                    return;
                case TransitionType.Fade:
                    transaction.SetCustomAnimations(Resource.Animation.fade_in, Resource.Animation.fade_out,
                                                    Resource.Animation.fade_out, Resource.Animation.fade_in);
                    break;
                case TransitionType.Flip:
                    transaction.SetCustomAnimations(Resource.Animation.flip_in, Resource.Animation.flip_out,
                                                    Resource.Animation.flip_out, Resource.Animation.flip_in);
                    break;
                case TransitionType.Scale:
                    transaction.SetCustomAnimations(Resource.Animation.scale_in, Resource.Animation.scale_out,
                                                    Resource.Animation.scale_out, Resource.Animation.scale_in);
                    break;
                case TransitionType.SlideFromLeft:
                    if (isPush)
                    {
                        transaction.SetCustomAnimations(Resource.Animation.enter_left, Resource.Animation.exit_right,
                                                        Resource.Animation.enter_right, Resource.Animation.exit_left);
                    }
                    else
                    {
                        transaction.SetCustomAnimations(Resource.Animation.enter_right, Resource.Animation.exit_left,
                                                        Resource.Animation.enter_left, Resource.Animation.exit_right);
                    }
                    break;
                case TransitionType.SlideFromRight:
                    if (isPush)
                    {
                        transaction.SetCustomAnimations(Resource.Animation.enter_right, Resource.Animation.exit_left,
                                                        Resource.Animation.enter_left, Resource.Animation.exit_right);
                    }
                    else
                    {
                        transaction.SetCustomAnimations(Resource.Animation.enter_left, Resource.Animation.exit_right,
                                                        Resource.Animation.enter_right, Resource.Animation.exit_left);
                    }
                    break;
                case TransitionType.SlideFromTop:
                    if (isPush)
                    {
                        transaction.SetCustomAnimations(Resource.Animation.enter_top, Resource.Animation.exit_bottom,
                                                        Resource.Animation.enter_bottom, Resource.Animation.exit_top);
                    }
                    else
                    {
                        transaction.SetCustomAnimations(Resource.Animation.enter_bottom, Resource.Animation.exit_top,
                                                        Resource.Animation.enter_top, Resource.Animation.exit_bottom);
                    }
                    break;
                case TransitionType.SlideFromBottom:
                    if (isPush)
                    {
                        transaction.SetCustomAnimations(Resource.Animation.enter_bottom, Resource.Animation.exit_top,
                                                        Resource.Animation.enter_top, Resource.Animation.exit_bottom);
                    }
                    else
                    {
                        transaction.SetCustomAnimations(Resource.Animation.enter_top, Resource.Animation.exit_bottom,
                                                        Resource.Animation.enter_bottom, Resource.Animation.exit_top);
                    }
                    break;
                default:
                    return;
            }
        }

        private void UpdateTransitionType()
        {
            var transitionNavigationPage = (Controls.TransitionNavigationPage)Element;
            _transitionType = transitionNavigationPage.TransitionType;
        }

        #endregion

        #region nav toolbar

        private static readonly FieldInfo ToolbarFieldInfo;

        private bool _disposed;
        private AToolbar _toolbar;

        static TransitionNavigationPageRenderer()
        {
            // get _toolbar private field info
            ToolbarFieldInfo = typeof(NavigationPageRenderer).GetField("_toolbar",
                    BindingFlags.NonPublic | BindingFlags.Instance)
                ;
        }

        public void OnClick(Android.Views.View v)
        {
            // Call the NavigationPage which will trigger the default behavior
            // The default behavior is to navigate back if the Page derived classes return true from OnBackButtonPressed override            
            var curPage = Element.CurrentPage as BasePage;
            if (curPage == null)
            {
                Element.PopAsync();
            }
            else
            {
                if (curPage.NeedOverrideSoftBackButton)
                    curPage.OnSoftBackButtonPressed();
                else Element.PopAsync();
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            UpdateToolbarInstance();

            //if (!changed) return;

            //if (!App.Current.Resources.Keys.Contains("Menu_Accent_Color")) return;
            //var tintColor = (Android.Graphics.Color)App.Current.Resources["Menu_Accent_Color"];

            //var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //for (var i = 0; i < toolbar.ChildCount; i++)
            //{
            //    if (toolbar.GetChildAt(i) is ImageButton)
            //    {
            //        var imageButton = (ImageButton)toolbar.GetChildAt(i);
            //        imageButton.SetColorFilter(tintColor.ToAndroid(), PorterDuff.Mode.SrcIn);
            //    }
            //}
        }

        protected override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            UpdateToolbarInstance();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                RemoveToolbarInstance();
            }

            base.Dispose(disposing);
        }

        private void UpdateToolbarInstance()
        {
            RemoveToolbarInstance();
            GetToolbarInstance();
        }

        private void GetToolbarInstance()
        {
            try
            {
                //sai o cho nay nay
                //how to get toolbar navigation page
                _toolbar = (AToolbar)ToolbarFieldInfo.GetValue(this);
                //var mi = t.GetMethod(“BarOnNavigationClick”, BindingFlags.NonPublic | BindingFlags.Instance);
                _toolbar.SetNavigationOnClickListener(this);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Can't get toolbar with error: {exception.Message}");
            }
        }

        private void RemoveToolbarInstance()
        {
            if (_toolbar == null) return;
            _toolbar.SetNavigationOnClickListener(null);
            _toolbar = null;
        }

        #endregion

        public AToolbar toolbar;
        public Android.App.Activity context;

        protected override Task<bool> OnPushAsync(Page view, bool animated)
        {
            var retVal = base.OnPushAsync(view, animated);

            context = (Android.App.Activity)Xamarin.Forms.Forms.Context;
            toolbar = context.FindViewById<Android.Support.V7.Widget.Toolbar>(Droid.Resource.Id.toolbar);

            if (toolbar != null)
            {
                if (toolbar.NavigationIcon != null)
                {
                    toolbar.NavigationIcon = Android.Support.V4.Content.ContextCompat.GetDrawable(context, Resource.Drawable.BackBlack);
                    //toolbar.SetNavigationIcon(Resource.Drawable.Back);
                }
            }
            return retVal;
        }

        //protected override Task<bool> OnPopViewAsync(Page page, bool animated)
        //{
        //    var retVal = base.OnPopViewAsync(page, animated);

        //    context = (Android.App.Activity)Xamarin.Forms.Forms.Context;
        //    toolbar = context.FindViewById<Android.Support.V7.Widget.Toolbar>(Droid.Resource.Id.toolbar);

        //    if (toolbar != null)
        //    {
        //        if (toolbar.NavigationIcon != null)
        //        {
        //            toolbar.NavigationIcon = Android.Support.V4.Content.ContextCompat.GetDrawable(context, Resource.Drawable.BackBlack);
        //            //toolbar.SetNavigationIcon(Resource.Drawable.Back);
        //        }
        //    }
        //    return retVal;
        //}
    }
}