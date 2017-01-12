using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms
{
	public class Messaging : IMessaging
	{
		readonly Dictionary<Tuple<string, Type, Type>, List<Tuple<WeakReference, WeakReference>>> _callbacks = new Dictionary<Tuple<string, Type, Type>, List<Tuple<WeakReference, WeakReference>>>();

		static readonly Lazy<Messaging> s_lazy = new Lazy<Messaging>(() => new Messaging());

		public static Messaging Instance => s_lazy.Value;

		Messaging()
		{
		}

		public void Send<TSender, TArgs>(TSender sender, string message, TArgs args) where TSender : class
		{
			if (sender == null)
				throw new ArgumentNullException(nameof(sender));
			if(string.IsNullOrWhiteSpace(message))
				throw new ArgumentException("Message name cannot be empty.");

			InnerSend(message, typeof(TSender), typeof(TArgs), sender, args);
		}

		public void Send<TSender>(TSender sender, string message) where TSender : class
		{
			if (sender == null)
				throw new ArgumentNullException(nameof(sender));
			if (string.IsNullOrWhiteSpace(message))
				throw new ArgumentException("Message name cannot be empty.");

			InnerSend(message, typeof(TSender), null, sender, null);
		}

		public void Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender source = null) where TSender : class
		{
			if (subscriber == null)
				throw new ArgumentNullException(nameof(subscriber));
			if (string.IsNullOrWhiteSpace(message))
				throw new ArgumentException("Message name cannot be empty.");
			if (callback == null)
				throw new ArgumentNullException(nameof(callback));

			Action<object, object> wrap = (sender, args) =>
			{
				var send = (TSender)sender;
				if (source == null || send == source)
					callback((TSender)sender, (TArgs)args);
			};

			InnerSubscribe(subscriber, message, typeof(TSender), typeof(TArgs), wrap);
		}

		public void Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender source = null) where TSender : class
		{
			if (subscriber == null)
				throw new ArgumentNullException(nameof(subscriber));
			if (string.IsNullOrWhiteSpace(message))
				throw new ArgumentException("Message name cannot be empty.");
			if (callback == null)
				throw new ArgumentNullException(nameof(callback));

			Action<object, object> wrap = (sender, args) =>
			{
				var send = (TSender)sender;
				if (source == null || send == source)
					callback((TSender)sender);
			};

			InnerSubscribe(subscriber, message, typeof(TSender), null, wrap);
		}

		public void Unsubscribe<TSender, TArgs>(object subscriber, string message) where TSender : class
		{
			InnerUnsubscribe(message, typeof(TSender), typeof(TArgs), subscriber);
		}

		public void Unsubscribe<TSender>(object subscriber, string message) where TSender : class
		{
			InnerUnsubscribe(message, typeof(TSender), null, subscriber);
		}

		internal void ClearSubscribers()
		{
			_callbacks.Clear();
		}

		void InnerSend(string message, Type senderType, Type argType, object sender, object args)
		{
			var key = new Tuple<string, Type, Type>(message, senderType, argType);

			if (!_callbacks.ContainsKey(key))
				return;

			List<Tuple<WeakReference, WeakReference>> actions = _callbacks[key];
			if (actions == null || !actions.Any())
				return; // should not be reachable

			// ok so this code looks a bit funky but here is the gist of the problem. It is possible that in the course
			// of executing the callbacks for this message someone will subscribe/unsubscribe from the same message in
			// the callback. This would invalidate the enumerator. To work around this we make a copy. However if you unsubscribe 
			// from a message you can fairly reasonably expect that you will therefore not receive a call. To fix this we then
			// check that the item we are about to send the message to actually exists in the live list.
			List<Tuple<WeakReference, WeakReference>> actionsCopy = actions.ToList();
			foreach (Tuple<WeakReference, WeakReference> action in actionsCopy)
			{
				if (action.Item1.Target != null && actions.Contains(action))
					((Action<object, object>)action.Item2.Target)?.Invoke(sender, args);
			}
		}

		void InnerSubscribe(object subscriber, string message, Type senderType, Type argType, Action<object, object> callback)
		{
			var key = new Tuple<string, Type, Type>(message, senderType, argType);
			var value = new Tuple<WeakReference, WeakReference>(new WeakReference(subscriber), new WeakReference(callback));

			if (_callbacks.ContainsKey(key))
				_callbacks[key].Add(value);
			else
				_callbacks[key] = new List<Tuple<WeakReference, WeakReference>> { value };
		}

		void InnerUnsubscribe(string message, Type senderType, Type argType, object subscriber)
		{
			var key = new Tuple<string, Type, Type>(message, senderType, argType);

			if (!_callbacks.ContainsKey(key))
				return;

			_callbacks[key].RemoveAll(tuple => !tuple.Item1.IsAlive || tuple.Item1.Target == subscriber);

			if (!_callbacks[key].Any())
				_callbacks.Remove(key);
		}
	}
}