using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Xamarin.Forms.Internals
{
	///  <summary>For internal use by the Xamarin.Forms platform.</summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class ActionSheetArguments
	{
		/// <param name="title">For internal use by the Xamarin.Forms platform.</param>
		/// <param name="cancel">For internal use by the Xamarin.Forms platform.</param>
		/// <param name="destruction">For internal use by the Xamarin.Forms platform.</param>
		/// <param name="buttons">For internal use by the Xamarin.Forms platform.</param>
		/// <summary>For internal use by the Xamarin.Forms platform.</summary>
		public ActionSheetArguments(string title, string cancel, string destruction, IEnumerable<string> buttons)
		{
			Title = title;
			Cancel = cancel;
			Destruction = destruction;
			Buttons = buttons?.Where(c => c != null);
			Result = new TaskCompletionSource<string>();
		}


		//     Gets titles of any buttons on the action sheet that aren't <see cref="Cancel" /> or <see cref="Destruction" />. Can
		//     be <c>null</c>.
		/// <summary>For internal use by the Xamarin.Forms platform.</summary>
		/// <value>For internal use by the Xamarin.Forms platform.</value>
		public IEnumerable<string> Buttons { get; private set; }

		//     Gets the text for a cancel button. Can be null.
		/// <summary>For internal use by the Xamarin.Forms platform.</summary>
		/// <value>For internal use by the Xamarin.Forms platform.</value>
		public string Cancel { get; private set; }

		//     Gets the text for a destructive button. Can be null.
		/// <summary>For internal use by the Xamarin.Forms platform.</summary>
		/// <value>For internal use by the Xamarin.Forms platform.</value>
		public string Destruction { get; private set; }

		/// <summary>For internal use by the Xamarin.Forms platform.</summary>
		/// <value>For internal use by the Xamarin.Forms platform.</value>
		public TaskCompletionSource<string> Result { get; }

		//     Gets the title for the action sheet. Can be null.
		/// <summary>For internal use by the Xamarin.Forms platform.</summary>
		/// <value>For internal use by the Xamarin.Forms platform.</value>
		public string Title { get; private set; }

		/// <param name="result">For internal use by the Xamarin.Forms platform.</param>
		/// <summary>For internal use by the Xamarin.Forms platform.</summary>
		public void SetResult(string result)
		{
			Result.TrySetResult(result);
		}
	}
}