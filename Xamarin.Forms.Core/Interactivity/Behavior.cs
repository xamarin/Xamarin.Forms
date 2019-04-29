using System;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	public abstract class Behavior : BindableObject, IAttachedObject
	{
		protected Behavior() : this(typeof(BindableObject))
		{
		}

		internal Behavior(Type associatedType) => AssociatedType = associatedType ?? throw new ArgumentNullException(nameof(associatedType));

		protected Type AssociatedType { get; }

		void IAttachedObject.AttachTo(BindableObject bindable)
		{
			if (bindable == null)
				throw new ArgumentNullException(nameof(bindable));
			if (!AssociatedType.IsInstanceOfType(bindable))
				throw new InvalidOperationException("bindable not an instance of AssociatedType");
			OnAttachedTo(bindable);
		}

		void IAttachedObject.DetachFrom(BindableObject bindable) => OnDetachingFrom(bindable);

		protected virtual void OnAttachedTo(BindableObject bindable)
		{
		}

		protected virtual void OnDetachingFrom(BindableObject bindable)
		{
		}
	}

	public abstract class Behavior<T> : Behavior where T : BindableObject
	{
		protected Behavior() : base(typeof(T))
		{
		}

		public T AssociatedObject { get; private set; }

		protected override void OnAttachedTo(BindableObject bindable)
		{
			base.OnAttachedTo(bindable);
			AssociatedObject = (T)bindable;

			if(AssociatedObject.BindingContext != null)
			{
				BindingContext = AssociatedObject.BindingContext;
			}

			AssociatedObject.BindingContextChanged += OnBindingContextChanged;

			OnAttachedTo((T)bindable);
		}

		protected virtual void OnAttachedTo(T bindable)
		{
		}

		protected override void OnDetachingFrom(BindableObject bindable)
		{
			OnDetachingFrom((T)bindable);
			AssociatedObject.BindingContextChanged -= OnBindingContextChanged;
			AssociatedObject = null;
			base.OnDetachingFrom(bindable);
		}

		protected virtual void OnDetachingFrom(T bindable)
		{
		}

		void OnBindingContextChanged(object sender, EventArgs e)
		{
			BindingContext = AssociatedObject.BindingContext;
		}
	}
}