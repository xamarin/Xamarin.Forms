using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_CollectionViewRenderer))]
	public class CollectionView : ItemsView
	{
		public CollectionView()
		{
			Flags.VerifyCollectionView(constructorHint: nameof(CollectionView));
		}
	}
}
