using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 9618, "[Bug] Changing Text of TableSection changes focus.", PlatformAffected.All)]
	public class Issue9618 : TestContentPage 
	{
		public TableSection TableSectionToChange { get; set; }
		public Editor EditorToFocus { get; set; }

		protected override void Init()
		{

			var tableView = new TableView();
			var tableRoot = new TableRoot();

			TableSectionToChange = new TableSection() { Title = "Change me" };
			var firstViewCell = new ViewCell();
			firstViewCell.View = new Label() { Text = "Hello" };

			TableSectionToChange.Add(firstViewCell);

			var editorTableSection = new TableSection() { Title = "Editor Title" };
			var editorViewCell = new ViewCell();
			EditorToFocus = new Editor();

			editorViewCell.View = EditorToFocus;
			editorTableSection.Add(editorViewCell);

			tableRoot.Add(TableSectionToChange);
			tableRoot.Add(editorTableSection);
			tableView.Root = tableRoot;

			Content = tableView;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			Device.BeginInvokeOnMainThread(async () =>
			{
				EditorToFocus.Focus();

				await Task.Delay(4000);

				TableSectionToChange.Title = "Did I really lose focus?";
			});
		}
	}
}