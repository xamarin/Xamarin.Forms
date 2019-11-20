namespace Xamarin.Forms
{
	static class StatusBarElement
	{
		public static readonly BindableProperty StatusBarColorProperty = BindableProperty.Create(nameof(IStatusBarElement.StatusBarColor), typeof(Color), typeof(IStatusBarElement), Color.Default);

		public static readonly BindableProperty StatusBarStyleProperty = BindableProperty.Create(nameof(IStatusBarElement.StatusBarStyle), typeof(StatusBarStyle), typeof(IStatusBarElement), StatusBarStyle.Default);
	}
}
