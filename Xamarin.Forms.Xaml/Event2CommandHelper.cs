using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;

namespace Xamarin.Forms.Xaml
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class Event2CommandHelper
	{
		public static Delegate GetHandler(BindableObject owner, EventInfo eventInfo, EventToCommandSource eventToCommandSource)
		{
			SetCommandPropertyAndBinding(
				owner,
				eventToCommandSource.CommandPath,
				out BindableProperty bpEventToCommand);

			TrySetCommandParameterPropertyAndBinding(
				owner,
				eventToCommandSource.CommandParameter as Binding,
				out BindableProperty bpCommandParamBinding);

			return CreateEventHandler(eventInfo, eventToCommandSource, bpEventToCommand, bpCommandParamBinding, OnEventRaised);
		}

		static void SetCommandPropertyAndBinding(BindableObject owner, string commandPath, out BindableProperty bp)
		{
			bp = BindableProperty.CreateAttached(
				"eventToCommand",
				typeof(ICommand),
				typeof(BindableObject),
				null);

			var commandBinding = new Binding(commandPath, BindingMode.OneWay);
			owner.SetBinding(bp, commandBinding);
		}

		static void TrySetCommandParameterPropertyAndBinding(BindableObject owner, Binding commandParamBinding, out BindableProperty bp)
		{
			if (commandParamBinding == null)
			{
				bp = null;
				return;
			}

			bp = BindableProperty.CreateAttached(
				"commandParam",
				typeof(object),
				typeof(BindableObject),
				null);

			owner.SetBinding(bp, commandParamBinding);
		}

		static Delegate CreateEventHandler(
			EventInfo eventInfo,
			EventToCommandSource source,
			BindableProperty bpEventToCommand,
			BindableProperty bpCommandParameter,
			Action<object, EventArgs, EventToCommandSource, BindableProperty, BindableProperty> action)
		{
			ParameterExpression[] eventParams = eventInfo.EventHandlerType
				.GetRuntimeMethods().First(m => m.Name == "Invoke")
				.GetParameters()
				.Select(p => Expression.Parameter(p.ParameterType))
				.ToArray();

			var actionInvoke = action.GetType()
				.GetRuntimeMethods().First(m => m.Name == "Invoke");

			MethodCallExpression body = Expression.Call(
				Expression.Constant(action),
				actionInvoke,
				eventParams[0],
				eventParams[1],
				Expression.Constant(source),
				Expression.Constant(bpEventToCommand),
				Expression.Constant(bpCommandParameter, typeof(BindableProperty)));

			return Expression.Lambda(eventInfo.EventHandlerType, body, eventParams).Compile();
		}

		static void OnEventRaised(object sender, EventArgs eventArgs, EventToCommandSource source, BindableProperty bpEventToCommand, BindableProperty bpCommandParameter)
		{
			var command = (ICommand)((BindableObject)sender).GetValue(bpEventToCommand);
			if (command == null)
				return;

			object parameter;
			if (bpCommandParameter != null)
				parameter = ((BindableObject)sender).GetValue(bpCommandParameter);
			else
				parameter = source.CommandParameter;

			if (parameter == null && !string.IsNullOrEmpty(source.EventArgsParameterPath))
			{
				// Walk the ParameterPath for nested properties.
				string[] propertyPathParts = source.EventArgsParameterPath.Split('.');
				object propertyValue = eventArgs;
				foreach (var propertyPathPart in propertyPathParts)
				{
					PropertyInfo propInfo = propertyValue.GetType().GetRuntimeProperty(propertyPathPart);
					if (propInfo == null)
						throw new MissingMemberException($"Unable to find {source.EventArgsParameterPath}");

					propertyValue = propInfo.GetValue(propertyValue);
					if (propertyValue == null)
						break;
				}

				parameter = propertyValue;
			}

			if (parameter == null &&
				eventArgs != null &&
				eventArgs != EventArgs.Empty &&
				source.EventArgsConverter != null)
			{
				parameter = source.EventArgsConverter.Convert(
					eventArgs, typeof(object), source.EventArgsConverterParameter, CultureInfo.CurrentUICulture);
			}

			if (command.CanExecute(parameter))
				command.Execute(parameter);
		}
	}
}
