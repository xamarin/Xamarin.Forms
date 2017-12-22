using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;

namespace Xamarin.Forms.Xaml
{
	[ContentProperty("Command")]
	public sealed class AutoCommandExtension : IMarkupExtension<ICommand>
	{
		static readonly ICommand EmptyCommand =
			new Command(() => { });

		PropertyChangedEventHandler invalidateCanExecuteHandler;

		WeakReference oldBindingContex;


		[DefaultValue(null)]
		public string Command { get; set; }

		[DefaultValue(null)]
		public object Source { get; set; }

		ICommand IMarkupExtension<ICommand>.ProvideValue(IServiceProvider serviceProvider)
		{
			BindableObject target = null;
			BindableProperty targetProperty = null;
			if (string.IsNullOrWhiteSpace(Command) == false
				&& this.TryGetTargetItems(serviceProvider, out target, out targetProperty)
				&& targetProperty.ReturnType == typeof(ICommand))
			{
				target.BindingContextChanged += (s, e) 
					=> OnDataContextChanged(target, targetProperty);					 
			}
			return EmptyCommand;
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return (this as IMarkupExtension<ICommand>).ProvideValue(serviceProvider);
		}

		private bool TryGetTargetItems(IServiceProvider provider, out BindableObject target, out BindableProperty dp)
		{
			return this.TryGetTargetItems<BindableObject>(provider, out target, out dp);
		}

		private void OnDataContextChanged(BindableObject target, BindableProperty targetProperty)
		{
			var currentContext = target.BindingContext;
			//CleanUp
			if (oldBindingContex != null && oldBindingContex.IsAlive)
			{
				OnUnregisterCommand(target
					, targetProperty
					, oldBindingContex.Target);
			}
			oldBindingContex = new WeakReference(currentContext);
			//Setup
			OnRegisterCommand(target
				, targetProperty
				, target.BindingContext);
		}

		void OnUnregisterCommand(BindableObject target, BindableProperty targetProperty, object oldDataContext)
		{
			if (oldDataContext is INotifyPropertyChanged inpc)
			{
				inpc.PropertyChanged -= invalidateCanExecuteHandler;
				invalidateCanExecuteHandler = null;
			} 
			(target.GetValue(targetProperty) as Command)
				?.ChangeCanExecute();
			target.SetValue(targetProperty, EmptyCommand);
		}

		void OnRegisterCommand(BindableObject target, BindableProperty targetProperty, object newDataContext)
		{
			if (string.IsNullOrWhiteSpace(this.Command) == false)
			{
				//Search dataContext 
				var methodPath = this.Command;
				var nestedPath = methodPath
					.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
				var dataContext = Source ?? newDataContext;
				var pathLevel = 0;

				while (dataContext != null && pathLevel < nestedPath.Length - 1)
				{
					var currentProperty = nestedPath[pathLevel];
					var dataContextType = dataContext.GetType();
					var pi = dataContextType.GetProperty(currentProperty);
					if (pi == null)
					{
						dataContext = null;
						break;
					}
					dataContext = pi.GetValue(dataContext, null);
					pathLevel++;
					methodPath = nestedPath[pathLevel];
				}

				if (dataContext == null)
				{
					//TODO: Throw Exception???
					return;
				}

				// Search Execution Method
				var executeMethod = GetMethod(dataContext
					, methodPath);

				if (executeMethod == null)
				{
					//TODO: Throw Exception???
					return;
				}

				//Search CanExecuteMethod
				var canExecuteMethod = GetMethod(dataContext
					, "Can" + methodPath
					, m => m.ReturnType == typeof(bool));

				Command command = null;
				var onExecute = CreateExecute(dataContext,
					executeMethod);
				if (canExecuteMethod == null)
				{
					command = new Command(onExecute);
				}
				else
				{
					var onCanExecute = CreateCanExecute(dataContext,
						canExecuteMethod);
					command = new Command(onExecute
						, onCanExecute);

					// Check CanExcute Dependecy for invlidated
					if (dataContext is INotifyPropertyChanged inpc)
					{
						var dependencyProperties = canExecuteMethod
							.GetCustomAttributes(typeof(DependedOnAttribute), true)
							.OfType<DependedOnAttribute>()
							.Select(x => x.PropertyName)
							.ToArray();
						invalidateCanExecuteHandler = (s, e) =>
							{
								if (string.IsNullOrWhiteSpace(e.PropertyName)
									|| dependencyProperties.Contains(e.PropertyName))
								{
									command.ChangeCanExecute();
								}
							};
						inpc.PropertyChanged += invalidateCanExecuteHandler;
					}

				}
				target.SetValue(targetProperty, command);
			}
		}

		private bool TryGetTargetItems<T>(IServiceProvider provider, out T target, out BindableProperty dp)
			where T : BindableObject
		{
			target = null;
			dp = null;
			if (provider == null) return false;

			//create a binding and assign it to the target
			var service = provider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (service == null) return false;

			//we need dependency objects / properties
			target = service.TargetObject as T;
			dp = service.TargetProperty as BindableProperty;
			return target != null && dp != null;
		}

		static System.Reflection.MethodInfo GetMethod(object dataContext
			, string methodPath
			, Func<System.Reflection.MethodInfo, bool> filter = null)
		{
			if (filter == null)
			{
				filter = m => m.Name == methodPath
					&& m.GetParameters().Length == 1
					&& m.GetParameters()[0].ParameterType == typeof(object);
			}
			else
			{
				var t = filter;
				filter = m => m.Name == methodPath
					&& m.GetParameters().Length == 1
					&& m.GetParameters()[0].ParameterType == typeof(object)
					&& t(m);
			}
			return dataContext.GetType()
				.GetMethods(System.Reflection.BindingFlags.Public
					| System.Reflection.BindingFlags.NonPublic
					| System.Reflection.BindingFlags.Instance)
				.FirstOrDefault(filter);
		}

		static Action<object> CreateExecute(object target
			, System.Reflection.MethodInfo method)
		{

			var parameter = Expression.Parameter(typeof(object), "parameter");

			var instance = Expression.Convert
			(
				Expression.Constant(target),
				method.DeclaringType
			);

			var call = Expression.Call
			(
				instance,
				method,
				parameter
			);

			return Expression
				.Lambda<Action<object>>(call, parameter)
				.Compile();
		}

		static Func<object, bool> CreateCanExecute(object target
			, System.Reflection.MethodInfo method)
		{
			var parameter = Expression.Parameter(typeof(object), "parameter");
			var instance = Expression.Convert
			(
				Expression.Constant(target),
				method.DeclaringType
			);
			var call = Expression.Call
			(
				instance,
				method,
				parameter
			);
			return Expression
				.Lambda<Func<object, bool>>(call, parameter)
				.Compile();
		}
	}
}
