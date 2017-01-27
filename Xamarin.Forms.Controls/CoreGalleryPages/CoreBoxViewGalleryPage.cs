using System;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls
{
	internal class CoreBoxViewGalleryPage : CoreGalleryPage<BoxView>
	{
		static readonly object SyncLock = new object();
		static readonly Random Rand = new Random();

		protected override bool SupportsFocus
		{
			get { return false; }
		}

		protected override void Build(StackLayout stackLayout)
		{
			base.Build(stackLayout);

			var colorContainer = new ViewContainer<BoxView>(Test.BoxView.Color, new BoxView { Color = Color.Pink });

			Add(colorContainer);
		}

		protected override void InitializeElement(BoxView element)
		{
			lock (SyncLock)
			{
				double red = Rand.NextDouble();
				double green = Rand.NextDouble();
				double blue = Rand.NextDouble();
				element.Color = new Color(red, green, blue);
			}
		}
	}
}