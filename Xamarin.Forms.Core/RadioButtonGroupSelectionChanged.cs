namespace Xamarin.Forms
{
	internal class RadioButtonGroupSelectionChanged
	{ 
		public Element Scope { get; }

		public RadioButtonGroupSelectionChanged(Element scope) => Scope = scope;
	}
}