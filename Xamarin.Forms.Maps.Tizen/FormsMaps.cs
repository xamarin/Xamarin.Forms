using System.Diagnostics;
using Tizen.Maps;
using Xamarin.Forms.Maps.Tizen;

namespace Xamarin
{
	public static class FormsMaps
	{
		static MapService _mapService = null;

		static string ProviderName { get; set; }

		static string AuthenticationToken { get; set; }

		internal static bool IsInitialized { get; private set; }

		internal static MapService MapService
		{
			get
			{
				Debug.Assert(_mapService != null, "FormsMaps is not initialized");
				return _mapService;
			}
		}

		public static void Init(string provider, string authenticationToken)
		{
			ProviderName = provider;
			AuthenticationToken = authenticationToken;
			Init();
		}

		internal static async void Init()
		{
			if (IsInitialized)
				return;
			var requestResult = await MapService.RequestUserConsent(ProviderName);
			if (requestResult)
			{
				_mapService = new MapService(ProviderName, AuthenticationToken);
				if (_mapService != null)
				{
					GeocoderBackend.Register();
					IsInitialized = true;
				}
			}
		}
	}
}