using System;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_CarouselPageRenderer))]
	[Obsolete("CarouselPage is obsolete since 4.4. Use CarouselView")]
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
			var page = new ContentPage();
			if (item != null)
				page.Title = item.ToString();

			return page;
		}
	}
}
