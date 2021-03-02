using System.Collections.Generic;
using System.Linq;
using Maui.Controls.Sample.Services;
using Microsoft.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace Maui.Controls.Sample.ViewModel
{
	public class MainPageViewModel : ViewModelBase
	{
		string _text;
		readonly ITextService _textService;

		public MainPageViewModel() : this(new ITextService[] { Application.Current.Services.GetService<ITextService>() })
		{

		}

		public MainPageViewModel(IEnumerable<ITextService> textServices)
		{
			_textService = textServices.FirstOrDefault();
			Text = _textService.GetText();
		}

		public string Text
		{
			get => _text;
			set => SetProperty(ref _text, value);
		}
	}
}
