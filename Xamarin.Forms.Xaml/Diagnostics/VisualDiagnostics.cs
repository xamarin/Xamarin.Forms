﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.Xaml.Diagnostics
{
	class VisualDiagnostics
	{
		static ConditionalWeakTable<object, XamlSourceInfo> sourceInfos = new ConditionalWeakTable<object, XamlSourceInfo>();
		internal static void RegisterSourceInfo(object target, Uri uri, int lineNumber, int linePosition)
		{
			if (DebuggerHelper.DebuggerIsAttached && !sourceInfos.TryGetValue(target, out _))
				sourceInfos.Add(target, new XamlSourceInfo(uri, lineNumber, linePosition));
		}

		internal static void SendVisualTreeChanged(object parent, object child)
		{
			if (DebuggerHelper.DebuggerIsAttached)
				VisualTreeChanged?.Invoke(parent, new VisualTreeChangeEventArgs(parent, child, -1, VisualTreeChangeType.Add));
		}

		public static event EventHandler<VisualTreeChangeEventArgs> VisualTreeChanged;
		public static XamlSourceInfo GetXamlSourceInfo(object obj) => sourceInfos.TryGetValue(obj, out var sourceinfo) ? sourceinfo : null;
	}

	class XamlSourceInfo
	{
		public XamlSourceInfo(Uri sourceUri, int lineNumber, int linePosition)
		{
			SourceUri = sourceUri;
			LineNumber = lineNumber;
			LinePosition = linePosition;
		}

		public Uri SourceUri { get; }
		public int LineNumber { get; }
		public int LinePosition { get; }

		public void Deconstruct(out Uri sourceUri, out int lineNumber, out int linePosition)
		{
			sourceUri = SourceUri;
			lineNumber = LineNumber;
			linePosition = LinePosition;
		}
	}

	class VisualTreeChangeEventArgs : EventArgs
	{
		public VisualTreeChangeEventArgs(object parent, object child, int childIndex, VisualTreeChangeType changeType)
		{
			Parent = parent;
			Child = child;
			ChildIndex = childIndex;
			ChangeType = changeType;
		}

		public object Parent { get; }
		public object Child { get; }
		public int ChildIndex { get; }
		public VisualTreeChangeType ChangeType { get; }
	}

	enum VisualTreeChangeType
	{
		Add = 0,
		Remove = 1
	}
}
