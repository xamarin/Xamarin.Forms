using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Java.Lang;
using Xamarin.Forms.Internals;
using FragmentTransit = Android.App.FragmentTransit;

namespace Xamarin.Forms.Platform.Android.AppCompat
{
	internal class FormsFragmentPagerAdapter<T> : FragmentPagerAdapter where T : Page
	{
		MultiPage<T> _page;
		FragmentManager _fragmentManager;
		bool _disposed;

		public FormsFragmentPagerAdapter(MultiPage<T> page, FragmentManager fragmentManager) : base(fragmentManager)
		{
			_page = page;
			_fragmentManager = fragmentManager;
		}

		public override int Count => CountOverride;

		public int CountOverride { get; set; }

		public override Fragment GetItem(int position)
		{
			return FragmentContainer.CreateInstance(_page.Children[position]);
		}

		public override long GetItemId(int position)
		{
			return _page.Children[position].GetHashCode();
		}

		public override int GetItemPosition(Object objectValue)
		{
			var fragContainer = objectValue as FragmentContainer;
			if (fragContainer?.Page != null)
			{
				int index = _page.Children.IndexOf(fragContainer.Page);
				if (index >= 0)
					return index;
			}
			return PositionNone;
		}

		public override ICharSequence GetPageTitleFormatted(int position)
		{
			return new String(_page.Children[position].Title);
		}

		// http://stackoverflow.com/questions/18642890/fragmentstatepageradapter-with-childfragmentmanager-fragmentmanagerimpl-getfra/19099987#19099987
		public override void RestoreState(IParcelable state, ClassLoader loader)
		{
		}

		public override void DestroyItem(ViewGroup container, int position, Object item)
		{
			FragmentTransaction transaction = _fragmentManager.BeginTransactionEx();
		
			transaction.RemoveEx((Fragment)item);
			transaction.SetTransitionEx((int)FragmentTransit.None);
			transaction.CommitAllowingStateLossEx();

			base.DestroyItem(container, position, item);
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
			{
				return;
			}

			if (disposing)
			{
				_disposed = true;

				_page = null;
				_fragmentManager = null;
			}

			base.Dispose(disposing);
		}
	}
}