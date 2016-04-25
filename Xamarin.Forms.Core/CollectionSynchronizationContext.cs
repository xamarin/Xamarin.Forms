using System;
using System.Collections;

namespace Xamarin.Forms
{
	internal sealed class CollectionSynchronizationContext
	{
		CollectionSynchronizationCallback _callback;
		WeakReference _contextReference;

		internal CollectionSynchronizationContext(object context, CollectionSynchronizationCallback callback)
		{
			_contextReference = new WeakReference(context);
			_callback = callback;
		}

		internal void Callback(IEnumerable collection, Action accessMethod, bool writeAccess = false)
		{
			_callback(collection, _contextReference.Target, accessMethod, writeAccess);
		}

		internal CollectionSynchronizationCallback CallbackDelegate => _callback;
		internal WeakReference ContextReference => _contextReference;
	}
}