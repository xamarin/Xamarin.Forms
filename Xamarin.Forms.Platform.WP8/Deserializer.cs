using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.WinPhone
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
				using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
				using (IsolatedStorageFileStream stream = store.OpenFile(PropertyStoreFile, System.IO.FileMode.OpenOrCreate))
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
				using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
				using (IsolatedStorageFileStream stream = store.OpenFile(PropertyStoreFile + ".tmp", System.IO.FileMode.OpenOrCreate))
				using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
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
				using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					try
					{
						if (store.FileExists(PropertyStoreFile))
							store.DeleteFile(PropertyStoreFile);
						store.MoveFile(PropertyStoreFile + ".tmp", PropertyStoreFile);
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