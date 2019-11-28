using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public static class ViewExtensions
	{
		public static IEnumerable<Page> GetParentPages(this Page target)
		{
			var result = new List<Page>();
			var parent = target.RealParent as Page;
			while (!Application.IsApplicationOrNull(parent))
			{
				result.Add(parent);
				parent = parent.RealParent as Page;
			}

			return result;
		}

		internal static T FindParentOfType<T>(this VisualElement element)
		{
			var navPage = element.GetParentsPath()
										.OfType<T>()
										.FirstOrDefault();
			return navPage;
		}

		internal static IEnumerable<Element> GetParentsPath(this VisualElement self)
		{
			Element current = self;

			while (!Application.IsApplicationOrNull(current.RealParent))
			{
				current = current.RealParent;
				yield return current;
			}
		}

#if __MOBILE__
		public static UILineBreakMode GetNativeLineBreakMode(this LineBreakMode lineBreakMode)
		{
			switch (lineBreakMode)
			{
				case LineBreakMode.NoWrap:
					return UILineBreakMode.Clip;					
				case LineBreakMode.WordWrap:
					return UILineBreakMode.WordWrap;					
				case LineBreakMode.CharacterWrap:
					return UILineBreakMode.CharacterWrap;					
				case LineBreakMode.HeadTruncation:
					return UILineBreakMode.HeadTruncation;					
				case LineBreakMode.MiddleTruncation:
					return UILineBreakMode.MiddleTruncation;					
				case LineBreakMode.TailTruncation:
					return UILineBreakMode.TailTruncation;					
			}

			return UILineBreakMode.Clip;
		}
#else
		public static NSLineBreakMode GetNativeLineBreakMode(this LineBreakMode lineBreakMode)
		{
			switch (lineBreakMode)
			{
				case LineBreakMode.NoWrap:
					return NSLineBreakMode.Clipping;					
				case LineBreakMode.WordWrap:
					return NSLineBreakMode.ByWordWrapping;					
				case LineBreakMode.CharacterWrap:
					return NSLineBreakMode.CharWrapping;					
				case LineBreakMode.HeadTruncation:
					return NSLineBreakMode.TruncatingHead;					
				case LineBreakMode.MiddleTruncation:
					return NSLineBreakMode.TruncatingMiddle;					
				case LineBreakMode.TailTruncation:
					return NSLineBreakMode.TruncatingTail;					
			}

			return NSLineBreakMode.Clipping;
		}
#endif

	}
}