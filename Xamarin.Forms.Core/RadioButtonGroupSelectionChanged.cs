namespace Xamarin.Forms
{
	internal class RadioButtonGroupSelectionChanged
	{ 
		public Element Scope { get; }

		public RadioButtonGroupSelectionChanged(Element scope) => Scope = scope;
	}

	internal class RadioButtonGroupNameChanged
	{
		public Element Scope { get; }
		public string OldName { get; }

		public RadioButtonGroupNameChanged(Element scope, string oldName) 
		{
			Scope = scope;
			OldName = oldName;
		}
	}
}