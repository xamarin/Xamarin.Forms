using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms
{
	public class ShellParameter : Dictionary<string, object>
	{
		public ShellParameter()
		{
		}

		public ShellParameter(int count)
			: base(count)
		{
		}

		public void Merge(Dictionary<string, string> input)
		{
			if (input == null || input.Count == 0)
				return;

			foreach (var item in input)
				Add(item.Key, item.Value);
		}
	}


	public static class ShellParameterExtensions
	{
		public static void Deconstruct(this KeyValuePair<string, object> tuple, out string key, out object value)
		{
			key = tuple.Key;
			value = tuple.Value;
		}


		public static Dictionary<string, string> GetStringValues(this ShellParameter parameter)
		{
			var dic = new Dictionary<string, string>();

			foreach (var (key, value) in parameter)
			{
				if (value is string v)
					dic.Add(key, v);
			}

			return dic;
		}
	}
}
