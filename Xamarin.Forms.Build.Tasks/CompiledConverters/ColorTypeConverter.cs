using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Core.XamlC
{
	class ColorTypeConverter : ICompiledTypeConverter
	{
		public IEnumerable<Instruction> ConvertFromString(string value, ModuleDefinition module, BaseNode node)
		{
			do {
				if (string.IsNullOrEmpty(value))
					break;

				value = value.Trim();

				if (value.StartsWith("#", StringComparison.Ordinal)) {
					var color = Color.FromHex(value);
					yield return Instruction.Create(OpCodes.Ldc_R8, color.R);
					yield return Instruction.Create(OpCodes.Ldc_R8, color.G);
					yield return Instruction.Create(OpCodes.Ldc_R8, color.B);
					yield return Instruction.Create(OpCodes.Ldc_R8, color.A);
					var colorCtor = module.Import(typeof(Color)).Resolve().Methods.FirstOrDefault(
						md => md.IsConstructor && md.Parameters.Count == 4 &&
						md.Parameters.All(p => p.ParameterType.FullName == "System.Double"));
					var colorCtorRef = module.Import(colorCtor);
					yield return Instruction.Create(OpCodes.Newobj, colorCtorRef);
					yield break;
				}
				var parts = value.Split('.');
				if (parts.Length == 1 || (parts.Length == 2 && parts [0] == "Color")) {
					var color = parts [parts.Length - 1];

					var field = module.Import(typeof(Color)).Resolve().Fields.SingleOrDefault(fd => fd.Name == color && fd.IsStatic);
					if (field != null) {
						yield return Instruction.Create(OpCodes.Ldsfld, module.Import(field));
						yield break;
					}
					var propertyGetter = module.Import(typeof(Color)).Resolve().Properties.SingleOrDefault(pd => pd.Name == color && pd.GetMethod.IsStatic)?.GetMethod;
					if (propertyGetter != null) {
						yield return Instruction.Create(OpCodes.Call, module.Import(propertyGetter));
						yield break;
					}
				}
			} while (false);

			throw new XamlParseException($"Cannot convert \"{value}\" into {typeof(Color)}", node);
		}
	}
}