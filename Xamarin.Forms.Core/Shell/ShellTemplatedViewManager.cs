using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	public static class ShellTemplatedViewManager
	{
		public static void SetView(ref View localView, View newView, Action<Element, int> OnChildRemoved, Action<Element> OnChildAdded)
		{
			if (localView == newView)
				return;

			if (localView != null)
				OnChildRemoved(localView, -1);
			localView = newView;
			if (localView != null)
				OnChildAdded(localView);
		}


		public static void OnViewDataChanged(
			DataTemplate currentViewTemplate,
			ref View localViewRef,
			object newViewData,
			Action<Element, int> OnChildRemoved,
			Action<Element> OnChildAdded)
		{
			if (currentViewTemplate == null)
			{
				SetView(ref localViewRef,
					newViewData as View,
					OnChildRemoved,
					OnChildAdded);
			}
		}

		public static void OnViewTemplateChanged(
			DataTemplate newViewTemplate,
			ref View localViewRef,
			object currentViewData,
			Action<Element, int> OnChildRemoved,
			Action<Element> OnChildAdded,
			Shell shell)
		{
			View newContentView = currentViewData as View;
			if (newViewTemplate != null)
			{ 
				newContentView = (View)newViewTemplate.CreateContent(newViewTemplate, shell);
			}

			SetView(ref localViewRef,
				newContentView,
				OnChildRemoved,
				OnChildAdded);
		}
	}
}
