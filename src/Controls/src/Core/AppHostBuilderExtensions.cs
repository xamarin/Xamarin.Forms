using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Controls.Internals;

namespace Microsoft.Maui.Hosting
{
	public static class AppHostBuilderExtensions
	{
		public static IAppHostBuilder RegisterFont(this IAppHostBuilder builder, string filename, string alias = null)
		{
			if (!FontRegistrar.RegisterNative(filename, alias))
				throw new System.IO.FileNotFoundException("Failed to load requested font.", filename);

			return builder;
		}
	}
}
