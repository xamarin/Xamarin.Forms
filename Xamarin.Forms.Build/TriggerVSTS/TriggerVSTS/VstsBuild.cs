//
// VstsBuild.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2017 Microsoft. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xamarin.Provisioning
{
	public static class VstsBuild
	{
		public static async Task<JToken> QueueBuildAsync (
			string host,
			string pat,
			int definitionId,
			string project,
			string collection = "DefaultCollection",
			string sourceBranch = "refs/heads/master",
			(string name, string value) [] parameters = null,
			CancellationToken cancellationToken = default (CancellationToken))
		{
			var httpClient = new HttpClient {
				BaseAddress = new UriBuilder (host) {
					Scheme = "https"
				}.Uri
			};

			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue (
				"Basic",
				Convert.ToBase64String (Encoding.ASCII.GetBytes ($":{pat}"))
			);

			httpClient.DefaultRequestHeaders.Accept.Clear ();
			httpClient.DefaultRequestHeaders.Accept.Add (
				new MediaTypeWithQualityHeaderValue ("application/json"));

			var writer = new StringWriter ();
			new JsonSerializer {
				NullValueHandling = NullValueHandling.Ignore
			}.Serialize (
				writer,
				new {
					definition = new {
						id = definitionId
					},
					sourceBranch = sourceBranch,
					parameters = JsonConvert.SerializeObject (parameters?.ToDictionary (p => p.name, p => p.value))
				}
			);

			var httpContent = new StringContent (writer.ToString ());
			httpContent.Headers.ContentType = new MediaTypeHeaderValue ("application/json");

			var response = await httpClient.PostAsync (
				$"/{collection}/{project}/_apis/build/builds?api-version=2.0",
				httpContent,
				cancellationToken);

			object content = await response.Content.ReadAsStringAsync ();

			try {
				content = JToken.Parse ((string)content);
			} catch {
			}

			return JToken.FromObject (new {
				StatusCode = (int)response.StatusCode,
				ReasonPhrase = response.ReasonPhrase,
				Headers = response.Headers.ToDictionary (a => a.Key, a => a.Value),
				Content = content
			});
		}
	}
}
