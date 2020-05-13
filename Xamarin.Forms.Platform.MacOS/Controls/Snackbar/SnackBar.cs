using System;
using System.Threading.Tasks;
using Foundation;
using Xamarin.Forms.Platform.macOS.Controls.Snackbar.SnackbarViews;

namespace Xamarin.Forms.Platform.macOS.Controls.Snackbar
{
	class SnackBar
	{
		NSTimer _timer;

		public Func<Task> Action { get; protected set; }

		public Func<Task> TimeoutAction { get; protected set; }

		public string ActionButtonText { get; protected set; }

		public SnackbarAppearance Appearance { get; } = new SnackbarAppearance();

		public double Duration { get; protected set; }

		public SnackbarLayout Layout { get; } = new SnackbarLayout();

		public string Message { get; protected set; }

		protected BaseSnackbarView SnackbarView { get; set; }

		public void Dismiss()
		{
			if (_timer != null)
			{
				_timer.Invalidate();
				_timer.Dispose();
				_timer = null;
			}

			SnackbarView?.Dismiss();
		}

		public static SnackBar MakeSnackbar(string message)
		{
			var snackbar = new SnackBar { Message = message };
			return snackbar;
		}

		public SnackBar SetAction(Func<Task> action)
		{
			Action = action;
			return this;
		}
		
		public SnackBar SetTimeoutAction(Func<Task> action)
		{
			TimeoutAction = action;
			return this;
		}

		public SnackBar SetActionButtonText(string title)
		{
			ActionButtonText = title;
			return this;
		}

		public SnackBar SetDuration(double duration)
		{
			Duration = duration;
			return this;
		}

		public SnackBar Show()
		{
			SnackbarView = GetSnackbarView();

			SnackbarView.ParentView.AddSubview(SnackbarView);

			SnackbarView.Setup();

			_timer = NSTimer.CreateScheduledTimer(TimeSpan.FromMilliseconds(Duration), async t =>
			{
				await TimeoutAction();
				Dismiss();
			});

			return this;
		}

		BaseSnackbarView GetSnackbarView()
		{
			if (Action != null && !string.IsNullOrEmpty(ActionButtonText))
			{
				return new ActionMessageSnackbarView(this);
			}

			return new MessageSnackbarView(this);
		}
	}
}