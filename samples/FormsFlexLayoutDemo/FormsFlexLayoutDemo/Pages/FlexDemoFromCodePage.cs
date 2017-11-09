using Xamarin.Forms;

namespace FormsFlexLayoutDemo
{
	public class FlexDemoFromCodePage : ContentPage
	{
		public FlexDemoFromCodePage()
		{
			var flexLayout = new Xamarin.Forms.FlexLayout();
			CreateViewHierarchy(flexLayout);
			Content = flexLayout;
		}

		static void CreateViewHierarchy(Xamarin.Forms.FlexLayout root)
		{
			root.BackgroundColor = Color.Red;

			root.SetBinding(Xamarin.Forms.FlexLayout.AlignItemsProperty, new Binding(nameof(FlexLayoutViewModel.SelectedAlignItems), BindingMode.TwoWay));
			root.SetBinding(Xamarin.Forms.FlexLayout.AlignContentProperty, new Binding(nameof(FlexLayoutViewModel.SelectedAlignContent), BindingMode.TwoWay));
			root.SetBinding(Xamarin.Forms.FlexLayout.AlignSelfProperty, new Binding(nameof(FlexLayoutViewModel.SelectedAlignSelf), BindingMode.TwoWay));
			root.SetBinding(Xamarin.Forms.FlexLayout.FlexDirectionProperty, new Binding(nameof(FlexLayoutViewModel.SelectedFlexDirection), BindingMode.TwoWay));
			root.SetBinding(Xamarin.Forms.FlexLayout.WrapProperty, new Binding(nameof(FlexLayoutViewModel.SelectedWrap), BindingMode.TwoWay));
			root.SetBinding(Xamarin.Forms.FlexLayout.JustifyContentProperty, new Binding(nameof(FlexLayoutViewModel.SelectedJustifyContent), BindingMode.TwoWay));

			var child1 = new BoxView
			{
				BackgroundColor = Color.Blue,
				WidthRequest = 100,
				HeightRequest = 100
			};

			var child2 = new StackLayout
			{
				BackgroundColor = Color.Green,
				WidthRequest = 200,
				HeightRequest = 100
			};

			var child3 = new BoxView
			{
				BackgroundColor = Color.Yellow,
				WidthRequest = 100,
				HeightRequest = 100,
				HorizontalOptions = LayoutOptions.Start
			};

			child2.Children.Add(child3);
			root.Children.Add(child1);
			root.Children.Add(child2);
		}
	}
}

