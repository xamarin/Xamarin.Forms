﻿using System;

namespace Xamarin.Platform
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	public class PortHandlerAttribute : Attribute
	{
		public PortHandlerAttribute()
		{

		}

		public PortHandlerAttribute(string description)
		{
			Description = description;
		}

		public string? Description { get; set; }
	}
}