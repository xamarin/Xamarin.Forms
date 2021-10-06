using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 13818,
		"[Bug] System.DivideByZeroException: Attempted to divide by zero. exception when opening the CarouselView in UWP",
		PlatformAffected.UWP)]
	public partial class Issue13818 : TestContentPage
	{
		Issue13818ViewModel _context = new Issue13818ViewModel();

		public Issue13818()
		{
#if APP
			InitializeComponent();
			BindingContext = _context;
#endif
		}

		protected override void Init()
		{
			_context.Init();
		}
	}

	public class Issue13818ViewModel : BindableObject
	{
		int _position = 0;

		public void Init()
		{
			for (int i = 0; i < 10; i++)
			{
				Numbers.Add(i);
			}

			Position = 4;

		}

		public int Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<int> Numbers { get; set; } = new ObservableCollection<int>();
	}
}