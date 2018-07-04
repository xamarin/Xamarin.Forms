using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Diagnostics;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2210, "[Enhancement] Add better life cycle events", PlatformAffected.All)]
	public class Issue2210 : TestContentPage
	{
		protected override void Init()
		{
			var button = new Button() { Text = "Button" };
			button.Loaded += (_, __) => Debug.WriteLine("[Button] >> loaded >>");
			button.Unloaded += (_, __) => Debug.WriteLine("[Button] << unloaded <<");
			button.BeforeAppearing += (_, __) => Debug.WriteLine("[Button] == BeforeAppearing ==");
			Content = new StackLayout()
			{
				Children = { button }
			};

			Loaded += (_, __) => Debug.WriteLine("[Page] >> loaded >>");
			Unloaded += (_, __) => Debug.WriteLine("[Page] << unloaded <<");
			BeforeAppearing += (_, __) => Debug.WriteLine("[Page] == BeforeAppearing ==");
			Appearing += (_, __) => Debug.WriteLine("[Page] ++ AppearING >>");
			Appeared += (_, __) => Debug.WriteLine("[Page] ++ AppearED ++");
			Disappearing += (_, __) => Debug.WriteLine("[Page] -- DisappearING >>");
			Disappeared += (_, __) => Debug.WriteLine("[Page] -- DisappearED --");
		}

		protected override void OnLoaded()
		{
			Debug.WriteLine("[Page] >> loaded >>");
			base.OnLoaded();
		}

		protected override void OnUnloaded()
		{
			Debug.WriteLine("[Page] << unloaded <<");
			base.OnUnloaded();
		}

		protected override void OnBeforeAppearing()
		{
			Debug.WriteLine("[Page] == BeforeAppearing ==");
			base.OnBeforeAppearing();
		}

		protected override void OnAppearing()
		{
			Debug.WriteLine("[Page] ++ AppearING >>");
			base.OnAppearing();
		}

		protected override void OnAppeared()
		{
			Debug.WriteLine("[Page] ++ AppearED ++");
			base.OnAppeared();
		}

		protected override void OnDisappearing()
		{
			Debug.WriteLine("[Page] -- DisappearING >>");
			base.OnDisappearing();
		}

		protected override void OnDisappeared()
		{
			Debug.WriteLine("[Page] -- DisappearED --");
			base.OnDisappeared();
		}		
	}
}