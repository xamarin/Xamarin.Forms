// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Xamarin.Forms.Xaml.Diagnostics
{
	public class BindingErrorEventArgs : EventArgs
	{
		internal BindingErrorEventArgs(XamlSourceInfo xamlSourceInfo, BindingBase binding, string errorCode, string message, object[] messageArgs)
		{
			XamlSourceInfo = xamlSourceInfo;
			Binding = binding;
			ErrorCode = errorCode;
			Message = message;
			MessageArgs = messageArgs;
		}

		public XamlSourceInfo XamlSourceInfo { get; }
		public BindingBase Binding { get; }
		public string ErrorCode { get; }
		public string Message { get; }
		public object[] MessageArgs { get; }
	}
}
