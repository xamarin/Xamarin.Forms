using System;

using Xamarin.Forms;

namespace Xamarin.Forms
{
	[TypeConverter(typeof(NavigationActionConverter))]
	public enum NavigationAction
	{
		/// <summary>
		/// Corresponds to calling <see cref="INavigation.ShowAsync"/>
		/// </summary>
		Show = 0,

		/// <summary>
		/// Corresponds to calling <see cref="INavigation.PushAsync"/>.
		/// </summary>
		Push = 1,

		/// <summary>
		/// Corresponds to calling <see cref="INavigation.PushModalAsync"/>.
		/// </summary>
		Modal = 2,

		/// <summary>
		/// Corresponds to setting <see cref="Application.MainPage"/>.
		/// </summary>
		MainPage = 3,

		/// <summary>
		/// Corresponds to calling either <see cref="INavigation.PopAsync"/> or
		///  <see cref="INavigation.PopModalAsync"/>, as appropriate.
		/// </summary>
		Pop = 4,

		/// <summary>
		/// Corresponds to calling <see cref="INavigation.PopAsync"/>.
		/// </summary>
		PopPushed = Pop | Push,

		/// <summary>
		/// Corresponds to calling <see cref="INavigation.PopModalAsync"/>.
		/// </summary>
		PopModal = Pop | Modal,

		/// <summary>
		/// Corresponds to calling <see cref="INavigation.PopToRootAsync"/>.
		/// </summary>
		PopToRoot = Pop | MainPage,
	}

	public class NavigationActionConverter : TypeConverter
	{
		public static bool TryParse(string value, out NavigationAction action)
		{
			switch (value.ToLowerInvariant()) {
			case "show"     : action = NavigationAction.Show; return true;
			case "push"     : action = NavigationAction.Push; return true;
			case "modal"    : action = NavigationAction.Modal; return true;
			case "mainpage" : action = NavigationAction.MainPage; return true;
			case "pop"      : action = NavigationAction.Pop; return true;
			case "poppushed": action = NavigationAction.PopPushed; return true;
			case "popmodal" : action = NavigationAction.PopModal; return true;
			case "poptoroot": action = NavigationAction.PopToRoot; return true;
			}
			action = default(NavigationAction);
			return false;
		}

		public static NavigationAction Parse(string value)
		{
			NavigationAction result;
			if (!TryParse(value, out result))
				throw new ArgumentException();
			return result;
		}

		public override object ConvertFromInvariantString(string value) => Parse(value);
	}
}
