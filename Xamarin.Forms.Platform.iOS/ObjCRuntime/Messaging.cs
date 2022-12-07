using System;
using System.Runtime.InteropServices;

namespace ObjCRuntime
{
	static class Messaging
	{
		[DllImport(Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
		public static extern nint nint_objc_msgSend(IntPtr receiver, IntPtr selector);
	}
}