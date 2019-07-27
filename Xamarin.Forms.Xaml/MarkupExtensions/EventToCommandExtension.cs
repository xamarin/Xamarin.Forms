using System;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty(nameof(CommandPath))]
	[AcceptEmptyServiceProvider]
	public sealed class EventToCommandExtension : IMarkupExtension<EventToCommandSource>
	{
		public string CommandPath { get; set; }
		public object CommandParameter { get; set; }
		public string EventArgsParameterPath { get; set; }
		public IValueConverter EventArgsConverter { get; set; }
		public object EventArgsConverterParameter { get; set; }

		EventToCommandSource IMarkupExtension<EventToCommandSource>.ProvideValue(IServiceProvider serviceProvider)
			=> new EventToCommandSource(
				CommandPath, CommandParameter, EventArgsParameterPath, EventArgsConverter, EventArgsConverterParameter);

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
			=> (this as IMarkupExtension<EventToCommandSource>).ProvideValue(serviceProvider);
	}
}
