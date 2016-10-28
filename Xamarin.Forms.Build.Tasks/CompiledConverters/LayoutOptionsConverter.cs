using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Core.XamlC
{
	class LayoutOptionsConverter : ICompiledTypeConverter
	{
		public IEnumerable<Instruction> ConvertFromString(string value, ModuleDefinition module, BaseNode node)
		{
			do {
				if (string.IsNullOrEmpty(value))
					break;

				value = value.Trim();

				var parts = value.Split('.');
				if (parts.Length == 1 || (parts.Length == 2 && parts [0] == "LayoutOptions")) {
					var options = parts [parts.Length - 1];

					var field = module.Import(typeof(LayoutOptions)).Resolve().Fields.SingleOrDefault(fd => fd.Name == options && fd.IsStatic);
					if (field != null) {
						yield return Instruction.Create(OpCodes.Ldsfld, module.Import(field));
						yield break;
					}
				}
			} while (false);

			throw new XamlParseException(String.Format("Cannot convert \"{0}\" into {1}", value, typeof(LayoutOptions)), node);
		}
	}
}