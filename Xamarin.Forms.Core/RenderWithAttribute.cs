﻿using System;

namespace Xamarin.Forms
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class RenderWithAttribute : Attribute
	{

		public RenderWithAttribute(Type type) : this(type, new[] { typeof(VisualRendererMarker.Default) })
		{
		}

		public RenderWithAttribute(Type type, Type[] supportedVisuals)
		{
			Type = type;
			SupportedVisuals = supportedVisuals ?? new[] { typeof(VisualRendererMarker.Default) };
		}

		public Type[] SupportedVisuals { get; }
		public Type Type { get; }
	}
}