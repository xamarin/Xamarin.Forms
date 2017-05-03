using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.WPF.Helpers;

namespace Xamarin.Forms.Platform.WPF
{
	internal sealed class WindowsSerializer : IDeserializer
	{
		const string PropertyStoreFile = "PropertyStore.forms";

		public async Task<IDictionary<string, object>> DeserializePropertiesAsync()
		{
			try
			{
				using (var stream = ApplicationData.Instance.GetFileAsync(PropertyStoreFile))
				{
					if (stream.Length == 0)
						return new Dictionary<string, object>(4);

					var serializer = new DataContractSerializer(typeof(IDictionary<string, object>));
					return (IDictionary<string, object>)serializer.ReadObject(stream);
				}
			}
			catch (FileNotFoundException)
			{
				return new Dictionary<string, object>(4);
			}
		}

		public async Task SerializePropertiesAsync(IDictionary<string, object> properties)
		{
			try
			{
			    using (Stream stream = ApplicationData.Instance.CreateFileAsync(PropertyStoreFile))
			    {
			        var serializer = new DataContractSerializer(typeof(IDictionary<string, object>));
			        serializer.WriteObject(stream, properties);
					stream.Flush();
			    }
			}
			catch (Exception e)
			{
				Debug.WriteLine("Could not move new serialized property file over old: " + e.Message);
			}
		}
	}
}