namespace Microsoft.Maui
{
	public interface IButton : IView, IText, IBorder
	{
		void Pressed();
		void Released();
		void Clicked();
	}
}