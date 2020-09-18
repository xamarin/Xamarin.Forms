using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 9691, "[Bug][iOS] Keyboard hides CollectionView elements",
		PlatformAffected.iOS)]
	public class Issue9691 : TestContentPage
	{
		string IssueInstructions = "1) Scroll down to the last Editor.\n" +
								   "2) If things are working you should be able to see the text as you type.";

		protected override void Init()
		{
			var collectionView = new CollectionView();
			collectionView.Header = new Label() {Text = IssueInstructions };
			collectionView.ItemTemplate = new DataTemplate(() => new Frame() { Padding = 20, Content = new Editor() });
			collectionView.ItemsSource = Enumerable.Repeat(0, 20).ToList();

			Content = collectionView;
		}
	}
}