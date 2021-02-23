using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests.Stubs
{
	public partial class PickerStub : StubBase, IPicker
	{
		public string Title { get; set; }

		public Color TextColor { get; set; }

		public Color TitleColor { get; set; }

		public IList<string> Items { get; set; } = new List<string>();

		public IList ItemsSource { get; set; }

		public int SelectedIndex { get; set; } = -1;

		public object SelectedItem { get; set; }
	}
}