using Xamarin.Forms.Internals;
using MaterialComponents;

namespace Xamarin.Forms.Material.iOS
{
	public class MaterialSnackbar
	{
		public void ShowSnackbar(SnackbarArguments arguments)
		{
			var message = new SnackbarMessage
			{
				Text = arguments.Message,
				Duration = arguments.Duration
			};
			if (arguments.Action != null && !string.IsNullOrEmpty(arguments.ActionButtonText))
			{
				var action = new SnackbarMessageAction();
				action.Title = arguments.ActionButtonText;
				action.Handler = new SnackbarMessageActionHandler(async () =>
				{
					await arguments.Action();
					arguments.SetResult(true);
				});
				message.Action = action;
			}

			SnackbarManager.DefaultManager.ShowMessage(message);
		}
	}
}