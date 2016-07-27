using System;
using System.Reflection;

namespace Xamarin.Forms
{
	class NativeViewEventListener : IDisposable
	{
		readonly string _eventName;
		readonly string _propertyName;
		Delegate _handlerDelegate;
		EventInfo eventInfo;
		object _target;
		bool _disposedValue;

		public NativeViewEventListener(object target, string eventName, string propertyName)
		{
			_eventName = eventName;
			_propertyName = propertyName;
			_target = target;
			Subscribe();
		}

		public EventHandler<NativeViewEventFiredEventArgs> NativeViewEventFired;

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					UnSubscribe();
				}

				_disposedValue = true;
			}
		}

		void Subscribe()
		{
			eventInfo = _target.GetType().GetRuntimeEvent(_eventName);

			if (eventInfo == null)
			{
				throw new ArgumentNullException($"Event not found with the name {_eventName}");
			}

			Action<object, object> handler = NativeEventFired;
			var methodInfo = handler.GetMethodInfo();
			_handlerDelegate = methodInfo.CreateDelegate(eventInfo.EventHandlerType, this);
			eventInfo.AddEventHandler(_target, _handlerDelegate);
		}

		void UnSubscribe()
		{
			eventInfo?.RemoveEventHandler(_target, _handlerDelegate);
		}

		void NativeEventFired(object sender, object e)
		{
			if (NativeViewEventFired != null)
				NativeViewEventFired(this, new NativeViewEventFiredEventArgs { NativeEventArgs = e, PropertyName = _propertyName, EventName = _eventName });
		}
	}
}

