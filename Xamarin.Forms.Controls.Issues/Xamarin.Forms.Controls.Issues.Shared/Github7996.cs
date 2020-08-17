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
			var entry = new Entry();
			entry.SetBinding(Entry.TextProperty, new Binding(nameof(ViewModelIssue7996.MyDecimal), BindingMode.TwoWay));

			var stackLayout = new StackLayout
			{
				Children =
				{
					picker,
					entry
				}
			};

			Content = stackLayout;
			BindingContext = new ViewModelIssue7996();
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