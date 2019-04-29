using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;

namespace Xamarin.Forms.Core.Interactivity
{
	public class EventToCommandBehavior : Behavior
	{
		public static readonly BindableProperty EventNameProperty =
			BindableProperty.Create(nameof(EventName), typeof(string), typeof(EventToCommandBehavior));

		public static readonly BindableProperty CommandProperty =
			BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(EventToCommandBehavior));

		public static readonly BindableProperty CommandParameterProperty =
			BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(EventToCommandBehavior));

		public static readonly BindableProperty EventArgsConverterProperty =
			BindableProperty.Create(nameof(EventArgsConverter), typeof(IValueConverter), typeof(EventToCommandBehavior));

		public static readonly BindableProperty EventArgsConverterParameterProperty =
			BindableProperty.Create(nameof(EventArgsConverterParameter), typeof(object), typeof(EventToCommandBehavior));

		public static readonly BindableProperty EventArgsParameterPathProperty =
			BindableProperty.Create(
				nameof(EventArgsParameterPath),
				typeof(string),
				typeof(EventToCommandBehavior));

		protected EventInfo _eventInfo;
		protected Delegate _handler;

		public string EventArgsParameterPath
		{
			get { return (string)GetValue(EventArgsParameterPathProperty); }
			set { SetValue(EventArgsParameterPathProperty, value); }
		}

		public string EventName
		{
			get { return (string)GetValue(EventNameProperty); }
			set { SetValue(EventNameProperty, value); }
		}

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public object CommandParameter
		{
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public IValueConverter EventArgsConverter
		{
			get { return (IValueConverter)GetValue(EventArgsConverterProperty); }
			set { SetValue(EventArgsConverterProperty, value); }
		}

		public object EventArgsConverterParameter
		{
			get { return GetValue(EventArgsConverterParameterProperty); }
			set { SetValue(EventArgsConverterParameterProperty, value); }
		}

		protected override void OnAttachedTo(BindableObject bindable)
		{
			base.OnAttachedTo(bindable);

			_eventInfo = bindable
				.GetType()
				.GetRuntimeEvent(EventName);
			if (_eventInfo == null)
			{
				throw new ArgumentException(
					$"No matching event '{EventName}' on attached type '{bindable.GetType().Name}'");
			}

			AddEventHandler(_eventInfo, bindable, OnEventRaised);
		}

		protected override void OnDetachingFrom(BindableObject bindable)
		{
			if (_handler != null)
			{
				_eventInfo.RemoveEventHandler(bindable, _handler);
			}
			_handler = null;
			_eventInfo = null;
			base.OnDetachingFrom(bindable);
		}

		void AddEventHandler(EventInfo eventInfo, object item, Action<object, EventArgs> action)
		{
			var eventParameters = eventInfo.EventHandlerType
				.GetRuntimeMethods().First(m => m.Name == "Invoke")
				.GetParameters()
				.Select(p => Expression.Parameter(p.ParameterType))
				.ToArray();

			var actionInvoke = action.GetType()
				.GetRuntimeMethods().First(m => m.Name == "Invoke");

			_handler = Expression.Lambda(
				eventInfo.EventHandlerType,
				Expression.Call(Expression.Constant(action), actionInvoke, eventParameters[0], eventParameters[1]),
				eventParameters)
				.Compile();

			eventInfo.AddEventHandler(item, _handler);
		}

		protected virtual void OnEventRaised(object sender, EventArgs eventArgs)
		{
			if (Command == null)
			{
				return;
			}

			var parameter = CommandParameter;

			if (parameter == null && !string.IsNullOrEmpty(EventArgsParameterPath))
			{
				//Walk the ParameterPath for nested properties.
				var propertyPathParts = EventArgsParameterPath.Split('.');
				object propertyValue = eventArgs;
				foreach (var propertyPathPart in propertyPathParts)
				{
					var propInfo = propertyValue.GetType().GetRuntimeProperty(propertyPathPart);
					if (propInfo == null)
						throw new MissingMemberException($"Unable to find {EventArgsParameterPath}");

					propertyValue = propInfo.GetValue(propertyValue);
					if (propertyValue == null)
					{
						break;
					}
				}
				parameter = propertyValue;
			}

			if(EventArgsConverter != null && (parameter != null || eventArgs != EventArgs.Empty))
			{
				parameter = EventArgsConverter.Convert(eventArgs, typeof(object), EventArgsConverterParameter,
					CultureInfo.CurrentUICulture);
			}

			if (Command.CanExecute(parameter))
			{
				Command.Execute(parameter);
			}
		}
	}
}
