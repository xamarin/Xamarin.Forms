using System;

namespace Xamarin.Forms
{
	interface ILetterSpacingElement
	{
		//note to implementor: implement this property publicly
		double LetterSpacing { get; }

		//note to implementor: but implement this method explicitly
		void OnLetterSpacingChanged(double oldValue, double newValue);
	}
}