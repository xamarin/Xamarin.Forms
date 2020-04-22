namespace Xamarin.Forms.Platform.Android.UnitTests
{
	public class _5560Model : System.ComponentModel.INotifyPropertyChanged
	{
		string _text;

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}

		public string Text
		{
			get => _text;
			set
			{
				_text = value;

				if(!TestCompleted)
					OnPropertyChanged(nameof(Text));
			}
		}

		public void MarkTestCompleted()
		{
			// because this model is reused by multiple controls it can sometimes cause a ping pong effect
			// where multiple controls are updating the model and then the model is re-updating those controls
			// which then re-update the model
			TestCompleted = true;
		}

		bool TestCompleted { get; set; }
	}
}
