using System;
using System.ComponentModel;
using Foundation;
using ObjCRuntime;

namespace Xamarin.Forms.Platform.iOS
{
	/// <summary>
	/// Custom wrapper for <see cref="NSIndexPath"/>. Required to speed up
	/// <see cref="ItemsViewDelegator.GetSizeForItem(IntPtr, IntPtr, NSIndexPathRef)"/>
	/// More info <see href="https://github.com/xamarin/xamarin-macios/issues/4923#issuecomment-435827504"/>.
	/// Must be a ref struct to seize a reference at stack.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public ref struct NSIndexPathRef
	{
		readonly IntPtr _reference;

		internal NSIndexPathRef(IntPtr reference) => _reference = reference;

		public nint Length => Messaging.nint_objc_msgSend(_reference, Selector.GetHandle("length"));
		public nint Row => Messaging.nint_objc_msgSend(_reference, Selector.GetHandle("row"));
		public nint Item => Messaging.nint_objc_msgSend(_reference, Selector.GetHandle("item"));
		public nint Section => Messaging.nint_objc_msgSend(_reference, Selector.GetHandle("section"));
	}
}