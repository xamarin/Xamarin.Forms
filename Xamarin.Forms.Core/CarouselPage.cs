using System;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_CarouselPageRenderer))]
	public class CarouselPage : MultiPage<ContentPage>, IElementConfiguration<CarouselPage>
	{
		readonly Lazy<PlatformConfigurationRegistry<CarouselPage>> _platformConfigurationRegistry;

		public CarouselPage()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<CarouselPage>>(() => new PlatformConfigurationRegistry<CarouselPage>(this));
		}

		public new IPlatformElementConfiguration<T, CarouselPage> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		protected override ContentPage CreateDefault(object item)
		{
			return (ContentPage)DataTemplateHelpers.DefaultPageTemplate.CreateContent();
		}
	}
}