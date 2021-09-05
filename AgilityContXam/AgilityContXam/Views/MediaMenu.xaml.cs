using Xamarin.Forms;
using System.Windows.Input;

namespace AgilityContXam.Views
{
    public partial class MediaMenu : ContentView
    {
        public ICommand CloseTappedCommand { get { return new Command((obj) => OnCloseMenu()); } }

        public MediaMenu()
        {
            InitializeComponent();
            var gestureRecognizer = new TapGestureRecognizer();
            gestureRecognizer.Command = CloseTappedCommand;
            CloseStack.GestureRecognizers.Add(gestureRecognizer);
        }

        public static readonly BindableProperty IsSlideOpenProperty =
            BindableProperty.Create("IsSlideOpen",
                                    typeof(bool),
                                    typeof(MediaMenu),
                                    false,
                                    BindingMode.TwoWay,
                                    propertyChanged: SlideOpenClose);

        public static readonly BindableProperty DefaultHeightProperty =
            BindableProperty.Create("DefaultHeight",
                                    typeof(double),
                                    typeof(MediaMenu),
                                    0.0D,
                                    BindingMode.TwoWay, null,
                                    propertyChanged: DefaultHeightChanged, propertyChanging: null, coerceValue: null, defaultValueCreator: null);

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create("ItemTemplate",
                                    typeof(StackLayout),
                                    typeof(MediaMenu),
                                    null,
                                    BindingMode.TwoWay, null,
                                    propertyChanged: StackLayoutAdded, propertyChanging: null, coerceValue: null, defaultValueCreator: null);

        public bool IsSlideOpen
        {
            get { return (bool)GetValue(IsSlideOpenProperty); }
            set { SetValue(IsSlideOpenProperty, value); }
        }

        public StackLayout ItemTemplate
        {
            get { return (StackLayout)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public double DefaultHeight
        {
            get { return (double)GetValue(DefaultHeightProperty); }
            set { SetValue(DefaultHeightProperty, value); }
        }


        private static async void SlideOpenClose(BindableObject bindable, object oldValue, object newValue)
        {
            if ((bool)newValue)
            {
                (bindable as MediaMenu).IsVisible = true;
                await (bindable as MediaMenu).TranslateTo(0, 0, 250, Easing.SinInOut);
                newValue = false;
            }
            else
            {
                await (bindable as MediaMenu).TranslateTo(0, App.Current.MainPage.Height, 250, Easing.SinInOut);
                (bindable as MediaMenu).IsVisible = false;
                newValue = true;
            }
        }

        private static void StackLayoutAdded(BindableObject bindable, object oldValue, object newValue)
        {
            if ((StackLayout)newValue != null)
            {
                //(bindable as MediaMenu).ParentStackLayout.Children.Add((StackLayout)newValue);
                (bindable as MediaMenu).ParentStackLayout.Content = (StackLayout)newValue;
            }
        }

        private static void DefaultHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as MediaMenu).IsVisible = false;
            (bindable as MediaMenu).TranslationY = (double)newValue;
        }

        private void OnCloseMenu()
        {
            IsSlideOpen = false;
        }
    }
}
