using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Xamarin.Forms.Platform.UAP.Extensions
{
	public static class ContentDialogExtensions
	{
		static TaskCompletionSource<object> PreviousDialogCompletion;

		public static async Task<ContentDialogResult> EnqueueAndShowAsync(this ContentDialog contentDialog, Func<int> numberOfContentDialogs = null)
		{
			TaskCompletionSource<object> currentDialogCompletion = new TaskCompletionSource<object>();

			var previousDialogCompletion = PreviousDialogCompletion;
			PreviousDialogCompletion = currentDialogCompletion;

			if (previousDialogCompletion != null)
				await previousDialogCompletion.Task;
			
			var contentDialogResult = ContentDialogResult.None;

			if (numberOfContentDialogs == null || numberOfContentDialogs() <= 1)
				contentDialogResult = await contentDialog.ShowAsync();
						
			currentDialogCompletion.SetResult(null);

			return contentDialogResult;
		}
	}
}
