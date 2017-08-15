using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public sealed class VisualElementAttribute : Attribute { }

	public class ElementChangedEventArgs : EventArgs
	{
		public ElementChangedEventArgs(Element oldElement, Element newElement)
		{
			OldElement = oldElement;
			NewElement = newElement;
		}

		public Element NewElement { get; private set; }

		public Element OldElement { get; private set; }
	}

	public interface IPlatformRenderer<out T> : IDisposable
	{
		VisualElement Element { get; }

		void SetElement(VisualElement element);

		SizeRequest Measure(double widthConstraint, double heightConstraint);

		T Control { get; }

		event EventHandler<ElementChangedEventArgs> ElementChanged;
	}
}
