using System.ComponentModel;
using System.Threading.Tasks;

namespace Microsoft.Maui.Controls.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class PromptArguments
	{
		public PromptArguments(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = default(Keyboard), string initialValue = "")
		{
			Title = title;
			Message = message;
			Accept = accept;
			Cancel = cancel;
			Placeholder = placeholder;
			InitialValue = initialValue;
			MaxLength = maxLength;
			Keyboard = keyboard ?? Keyboard.Default;
			Result = new TaskCompletionSource<string>();
		}

		public string Title { get; }

		public string Message { get; }

		public string Accept { get; }

		public string Cancel { get; }

		public string Placeholder { get; }

		public string InitialValue { get; }

		public int MaxLength { get; }

		public Keyboard Keyboard { get; }

		public TaskCompletionSource<string> Result { get; }

		public void SetResult(string text)
		{
			Result.TrySetResult(text);
		}
	}
}