using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Windows.Input;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 32899, "Setting button's IsEnabled to false does not disable button", PlatformAffected.Android)]
	public class Bugzilla32899 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		const string ButtonScanID = "btnScan";
		protected override void Init()
		{
			BindingContext = new ViewModelTest();

			var lbl = new Label
			{

			};

			lbl.SetBinding(Label.TextProperty, nameof(ViewModelTest.LblText));


			var btn1 = new Button
			{
				Text = "Change IsEnabled"
			};

			btn1.SetBinding(Button.CommandProperty, nameof(ViewModelTest.IsEnableCommand));

			var btn = new Button
			{
				Text = "Scan",
				AutomationId = ButtonScanID,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center

			};

			//order is important here 
			btn.SetBinding(VisualElement.IsEnabledProperty, nameof(ViewModelTest.ShouldBeEnabled));
			btn.SetBinding(Button.CommandProperty, nameof(ViewModelTest.StopScanningCommand));
		
			var layout = new StackLayout();
			layout.Children.Add(lbl);
			layout.Children.Add(btn1);
			layout.Children.Add(btn);
			Content = layout;
		}

		[Preserve(AllMembers = true)]
		class ViewModelTest : ViewModel
		{
			public const string Sucess = "Sucess";
			public ViewModelTest()
			{
				ShouldBeEnabled = false;
				StopScanningCommand = new Command((obj) =>
				{
					 LblText = Sucess;
				});

				IsEnableCommand = new Command((obj) => {
					ShouldBeEnabled = !ShouldBeEnabled;
				});
			}

			ICommand _stopScanningCommand;
			public ICommand StopScanningCommand
			{
				get { return _stopScanningCommand; }
				set
				{
					_stopScanningCommand = value;
					OnPropertyChanged();
				}
			}

			ICommand _isEnableCommand;
			public ICommand IsEnableCommand
			{
				get { return _isEnableCommand; }
				set
				{
					_isEnableCommand = value;
					OnPropertyChanged();
				}
			}

			string _lblText;
			public string LblText
			{
				get { return _lblText; }
				set
				{
					_lblText = value;
					OnPropertyChanged();
				}
			}
		}

#if UITEST
		[Test]
		public void Bugzilla32899Test ()
		{
			RunningApp.WaitForElement (q => q.Marked (ButtonScanID));
			RunningApp.Tap(q => q.Marked(ButtonScanID));
			RunningApp.WaitForNoElement (ViewModelTest.Sucess);
		}
#endif
	}
}