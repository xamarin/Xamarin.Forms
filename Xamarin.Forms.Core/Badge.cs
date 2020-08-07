namespace Xamarin.Forms.Core
{
	public class Badge
	{
		public static readonly BindableProperty BadgeTextProperty =
			BindableProperty.CreateAttached("BadgeText", typeof(string), typeof(Badge), null);

		public static string GetBadgeText(BindableObject obj)
		{
			return (string)obj.GetValue(BadgeTextProperty);
		}

		public static void SetBadgeText(BindableObject obj, string value)
		{
			obj.SetValue(BadgeTextProperty, value);
		}

		public static readonly BindableProperty BadgeTextColorProperty =
			BindableProperty.CreateAttached("BadgeTextColor", typeof(Color), typeof(Badge), null);

		public static Color GetBadgeTextColor(BindableObject obj)
		{
			return (Color)obj.GetValue(BadgeTextColorProperty);
		}

		public static void SetBadgeTextColor(BindableObject obj, Color value)
		{
			obj.SetValue(BadgeTextColorProperty, value);
		}

		public static readonly BindableProperty BadgeBackgroundProperty =
			BindableProperty.CreateAttached("BadgeBackground", typeof(Brush), typeof(Badge), null);

		public static Brush GetBadgeBackground(BindableObject obj)
		{
			return (Brush)obj.GetValue(BadgeBackgroundProperty);
		}

		public static void SetBadgeBackground(BindableObject obj, Brush value)
		{
			obj.SetValue(BadgeBackgroundProperty, value);
		}
	}
}