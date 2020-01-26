using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class SnackbarArguments
	{
		public SnackbarArguments(string message, int duration, string actionButtonText, Func<Task> action)
		{
			Duration = duration;
			Message = message;
			ActionButtonText = actionButtonText;
			Action = action;
			Result = new TaskCompletionSource<bool>(false);
		}

		/// <summary>
		///     Gets the text for the action button. Can be null.
		/// </summary>
		public string ActionButtonText { get; }

		/// <summary>
		///     Gets the message for the snackbar. Can be null.
		/// </summary>
		public string Message { get; }

		/// <summary>
		///     Result is true if ActionButton is clicked.
		/// </summary>
		public TaskCompletionSource<bool> Result { get; }

		/// <summary>
		///     Gets the duration for the snackbar.
		/// </summary>
		public int Duration { get; }
				
		/// <summary>
		///     Gets the action for the snackbar.
		/// </summary>
		public Func<Task> Action { get; }

		public void SetResult(bool result)
		{
			Result.TrySetResult(result);
		}
	}
}