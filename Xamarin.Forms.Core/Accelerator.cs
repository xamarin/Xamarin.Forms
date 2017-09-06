using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xamarin.Forms
{
	[TypeConverter(typeof(AcceleratorTypeConverter))]
	public class Accelerator
	{
		const char Separator = '+';
		string _text;

		internal Accelerator(string text)
		{
			if (string.IsNullOrEmpty(text))
				throw new ArgumentNullException(nameof(text));
			_text = text;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerable<string> Modififiers { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerable<string> Keys { get; set; }

		public static Accelerator FromString(string text)
		{
			var accelarat = new Accelerator(text);

			var acceleratorParts = text.Split(Separator);

			if (acceleratorParts.Length > 1)
			{
				var modifiers = new List<string>();
				for (int i = 0; i < acceleratorParts.Length; i++)
				{
					var modifierMask = acceleratorParts[i];
					var modiferMaskLower = modifierMask.ToLower();
					switch (modiferMaskLower)
					{
						case "ctrl":
						case "cmd":
						case "alt":
						case "shift":
						case "fn":
							modifiers.Add(modiferMaskLower);
							text = text.Replace(modifierMask, "");
							break;
					}
				}
				accelarat.Modififiers = modifiers;

			}

			var keys = text.Split(new char[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
			accelarat.Keys = keys;
			return accelarat;
		}

		public override string ToString()
		{
			return _text;
		}
	}
}
