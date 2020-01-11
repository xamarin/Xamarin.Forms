using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms.Xaml
{
	public class XamlMyExt
	{
		public static Func<string, string, Type> FallbackTypeResolver { get; set; }
		public static Func<Type, object> HookTypeInstantiate { get; set; }
	}
}
