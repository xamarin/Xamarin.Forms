using System;
using System.Diagnostics;

namespace Xamarin.Forms
{

	[DebuggerDisplay("Location = {Location}")]
	public class ShellNavigationState
	{
		Uri _fullLocation;

		internal Uri FullLocation
		{
			get => _fullLocation;
			set
			{
				_fullLocation = value;
				Location = Routing.RemoveImplicit(value);
			}
		}

		public Uri Location
		{
			get;
			private set;
        }

        public object Parameter
        {
            get;
            private set;
        }

        public ShellNavigationState() { }
		public ShellNavigationState(string location, object parameter = null)
        {
			var uri = ShellUriHandler.CreateUri(location);

			if (uri.IsAbsoluteUri)
				uri = new Uri($"/{uri.PathAndQuery}", UriKind.Relative);

			FullLocation = uri;
            Parameter = parameter;
        }

        public ShellNavigationState(Uri location, object parameter = null)
        {
            FullLocation = location;
            Parameter = parameter;
        }

		public static implicit operator ShellNavigationState(Uri uri) => new ShellNavigationState(uri);
		public static implicit operator ShellNavigationState(string value) => new ShellNavigationState(value);
	}
}
