using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.WPF;
using LayoutExtensions = Xamarin.Forms.Platform.WPF.LayoutExtensions;

namespace Xamarin.Forms.ControlGallery.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FormsApplicationPage
    {
        public MainWindow()
        {
            InitializeComponent();

            Forms.Init();
            var app = new Controls.App();

            var mdp = app.MainPage as MasterDetailPage;

            var detail = mdp?.Detail as NavigationPage;
            if (detail != null)
            {
                detail.Pushed += (sender, args) => {
                    var nncgPage = args.Page as NestedNativeControlGalleryPage;

                    if (nncgPage != null)
                    {
                        AddNativeControls(nncgPage);
                    }
                };
            }

            LoadApplication(app);
        }

        void AddNativeControls(NestedNativeControlGalleryPage page)
        {
            if (page.NativeControlsAdded)
            {
                return;
            }

            StackLayout sl = page.Layout;

            // Create and add a native TextBlock
            var originalText = "I am a native TextBlock";
            //var textBlock = new TextBlock
            //{
            //    Text = originalText,
            //    FontSize = 14,
            //    FontFamily = new FontFamily("HelveticaNeue")
            //};

            //sl?.Children.Add(textBlock);

            // Create and add a native Button 
            var button = new System.Windows.Controls.Button { Content = "Click to toggle font size", Height = 80 };
            //button.Click += (sender, args) => { textBlock.FontSize = textBlock.FontSize == 14 ? 24 : 14; };

            
            sl?.Children.Add(LayoutExtensions.ToView(button));

           
            // Add the misbehaving controls, one with a custom delegate for ArrangeOverrideDelegate

            page.NativeControlsAdded = true;
        }
    }
}
