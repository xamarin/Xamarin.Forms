namespace Xamarin.Forms
{
	public class EventToCommandSource
	{
		public string CommandPath { get; }
		public object CommandParameter { get; }
		public string EventArgsParameterPath { get; }
		public IValueConverter EventArgsConverter { get; }
		public object EventArgsConverterParameter { get; }

		public EventToCommandSource(
			string commandPath,
			object commandParameter,
			string eventArgsParameterPath,
			IValueConverter eventArgsConverter,
			object eventArgsConverterParameter)
		{
			CommandPath = commandPath;
			CommandParameter = commandParameter;
			EventArgsParameterPath = eventArgsParameterPath;
			EventArgsConverter = eventArgsConverter;
			EventArgsConverterParameter = eventArgsConverterParameter;
		}
	}
}
