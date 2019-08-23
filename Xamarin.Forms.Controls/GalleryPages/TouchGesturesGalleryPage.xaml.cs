using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TouchGesturesGalleryPage : ContentPage
	{
		int _count;
		ObservableCollection<string> _logs = new ObservableCollection<string>();
		public TouchGesturesGalleryPage()
		{
			InitializeComponent();
			LogListView.ItemsSource = _logs;
		}

		void TouchGestureRecognizer_OnTouchUpdated(object sender, TouchEventArgs e)
		{
			var logItem = $"N:{++_count}, Index={e.Id}, Event:{e.TouchState};";
			_logs.Insert(0, logItem);
			LogListView.ScrollTo(logItem,ScrollToPosition.MakeVisible,true);
		}
	}
}