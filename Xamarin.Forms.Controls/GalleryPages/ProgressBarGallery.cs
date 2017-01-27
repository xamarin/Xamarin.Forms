namespace Xamarin.Forms.Controls
{
	public class ProgressBarGallery : ContentPage
	{
		readonly StackLayout _stack;

		public ProgressBarGallery()
		{
			_stack = new StackLayout();

			var normal = new ProgressBar
			{
				Progress = 0.24
			};

			Content = _stack;

			_stack.Children.Add(normal);
		}
	}
}