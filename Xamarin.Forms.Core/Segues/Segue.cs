using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	/// <summary>
	/// A navigational transition from one <see cref="Page"/> to another.
	/// </summary>
	public class Segue : BindableObject, ISegueExecution
	{
		public static readonly BindableProperty ActionProperty =
			BindableProperty.Create(nameof(Action), typeof(NavigationAction), typeof(Segue), default(NavigationAction));

		public static readonly BindableProperty IsAnimatedProperty =
			BindableProperty.Create(nameof(IsAnimated), typeof(bool), typeof(Segue), defaultValue: true);

		public static readonly BindableProperty IsEnabledProperty =
			BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(Segue), defaultValue: true, 
				propertyChanged: (s, _, __) => ((Segue)s).OnCanExecuteChanged());

		public static readonly BindableProperty TargetProperty =
			BindableProperty.CreateAttached("Target", typeof(SegueTarget), typeof(Segue), defaultValue: null,
				propertyChanged: (obj, oldVal, newVal) => {
					if ((oldVal == null || newVal == null) && obj is ICommandableElement e && e.Command is Segue.Command c)
						c.OnCanExecuteChanged();
				});

		/// <summary>
		/// Gets or sets the navigation action this <see cref="Segue"/> will perform when executed.
		/// </summary>
		public NavigationAction Action {
			get => (NavigationAction)GetValue(ActionProperty);
			set => SetValue(ActionProperty, value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Segue"/> is animated when executed.
		/// </summary>
		/// <value><c>true</c> if animated; otherwise, <c>false</c>.</value>
		public bool IsAnimated {
			get => (bool)GetValue(IsAnimatedProperty);
			set => SetValue(IsAnimatedProperty, value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Segue"/> is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public bool IsEnabled {
			get => (bool)GetValue(IsEnabledProperty);
			set => SetValue(IsEnabledProperty, value);
		}

		public event EventHandler<SegueBeforeExecuteEventArgs> BeforeExecute;

		/// <summary>
		/// Called before execution of this <see cref="Segue"/> to raise the <see cref="BeforeExecute"/> event.
		/// </summary>
		/// <remarks>
		/// Execution of the segue is deferred until the returned <see cref="Task"/> completes with the result <c>true</c>.
		///  If the result is <c>false</c>, execution of the segue is cancelled. You can use this method to display
		///  an alert to the user to cancel the navigation, or to perform any asynchronous loading or clean up before
		///  navigating to the next page.
		/// </remarks>
		/// <returns><c>true</c> to execute the segue, or <c>false</c> to cancel it.</returns>
		/// <param name="target">The destination <see cref="SegueTarget"/> for this segue.</param>
		protected virtual Task<bool> OnBeforeExecute(SegueTarget target)
		{
			var beforeExec = BeforeExecute;
			if (beforeExec != null) {
				var args = new SegueBeforeExecuteEventArgs(target);
				beforeExec(this, args);
				return Task.FromResult(!args.Cancelled);
			}
			return Task.FromResult(true);
		}
		Task<bool> ISegueExecution.OnBeforeExecute(SegueTarget target) => OnBeforeExecute(target);

		public static SegueTarget GetTarget(BindableObject obj)
		{
			return (SegueTarget)obj.GetValue(TargetProperty);
		}

		public static void SetTarget(BindableObject obj, SegueTarget value)
		{
			obj.SetValue(TargetProperty, value);
		}

		#region ICommand adapters

		event EventHandler CanExecuteChanged;

		/// <summary>
		/// Raises the <see cref="CanExecuteChanged"/> event.
		/// </summary>
		protected virtual void OnCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <remarks>
		/// If you override this method, be sure to call <see cref="OnCanExecuteChanged"/> as appropriate.
		/// </remarks>
		/// <param name="source">The source object that triggered this segue.</param>
		/// <param name="parameter">Parameter passed to <see cref="ICommand.CanExecute(object)"/>.</param>
		protected internal virtual bool CanExecuteCommand(Element source, object parameter)
		{
			return DefaultCanExecuteCommand(this, source);
		}

		internal static bool DefaultCanExecuteCommand(ValueSegue seg, Element source)
		{
			if (!seg.IsEnabled)
				return false;

			if (GetTarget(source) == null)
				return !seg.Action.RequiresTarget();

			return true;
		}

		/// <summary>
		/// Called when an <see cref="ICommand"/> that was vended by <see cref="ToCommand"/> is executed.
		/// </summary>
		/// <param name="source">The source object that triggered this segue.</param>
		/// <param name="parameter">Parameter passed to <see cref="ICommand.Execute(object)"/>.</param>
		protected internal virtual Task ExecuteCommand(Element source, object parameter)
		{
			return DefaultExecuteCommand(this, source);
		}

		internal static Task DefaultExecuteCommand(ValueSegue seg, Element source)
		{
			if (!seg.IsEnabled)
				throw new InvalidOperationException("IsEnabled is false");

			var nav = FindVisualElement(source)?.Navigation;
			if (nav == null)
				throw new NotSupportedException("Source must be in the view hierarchy");

 			return nav.SegueAsync(seg, GetTarget(source));
		}

		static VisualElement FindVisualElement(Element elem)
		{
			while (elem != null) {
				if (elem is VisualElement visElem)
					return visElem;
				elem = elem.Parent;
			}
			return null;
		}

		sealed class Command : ICommand
		{
			ValueSegue seg;
			Element source;
			EventHandler canExecuteChanged;

			public event EventHandler CanExecuteChanged {
				add {
					if (canExecuteChanged == null && seg.Segue != null)
						seg.Segue.CanExecuteChanged += Segue_CanExecuteChanged;
					canExecuteChanged += value;
				}
				remove {
					canExecuteChanged -= value;
					if (canExecuteChanged == null && seg.Segue != null)
						seg.Segue.CanExecuteChanged -= Segue_CanExecuteChanged;
				}
			}

			public Command(ValueSegue seg, Element source)
			{
				this.seg = seg;
				this.source = source;
			}

			public void OnCanExecuteChanged() => canExecuteChanged?.Invoke(this, EventArgs.Empty);
			void Segue_CanExecuteChanged(object sender, EventArgs e) => OnCanExecuteChanged();

			public bool CanExecute(object parameter) => seg.CanExecuteCommand(source, parameter);
			public void Execute(object parameter) => seg.ExecuteCommand(source, parameter);
		}

		/// <summary>
		/// Creates a new <see cref="ICommand"/> instance for triggering this segue
		///  from the given source element.
		/// </summary>
		public ICommand ToCommand(Element source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			return new Command(this, source);
		}

		/// <summary>
		/// Creates an <see cref="ICommand"/> to trigger the default <see cref="Segue"/>
		///  from the given source <see cref="Element"/> with the given <see cref="NavigationAction"/>.
		/// </summary>
		/// <remarks>
		/// Optimized equivalent to this:
		/// <code>
		///   new Segue { Action = action }.ToCommand(source)
		/// </code>
		/// </remarks>
		public static ICommand CreateCommand(Element source, NavigationAction action, bool animated = true)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			return new Command(new ValueSegue(action, animated), source);
		}

		#endregion
	}
}
