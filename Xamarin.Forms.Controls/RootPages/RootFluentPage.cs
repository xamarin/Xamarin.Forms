using Xamarin.Forms.Fluent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using static System.Math;

namespace Xamarin.Forms.Controls.RootPages
{
	public class RootFluentPage : ContentPage
	{
		public RootFluentPage()
			=> Content = new StackLayout
			{
				Padding = new Thickness(25, 50),
				Children =
					{
						new Label()
							.Bind(Label.TextProperty, nameof(RootFluentViewModel.LabelText))
							.Bind(BackgroundColorProperty, nameof(RootFluentViewModel.LabelBackgroundColor),
								converter: new GenericValueConverter(v => Color.FromHex(v.ToString())))
							.Bind(HeightRequestProperty, nameof(RootFluentViewModel.LabelHeight)),

						new Button
						{
							Text = "Click Me (1)",
							CommandParameter = "(1) Clicked"
						}.Command(nameof(RootFluentViewModel.ClickCommand)),

						new Button
						{
							Text = "Click Me (2)",
							CommandParameter = "(2) Clicked"
						}.Command(nameof(RootFluentViewModel.ClickCommand)),

						new ContentView
						{
							BackgroundColor = Color.Gold,
							Content = new Label
							{
								FontSize = 20,
								Text = "Click Me (3)"
							}
						}.Tap(nameof(RootFluentViewModel.ClickCommand), "(3) Clicked")
					}
			}.Fluent(p => BindingContext = new RootFluentViewModel());
	}

	public class RootFluentViewModel : BaseViewModel
	{
		public string LabelText
		{
			get => Get("Initial value");
			set => Set(value);
		}

		public string LabelBackgroundColor => "#ff0000";

		public double LabelHeight => 50;

		public ICommand ClickCommand => Cmd() ?? RegCmd(p =>
		{
			LabelText = p.ToString();
		});
	}

	#region Base classes

	public class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly Lazy<ConcurrentDictionary<Type, object>> _lazyPropertiesMapping = new Lazy<ConcurrentDictionary<Type, object>>(() => new ConcurrentDictionary<Type, object>());

		#region Properties

		protected T Get<T>(T defaultValue = default(T), [CallerMemberName] string key = null)
			=> GetTypeDict<T>().TryGetValue(key, out T val)
				? val
				: defaultValue;

		protected bool Set<T>(T value, bool shouldEqual = true, bool shouldRaisePropertyChanged = true, [CallerMemberName] string key = null)
		{
			var typeDict = GetTypeDict<T>();
			if (shouldEqual && typeDict.TryGetValue(key, out T oldValue) && EqualityComparer<T>.Default.Equals(oldValue, value))
			{
				return false;
			}
			typeDict[key] = value;
			if (shouldRaisePropertyChanged)
			{
				OnPropertyChanged(key);
			}
			return true;
		}

