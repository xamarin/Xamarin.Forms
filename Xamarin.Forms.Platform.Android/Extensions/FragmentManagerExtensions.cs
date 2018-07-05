using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace Xamarin.Forms.Platform.Android
{
	// This is a way to centralize all fragment modifications which makes it a lot easier to debug
	internal static class FragmentManagerExtensions
	{
		public static FragmentTransaction RemoveEx(this FragmentTransaction @this, Fragment fragment)
		{
			return @this.Remove(fragment);
		}

		public static FragmentTransaction AddEx(this FragmentTransaction @this, int containerViewId, Fragment fragment)
		{
			return @this.Add(containerViewId, fragment);
		}

		public static FragmentTransaction HideEx(this FragmentTransaction @this, Fragment fragment)
		{
			return @this.Hide(fragment);
		}

		public static FragmentTransaction ShowEx(this FragmentTransaction @this, Fragment fragment)
		{
			return @this.Show(fragment);
		}

		public static FragmentTransaction SetTransitionEx(this FragmentTransaction @this, int transit)
		{
			return @this.SetTransition(transit);
		}

		public static int CommitAllowingStateLossEx(this FragmentTransaction @this)
		{
			return @this.CommitAllowingStateLoss();
		}

		public static bool ExecutePendingTransactionsEx(this FragmentManager @this)
		{
			return @this.ExecutePendingTransactions();
		}

		public static FragmentTransaction BeginTransactionEx(this FragmentManager @this)
		{
			return @this.BeginTransaction();
		}
	}
}