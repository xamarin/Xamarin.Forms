using Xamarin.Platform;

namespace Xamarin.Forms
{
	[ContentProperty("Content")]
	public partial class ContentPage : TemplatedPage
	{
		public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(IView), typeof(ContentPage), null, propertyChanged: TemplateUtilities.OnContentChanged);

		public IView Content
		{
			get { return (IView)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			View content = Content as View;
			ControlTemplate controlTemplate = ControlTemplate;
			if (content != null && controlTemplate != null)
			{
				SetInheritedBindingContext(content, BindingContext);
			}
		}

		internal override void OnControlTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue)
		{
			if (oldValue == null)
				return;

			base.OnControlTemplateChanged(oldValue, newValue);
			View content = Content as View;
			ControlTemplate controlTemplate = ControlTemplate;
			if (content != null && controlTemplate != null)
			{
				SetInheritedBindingContext(content, BindingContext);
			}
		}
	}
}