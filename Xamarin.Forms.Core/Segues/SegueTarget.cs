using System;
using System.ComponentModel;

namespace Xamarin.Forms
{
	public class SegueTarget
	{
		object target;

		// This is set in the DataTemplate case below, or when Reject is called
		WeakReference cachedValue;

		// We don't know what type a DataTemplate will construct until we actually create it.
		//  Here we attempt to avoid creating a lot of garbage when TryCreateValue is called
		//  for types other than what our DataTemplate returns..
		Type templatedType;

		// Allow platforms to register their own SegueTarget subclasses that will
		//  be used for implicit conversion from DataTemplate, etc..
		protected static Func<object,SegueTarget> CreateFromObjectHandler { private get; set; } =
			obj => new SegueTarget(obj);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsTemplate {
			get {
				switch (target)
				{
					case Type _: return true;
					case DataTemplate _: return true;
				}
				return false;
			}
		}

		public Page Page {
			get {
				if (IsTemplate)
					throw new InvalidOperationException("Target is a template");
				return TryCreatePage();
			}
		}

		protected SegueTarget(object target)
		{
			this.target = target;
		}

		// Convenience
		internal Page TryCreatePage()
		{
			return (Page)TryCreateValue(typeof(Page));
		}

		/// <summary>
		/// For internal use.
		/// </summary>
		/// <remarks>
		/// Generally, you will want to pass <see cref="Page"/> or a platform-specific
		///  type for <see cref="targetType"/>. This call might be expensive.
		/// </remarks>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual object TryCreateValue(Type targetType)
		{
			var cached = cachedValue?.Target;
			if (cached != null && targetType.IsAssignableFrom(cached.GetType()))
			{
				cachedValue = null;
				return cached;
			}
			if (targetType.IsAssignableFrom(target.GetType()))
				return target;
			switch (target)
			{
				case Type ty when targetType.IsAssignableFrom(ty):
					return Activator.CreateInstance(ty);

				case DataTemplate dt when templatedType == null || targetType.IsAssignableFrom(templatedType):
				{
					var content = dt.CreateContent();
					if (templatedType != null)
						return content;

					templatedType = content.GetType();
					if (targetType.IsAssignableFrom(templatedType))
						return content;

					// Oops, we can't use content this time. But at least we have
					//  a chance of using it next time if it's not collected..
					Reject(content);
					break;
				}
			}
			return null;
		}

		protected void Reject(object value)
		{
			if (IsTemplate)
				cachedValue = new WeakReference(value);
		}

		protected static SegueTarget CreateFromObject(object obj)
		{
			return (obj == null) ? null : CreateFromObjectHandler(obj);
		}

		public static implicit operator SegueTarget(Type ty) => CreateFromObject(ty);
		public static implicit operator SegueTarget(DataTemplate dt) => CreateFromObject(dt);

		// The conversion from Page is explicit because we don't want people accidently
		//  using raw Page in XAML when they should be using DataTemplate..
		public static explicit operator SegueTarget(Page page) => CreateFromObject(page);
	}
}
