﻿using System.Collections.Generic;
using Mono.Cecil.Cil;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Build.Tasks;
using static Mono.Cecil.Cil.Instruction;
using static Mono.Cecil.Cil.OpCodes;

namespace Xamarin.Forms.Core.XamlC
{
	class EasingTypeConverter : ICompiledTypeConverter
	{
		public IEnumerable<Instruction> ConvertFromString(string value, ILContext context, BaseNode node)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				yield return Create(Ldnull);
				yield break;
			}

			var assemblyTypeInfo = ("Xamarin.Forms.Core", "Xamarin.Forms", nameof(Easing));
			var easing = value?.Trim();
			var module = context.Body.Method.Module;
			var fieldReference = module.ImportFieldReference(assemblyTypeInfo, easing, isStatic: true, caseSensitive: false);

			if (fieldReference != null)
			{
				yield return Create(Ldsfld, fieldReference);
				yield break;
			}

			throw new BuildException(BuildExceptionCode.Conversion, node, null, value, typeof(Easing));
		}
	}
}