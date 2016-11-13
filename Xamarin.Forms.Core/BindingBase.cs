using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms
{
	public abstract class BindingBase
	{
		static readonly ConditionalWeakTable<IEnumerable, CollectionSynchronizationContext> SynchronizedCollections = new ConditionalWeakTable<IEnumerable, CollectionSynchronizationContext>();

		BindingMode _mode = BindingMode.Default;
		string _stringFormat;
		object _nullValue;

		internal BindingBase()
		{
		}

		public BindingMode Mode
		{
			get { return _mode; }
			set
			{
				if (value != BindingMode.Default && value != BindingMode.OneWay && value != BindingMode.OneWayToSource && value != BindingMode.TwoWay)
					throw new ArgumentException("mode is not a valid BindingMode", "mode");

				ThrowIfApplied();

				_mode = value;
			}
		}

		public string StringFormat
		{
			get { return _stringFormat; }
			set
			{
				ThrowIfApplied();

				_stringFormat = value;
			}
		}

		public object NullValue
		{
			get { return _nullValue; }
			set
			{
				ThrowIfApplied();

				_nullValue = value;
			}
		}

		internal bool AllowChaining { get; set; }

		internal object Context { get; set; }

		internal bool IsApplied { get; private set; }

		public static void DisableCollectionSynchronization(IEnumerable collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			SynchronizedCollections.Remove(collection);
		}

		public static void EnableCollectionSynchronization(IEnumerable collection, object context, CollectionSynchronizationCallback callback)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");
			if (callback == null)
				throw new ArgumentNullException("callback");

			SynchronizedCollections.Add(collection, new CollectionSynchronizationContext(context, callback));
		}

		protected void ThrowIfApplied()
		{
			if (IsApplied)
				throw new InvalidOperationException("Can not change a binding while it's applied");
		}

		internal virtual void Apply(bool fromTarget)
		{
			IsApplied = true;
		}

		internal virtual void Apply(object context, BindableObject bindObj, BindableProperty targetProperty)
		{
			IsApplied = true;
		}

		internal abstract BindingBase Clone();

		internal virtual object GetSourceValue(object value, Type targetPropertyType)
		{
			if (StringFormat != null)
				return string.Format(StringFormat, value ?? NullValue);

			return value ?? NullValue;
		}

		internal virtual object GetTargetValue(object value, Type sourcePropertyType)
		{
			return value;
		}

		internal static bool TryGetSynchronizedCollection(IEnumerable collection, out CollectionSynchronizationContext synchronizationContext)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			return SynchronizedCollections.TryGetValue(collection, out synchronizationContext);
		}

		internal virtual void Unapply()
		{
			IsApplied = false;
		}
	}
}