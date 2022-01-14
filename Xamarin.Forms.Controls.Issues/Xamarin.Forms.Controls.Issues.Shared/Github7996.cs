using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{

	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7996, "Xamarin.Forms.Entry does not enter decimal when binding a float/double and decimal to it", PlatformAffected.Default)]
	public class Issue7996 : TestContentPage
	{
		protected override void Init()
		{
			var vm = new ViewModelIssue7996();
			BindingContext = vm;
			var textLabel1 = new Label
			{
				Text = "Select culture:"
			};
			var picker = new Picker();
			picker.ItemsSource = new[]
			{
				new CultureInfo("de"),
				new CultureInfo("en"),
			};
			picker.SelectedIndexChanged += (s, e) =>
			{
				if (picker.SelectedItem is CultureInfo culture)
				{
					CultureInfo.CurrentUICulture = culture;
				}
			};
			
			var textLabel2 = new Label
			{
				Text = "Resolved Binding Value:"
			};
			var bindingLable = new Label();
			bindingLable.SetBinding(Label.TextProperty, new Binding(nameof(ViewModelIssue7996.MyDecimal), BindingMode.TwoWay));

			var textLabel3 = new Label
			{
				Text = "Actual Value:"
			};
			var bindingLable2 = new Label();
			bindingLable2.SetBinding(Label.TextProperty, new Binding(nameof(ViewModelIssue7996.MyDecimal), BindingMode.TwoWay));

			var textLabel4 = new Label
			{
				Text = "Enter a number and watch result:"
			};
			var entry = new Entry();
			entry.Text = vm.MyDecimal.ToString();
			entry.TextChanged += (s, e) =>
			{
				bindingLable.Text = e.NewTextValue;
			};

			var stackLayout = new StackLayout
			{
				Children =
				{
					textLabel1,
					picker,
					textLabel2,
					bindingLable,
					textLabel3,
					bindingLable2,
					textLabel4,
					entry,
				}
			};

			Content = stackLayout;
		}
	}

	[Preserve(AllMembers = true)]
	public class ViewModelIssue7996 : INotifyPropertyChanged
	{

		decimal? myDecimal = 4.2m;

		public event PropertyChangedEventHandler PropertyChanged;

		public decimal? MyDecimal
		{
			get
			{
				return myDecimal;
			}
			set
			{
				myDecimal = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MyDecimal)));
			}
		}

		public ViewModelIssue7996()
		{

		}
	}
}