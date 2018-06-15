using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty(nameof(SegueExtension.Action))]
	public class SegueExtension : IMarkupExtension<ICommand>
	{
		/// <summary>
		/// Gets or sets the action that this segue will perform.
		/// </summary>
		/// <remarks>
		/// Values should correspond to values of the <see cref="NavigationAction"/> enumeration. 
		/// </remarks>
		public string Action { get; set; }

		/// <summary>
		/// Gets or sets the type of custom segue to be created.
		/// </summary>
		/// Gets or sets a value indicating whether this segue is animated when executed.
		/// </summary>
		/// <value><c>true</c> if animated (default); otherwise, <c>false</c>.</value>
		public bool IsAnimated { get; set; }

		public SegueExtension()
		{
			IsAnimated = true;
		}

		public ICommand ProvideValue(IServiceProvider serviceProvider)
		{
			var targetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (targetProvider == null)
				throw new ArgumentException();

			var target = targetProvider.TargetObject as Element;
			if (target == null)
				XamlError("Segue may only be used on Elements", serviceProvider);

			var action = default(NavigationAction);
			if (Action != null && !NavigationActionConverter.TryParse(Action, out action))
				XamlError($"Unknown segue action \"{Action}\"", serviceProvider);

			return Segue.CreateCommand(target, action, IsAnimated);
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);

		static void XamlError(string message, IServiceProvider serviceProvider)
		{
			var lineInfoProvider = serviceProvider.GetService(typeof(IXmlLineInfoProvider)) as IXmlLineInfoProvider;
			var lineInfo = lineInfoProvider?.XmlLineInfo ?? new XmlLineInfo();
			throw new XamlParseException(message, lineInfo);
		}
	}
}
