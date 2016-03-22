using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Xamarin.Forms.Pages
{
	public class UriJsonSource : JsonSource
	{
		public static readonly BindableProperty UriProperty = BindableProperty.Create(nameof(Uri), typeof(Uri), typeof(UriJsonSource), null);

		public Uri Uri
		{
			get { return (Uri)GetValue(UriProperty); }
			set { SetValue(UriProperty, value); }
		}

		public override async Task<string> GetJson()
		{
			var webClient = new HttpClient();
			string json = await webClient.GetStringAsync(Uri);
			return json;
		}
	}
}