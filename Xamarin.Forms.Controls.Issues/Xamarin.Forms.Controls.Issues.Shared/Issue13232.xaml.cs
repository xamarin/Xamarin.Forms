using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13232,
		"[Bug] Command is not working in SwipeItem",
		PlatformAffected.Android | PlatformAffected.iOS)]
	public partial class Issue13232 : TestContentPage
	{
		public Issue13232()
		{
#if APP
			InitializeComponent();
			BindingContext = new Issue13232ViewModel();
#endif
		}

		protected override void Init()
		{

		}
	}

	[Preserve(AllMembers = true)]
	public class Issue13232Model
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string SubTitle { get; set; }
	}

	[Preserve(AllMembers = true)]
	public class Issue13232ViewModel : BindableObject
	{
		public Issue13232ViewModel()
		{
			Items = new ObservableCollection<Issue13232Model>();

			LoadItems();

			DeleteCommand = new Command(async (object model) =>
			{
				await Task.Delay(100);

				var issue13232Model = (Issue13232Model)model;

				var itemToDelete = Items.SingleOrDefault(t => t.Id == issue13232Model.Id);

				if (itemToDelete != null)
				{
					await Task.Delay(500);

					Items.Remove(itemToDelete);
				}
			});
		}

		public ObservableCollection<Issue13232Model> Items { get; set; }

		public ICommand DeleteCommand { get; }

		void LoadItems()
		{
			for (int i = 0; i < 10; i++)
			{
				Items.Add(new Issue13232Model
				{
					Id = i + 1,
					Title = $"Message {i + 1}",
					SubTitle = $"Description {i + 1}",
				});
			}
		}
	}
}