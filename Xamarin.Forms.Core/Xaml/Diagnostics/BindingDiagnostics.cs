// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Xaml.Diagnostics
{
	public class BindingDiagnostics
	{
		public static event EventHandler<BindingBaseErrorEventArgs> BindingFailed;

		internal static void SendBindingFailure(BindingBase binding, string errorCode, string message, params object[] messageArgs)
		{
			var sourceinfo = VisualDiagnostics.GetXamlSourceInfo(binding);
			if (sourceinfo != null)
			{
				var sourceinfoline = $"\nBinding source: line={sourceinfo.LineNumber}, column={sourceinfo.LinePosition}, file=\"{sourceinfo.SourceUri}\"";
				Log.Warning(errorCode, message + sourceinfoline, messageArgs);
			}
			else
				Log.Warning(errorCode, message, messageArgs);
			BindingFailed?.Invoke(null, new BindingBaseErrorEventArgs(sourceinfo, binding, errorCode, message, messageArgs));
		}

		internal static void SendBindingFailure(BindingBase binding, object source, BindableObject bo, BindableProperty bp, string errorCode, string message, params object[] messageArgs)
		{
			var sourceinfo = VisualDiagnostics.GetXamlSourceInfo(binding);
			if (sourceinfo != null)
			{
				var sourceinfoline = $"\nBinding source: line={sourceinfo.LineNumber}, column={sourceinfo.LinePosition}, file=\"{sourceinfo.SourceUri}\"";
				Log.Warning(errorCode, message+sourceinfoline, messageArgs);
			}
			else
				Log.Warning(errorCode, message, messageArgs);
			BindingFailed?.Invoke(null, new BindingErrorEventArgs(sourceinfo, binding, source, bo, bp, errorCode, message, messageArgs));
		}
	}
}