		protected void OnPropertyChanged([CallerMemberName] string key = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));

		#endregion

		#region Commands

		/// <summary>
		/// Using: public ICommand YourCommand => RetCmd(() => new CustomCommand());
		/// </summary>
		protected ICommand RetCmd(Func<ICommand> commandCreator, [CallerMemberName] string key = null)
			=> Cmd(key) ?? RegCmd(commandCreator, key);

		/// <summary>
		/// Using: public ICommand YourCommand => RetCmd(() => { ..action.. });
		/// </summary>
		protected ICommand RetCmd(Action<object> action, TimeSpan? actionFrequency = null, bool shouldSuppressExceptions = false, Action<Exception> onExceptionAction = null, Func<bool> canExecute = null, [CallerMemberName] string key = null)
			=> Cmd(key) ?? RegCmd(action, actionFrequency, shouldSuppressExceptions, onExceptionAction, canExecute, key);

		/// <summary>
		/// Using: public ICommand YourCommand => RetCmd(() => { ..action.. });
		/// </summary>
		protected ICommand RetCmd(Action action, TimeSpan? actionFrequency = null, bool shouldSuppressExceptions = false, Action<Exception> onExceptionAction = null, Func<bool> canExecute = null, [CallerMemberName]  string key = null)
			=> RetCmd(p => action?.Invoke(), actionFrequency, shouldSuppressExceptions, onExceptionAction, canExecute, key);

		/// <summary>
		/// Using: public ICommand YourCommand => Cmd() ?? RegCmd(() => { ..action.. });
		/// </summary>
		protected ICommand Cmd([CallerMemberName]  string key = null)
			=> GetTypeDict<ICommand>().TryGetValue(key, out ICommand command)
				? command
				: null;

		/// <summary>
		/// Using: public ICommand YourCommand => Cmd() ?? RegCmd(new CustomCommand());
		/// </summary>
		protected ICommand RegCmd(ICommand command, [CallerMemberName]  string key = null)
			=> GetTypeDict<ICommand>()[key] = command;

		/// <summary>
		/// Using: public ICommand YourCommand => Cmd() ?? RegCmd(() => new CustomCommand());
		/// </summary>
		protected ICommand RegCmd(Func<ICommand> commandCreator, [CallerMemberName]  string key = null)
			=> RegCmd(commandCreator?.Invoke());

		/// <summary>
		/// Using: public ICommand YourCommand => Cmd() ?? RegCmd(() => { ..action.. });
		/// </summary>
		protected ICommand RegCmd(Action<object> action, TimeSpan? actionFrequency = null, bool shouldSuppressExceptions = false, Action<Exception> onExceptionAction = null, Func<bool> canExecute = null, [CallerMemberName] string key = null)
			=> RegCmd(new BaseCommand(p => action?.Invoke(p), actionFrequency, shouldSuppressExceptions, onExceptionAction, canExecute));

		/// <summary>
		/// Using: public ICommand YourCommand => Cmd() ?? RegCmd(() => { ..action.. });
		/// </summary>
		protected ICommand RegCmd(Action action, TimeSpan? actionFrequency = null, bool shouldSuppressExceptions = false, Action<Exception> onExceptionAction = null, Func<bool> canExecute = null, [CallerMemberName] string key = null)
			=> RegCmd(p => action?.Invoke(), actionFrequency, shouldSuppressExceptions, onExceptionAction, canExecute, key);

		#endregion

		private ConcurrentDictionary<string, T> GetTypeDict<T>()
		{
			var type = typeof(T);
			if (!_lazyPropertiesMapping.Value.TryGetValue(type, out object valDict))
			{
				_lazyPropertiesMapping.Value[type] = valDict = new ConcurrentDictionary<string, T>();
			}
			return valDict as ConcurrentDictionary<string, T>;
		}
	}

	public class BaseCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Action<object> _action;
		private readonly Action<Exception> _onExceptionAction;
		private readonly TimeSpan _actionFrequency;
		private readonly bool _shouldSuppressExceptions;
		private DateTime _lastActionTime;
		private Func<bool> _canExecute;

		public BaseCommand(Action<object> action, TimeSpan? actionFrequency = null, bool shouldSuppressExceptions = false, Action<Exception> onExceptionAction = null, Func<bool> canExecute = null)
		{
			_action = action;
			_actionFrequency = actionFrequency ?? TimeSpan.Zero;
			_shouldSuppressExceptions = shouldSuppressExceptions;
			_onExceptionAction = onExceptionAction;
			_canExecute = canExecute;
		}

		public BaseCommand(Action action, TimeSpan? actionFrequency = null, bool shouldSuppressExceptions = false, Action<Exception> onExceptionAction = null, Func<bool> canExecute = null) : this(p => action?.Invoke(), actionFrequency, shouldSuppressExceptions, onExceptionAction, canExecute)
		{
		}

		public bool CanExecute(object parameter)
			=> _canExecute?.Invoke() ?? true;

		public void Execute(object parameter)
		{
			var nowTime = DateTime.UtcNow;
			if (_actionFrequency == TimeSpan.Zero
				|| Abs((nowTime - _lastActionTime).TotalMilliseconds) >= _actionFrequency.TotalMilliseconds)
			{
				_lastActionTime = nowTime;
				try
				{
					_action.Invoke(parameter);
				}
				catch (Exception ex)
				{
					_onExceptionAction?.Invoke(ex);
					if (_shouldSuppressExceptions)
					{
						return;
					}
					throw ex;
				}
			}
		}

		public void ChangeCanExecute(bool value)
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	#endregion

}
