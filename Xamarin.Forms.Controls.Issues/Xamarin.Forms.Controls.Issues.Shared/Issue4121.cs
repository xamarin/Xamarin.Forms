using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4121, "TextDecorations aren't unset by a DataTrigger")]
	public class Issue4121 : TestContentPage
	{
		protected override void Init()
		{
			Grid grid = new Grid { Margin = new Thickness(20) };
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
			grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

			ScrollView scrollView = new ScrollView();
			Label label = new Label
			{
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				Text = "The switch should toggle underline on this Label."
			};
			scrollView.Content = label;

			Switch underlineSwitch = new Switch { VerticalOptions = LayoutOptions.Center };
			Label underlineLabel = new Label { Text = "Underline:", VerticalOptions = LayoutOptions.Center };
			var underlineTrigger = new DataTrigger(typeof(Label));
			underlineTrigger.Value = true;
			underlineTrigger.Binding = new Binding("IsToggled", BindingMode.Default, null, null, null, underlineSwitch);
			underlineTrigger.Setters.Add(new Setter() { Property = Label.TextDecorationsProperty, Value = TextDecorations.Underline });

			label.Triggers.Add(underlineTrigger);

			grid.Children.Add(underlineLabel, 0, 0);
			grid.Children.Add(underlineSwitch, 1, 0);
			grid.Children.Add(scrollView, 0, 2);
			Grid.SetColumnSpan(scrollView, 4);

			Content = grid;
		}
	}
}
