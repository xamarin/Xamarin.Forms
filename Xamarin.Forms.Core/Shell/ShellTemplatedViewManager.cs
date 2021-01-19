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
			DataTemplate viewTemplate,
			ref View localViewRef,
			object newViewData,
			Action<Element, int> OnChildRemoved,
			Action<Element> OnChildAdded)
		{
			if (viewTemplate == null)
			{
				SetView(ref localViewRef,
					newViewData as View,
					OnChildRemoved,
					OnChildAdded);
			}
		}

		public static void OnViewTemplateChanged(
			DataTemplate viewTemplate,
			ref View localViewRef,
			object newViewData,
			Action<Element, int> OnChildRemoved,
			Action<Element> OnChildAdded,
			Shell shell)
		{
			View newContentView = newViewData as View;
			if (viewTemplate != null)
			{ 
				newContentView = (View)viewTemplate.CreateContent(viewTemplate, shell);
			}

			OnViewDataChanged(
				viewTemplate,
				ref localViewRef,
				newContentView,
				OnChildRemoved,
				OnChildAdded);
		}
	}
}
