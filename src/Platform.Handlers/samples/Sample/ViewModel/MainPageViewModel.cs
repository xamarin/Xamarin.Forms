using System.Collections.Generic;
using System.Linq;
using Sample.Services;
using Xamarin.Platform;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.ViewModel
{
	public class MainPageViewModel : ViewModelBase
	{
		public MainPageViewModel() : this(new ITextService[] { App.Current.Services.GetService<ITextService>() })
		{
		}

		public MainPageViewModel(IEnumerable<ITextService> textServices)
		{
			//Last will be the native one, the first will be the cross platform
			ITextService textService = textServices.FirstOrDefault();
			Text = textService.GetText();
		}

		//public MainPageViewModel(ITextService textService)
		//{
		//	Text = textService.GetText();
		//}

		string _text;
		public string Text
		{
			get => _text;
			set => SetProperty(ref _text, value);
		}
	}
}
