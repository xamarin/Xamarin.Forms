namespace Xamarin.Forms
{
	/// <summary>
	/// this exists purely to mark classes using the legacy BorderRadius Property
	/// </summary>
	interface IBorderOrCornerRadius
	{
		/// <summary>
		/// Flag to prevent overwriting the value of CornerRadius
		/// </summary>
		bool cornerOrBorderRadiusSetting { get; set;}
	}
}