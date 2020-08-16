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
		protected override void Init()
		{
			var collectionView = new CollectionView();
			collectionView.ItemTemplate = new DataTemplate(() => new Editor());
			collectionView.ItemsSource = Enumerable.Repeat(0, 20).ToList();

			Content = collectionView;
		}
	}
}