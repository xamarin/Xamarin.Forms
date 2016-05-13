using System;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public class MenuItem : BaseMenuItem
	{
		public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(MenuItem), null);

		public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(MenuItem), null,
			propertyChanging: (bo, o, n) => ((MenuItem)bo).OnCommandChanging(), propertyChanged: (bo, o, n) => ((MenuItem)bo).OnCommandChanged());

		public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(MenuItem), null,
			propertyChanged: (bo, o, n) => ((MenuItem)bo).OnCommandParameterChanged());

		public static readonly BindableProperty IsDestructiveProperty = BindableProperty.Create("IsDestructive", typeof(bool), typeof(MenuItem), false);

		public static readonly BindableProperty IconProperty = BindableProperty.Create("Icon", typeof(FileImageSource), typeof(MenuItem), default(FileImageSource));

        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create("BackgroundColor", typeof(FileImageSource), typeof(MenuItem), Color.Default);
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(FileImageSource), typeof(MenuItem), Color.Default);

        internal static readonly BindableProperty IsEnabledProperty = BindableProperty.Create("IsEnabled", typeof(bool), typeof(ToolbarItem), true);

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

		public FileImageSource Icon
		{
			get { return (FileImageSource)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		public bool IsDestructive
		{
			get { return (bool)GetValue(IsDestructiveProperty); }
			set { SetValue(IsDestructiveProperty, value); }
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

        /// <summary>
        /// Setting the background color of the menu item if target platform is iOS.
        /// </summary>
        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        /// <summary>
        /// Setting the text olor of the menu item if target platform is iOS.
        /// </summary>
        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        internal bool IsEnabled
		{
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}

		bool IsEnabledCore
		{
			set { SetValueCore(IsEnabledProperty, value); }
		}

		public event EventHandler Clicked;

		protected virtual void OnClicked()
		{
			EventHandler handler = Clicked;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		internal void Activate()
		{
			if (Command != null)
			{
				if (IsEnabled)
					Command.Execute(CommandParameter);
			}

			OnClicked();
		}

		void OnCommandCanExecuteChanged(object sender, EventArgs eventArgs)
		{
			IsEnabledCore = Command.CanExecute(CommandParameter);
		}

		void OnCommandChanged()
		{
			if (Command == null)
			{
				IsEnabledCore = true;
				return;
			}

			IsEnabledCore = Command.CanExecute(CommandParameter);

			Command.CanExecuteChanged += OnCommandCanExecuteChanged;
		}

		void OnCommandChanging()
		{
			if (Command == null)
				return;

			Command.CanExecuteChanged -= OnCommandCanExecuteChanged;
		}

		void OnCommandParameterChanged()
		{
			if (Command == null)
				return;

			IsEnabledCore = Command.CanExecute(CommandParameter);
		}
	}
}