using Android.Content;
using Android.OS;
using Android.Views;
using LP = Android.Views.ViewGroup.LayoutParams;
using AView = Android.Views.View;

namespace Xamarin.Forms.Platform.Android.AppCompat
{
	internal class ShellFragmentContainer : FragmentContainer
	{
		Page _page;

		public ShellContent ShellContentTab { get; private set; }

		public ShellFragmentContainer(ShellContent shellContent) : base()
		{
			ShellContentTab = shellContent;
		}

		public override Page Page => _page;

		protected override PageContainer CreatePageContainer(Context context, IVisualElementRenderer child, bool inFragment)
		{
			return new ShellPageContainer(context, child, inFragment)
			{
				LayoutParameters = new LP(LP.MatchParent, LP.MatchParent)
			};
		}

		public override AView OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			_page = ((IShellContentController)ShellContentTab).GetOrCreateContent();
			return base.OnCreateView(inflater, container, savedInstanceState);
		}

		public override void OnDestroyView()
		{
			((IShellContentController)ShellContentTab).RecyclePage(_page);
			_page = null;

			base.OnDestroyView();
		}
	}
}