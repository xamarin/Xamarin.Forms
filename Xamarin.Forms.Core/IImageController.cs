using System;

namespace Xamarin.Forms
{
	public interface IImageController : IViewController
	{
		void SetIsLoading(bool isLoading);
		void RaiseImageSourcePropertyChanged();
		void OnImageSourcesSourceChanged(object sender, EventArgs e);
	}
}