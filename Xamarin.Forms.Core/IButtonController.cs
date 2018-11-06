using System;
using System.Windows.Input;

namespace Xamarin.Forms
{
	public interface IButtonController : IViewController
	{
		void SendClicked();
		void SendPressed();
		void SendReleased();
		void PropagateUpClicked();
		void PropagateUpPressed();
		void PropagateUpReleased();
		void SetIsPressed(bool isPressed);
		void OnCommandCanExecuteChanged(object sender, EventArgs e);
	}
}