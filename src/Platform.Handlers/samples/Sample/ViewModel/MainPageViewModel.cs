using System.Collections.Generic;
using System.Linq;
using Sample.Services;

namespace Sample.ViewModel
{
	public class MainPageViewModel : ViewModelBase
	{
		
		public MainPageViewModel(IEnumerable<ITextService> textServices)
		{
			//Last will be the Native One, first the on this project
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
