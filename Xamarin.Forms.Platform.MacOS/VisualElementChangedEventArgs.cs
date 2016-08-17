using System;
namespace Xamarin.Forms.Platform.MacOS
{
	public class VisualElementChangedEventArgs : ElementChangedEventArgs<VisualElement>
	{
		public VisualElementChangedEventArgs(VisualElement oldElement, VisualElement newElement) : base(oldElement, newElement)
		{
		}
	}
}

