using System.Collections.Generic;
using Xamarin.Forms.Internals;
using System.Linq;

namespace Xamarin.Forms
{
	public static class TabIndexExtensions
	{
		public static IDictionary<int, List<VisualElement>> GetTabIndexesOnParentPage(this VisualElement element, out int countChildrenWithTabStopWithoutThis)
		{
			countChildrenWithTabStopWithoutThis = 0;

			Element parentPage = element.Parent;
			while (parentPage != null && !(parentPage is Page))
				parentPage = parentPage.Parent;

			var descendantsOnPage = parentPage?.VisibleDescendants();
			if (descendantsOnPage == null)
				return null;

			var childrenWithTabStop = new List<VisualElement>();
			foreach (var descendant in descendantsOnPage)
			{
				if (descendant is VisualElement visualElement && visualElement.IsTabStop)
					childrenWithTabStop.Add(visualElement);
			}
			if (!childrenWithTabStop.Contains(element))
				return null;

			countChildrenWithTabStopWithoutThis = childrenWithTabStop.Count - 1;
			return childrenWithTabStop.GroupToDictionary(c => c.TabIndex);
		}

		public static VisualElement FindNextElement(this VisualElement element, bool forwardDirection, IDictionary<int, List<VisualElement>> tabIndexes, ref int tabIndex)
		{
			if (!tabIndexes.TryGetValue(tabIndex, out var tabGroup))
				return null;

			if (!forwardDirection)
			{
				// search prev element in same TabIndex group
				var prevSubIndex = tabGroup.IndexOf(element) - 1;
				if (prevSubIndex >= 0 && prevSubIndex < tabGroup.Count)
				{
					return tabGroup[prevSubIndex];
				}
				else // search prev element in prev TabIndex group
				{
					var smallerMax = int.MinValue;
					var tabIndexesMax = int.MinValue;
					foreach (var index in tabIndexes.Keys)
					{
						if (index < tabIndex && smallerMax < index)
							smallerMax = index;
						if (tabIndexesMax < index)
							tabIndexesMax = index;
					}
					tabIndex = smallerMax != int.MinValue ? smallerMax : tabIndexesMax;
					return tabIndexes[tabIndex][0];
				}
			}
			else // Forward
			{
				// search next element in same TabIndex group
				var nextSubIndex = tabGroup.IndexOf(element) + 1;
				if (nextSubIndex > 0 && nextSubIndex < tabGroup.Count)
				{
					return tabGroup[nextSubIndex];
				}
				else // search next element in next TabIndex group
				{
					var biggerMin = int.MaxValue;
					var tabIndexesMin = int.MaxValue;
					foreach (var index in tabIndexes.Keys)
					{
						if (index > tabIndex && biggerMin > index)
							biggerMin = index;
						if (tabIndexesMin > index)
							tabIndexesMin = index;
					}
					tabIndex = biggerMin != int.MaxValue ? biggerMin : tabIndexesMin;
					return tabIndexes[tabIndex][0];
				}
			}
		}

		public static VisualElement GetFirstElementByTabIndex(IDictionary<int, List<VisualElement>> tabIndexes)
		{
			if (tabIndexes == null || tabIndexes.Count == 0)
				return null;

			var minIndex = tabIndexes.Min(x => x.Key);

			var tabGroup = tabIndexes[minIndex];

			return tabGroup[0];
		}

		public static VisualElement GetFirstNonLayoutTabStop(IDictionary<int, List<VisualElement>> tabIndexes)
		{
			if (tabIndexes == null || tabIndexes.Count == 0)
				return null;

			var minIndex = tabIndexes.Min(x => x.Key);

			var tabGroup = tabIndexes[minIndex];

			for (int t = 0; t < tabGroup.Count; t++)
			{
				var ve = tabGroup[t];
				if (!(ve is Layout) && ve.IsTabStop)
					return ve;
			}

			return null;
		}
	}
}