using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace Xamarin.Forms.Platform.WPF
{
	internal class Deserializer : IDeserializer
	{
		const string PropertyStoreFile = "PropertyStore.forms";

		public Task<IDictionary<string, object>> DeserializePropertiesAsync()
		{
			// Deserialize property dictionary to local storage
			// Make sure to use Internal
			return Task.Run(() =>
			{
				using (var stream = File.Open(PropertyStoreFile, System.IO.FileMode.OpenOrCreate))
				using (XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
				{
					if (stream.Length == 0)
						return null;

					try
					{
						var dcs = new DataContractSerializer(typeof(Dictionary<string, object>));
						return (IDictionary<string, object>)dcs.ReadObject(reader);
					}
					catch (Exception e)
					{
						Debug.WriteLine("Could not deserialize properties: " + e.Message);
					}
				}

				return null;
			});
		}

		public Task SerializePropertiesAsync(IDictionary<string, object> properties)
		{
			properties = new Dictionary<string, object>(properties);
			// Serialize property dictionary to local storage
			// Make sure to use Internal
			return Task.Run(() =>
			{
				var success = false;
				
				using (var stream = File.Open(PropertyStoreFile + ".tmp", System.IO.FileMode.OpenOrCreate))
				using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
				{
					try
					{
						var dcs = new DataContractSerializer(typeof(Dictionary<string, object>));
						dcs.WriteObject(writer, properties);
						writer.Flush();
						success = true;
					}
					catch (Exception e)
					{
						Debug.WriteLine("Could not serialize properties: " + e.Message);
					}
				}

				if (!success)
					return;
				
				{
					try
					{
						if (File.Exists(PropertyStoreFile))
							File.Delete(PropertyStoreFile);
						File.Move(PropertyStoreFile + ".tmp", PropertyStoreFile);
					}
					catch (Exception e)
					{
						Debug.WriteLine("Could not move new serialized property file over old: " + e.Message);
					}
				}
			});
		}
	}
}