using System;
using CoreGraphics;

namespace Xamarin.Forms.Platform.iOS
{
	internal class CarouselViewLayout : ItemsViewLayout
	{
		public CarouselViewLayout(ListItemsLayout itemsLayout) : base(itemsLayout)
		{
			
		}

		public override void ConstrainTo(CGSize size)
		{
			ItemSize = size;
			//Setting EstimatedSize here crashes, do we need it for perfomance reasons? 
		}
	}
}
