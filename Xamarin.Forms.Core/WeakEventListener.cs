using System;
using System.ComponentModel;

namespace Xamarin.Forms
{
	[EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class WeakEventListener<TInstance, TSource, TEventArgs>
        where TInstance : class
    {
        private readonly WeakReference _weakInstance;

        public WeakEventListener(TInstance instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _weakInstance = new WeakReference(instance);
        }

        public Action<TInstance, TSource, TEventArgs> OnEventAction { get; set; }

        public Action<WeakEventListener<TInstance, TSource, TEventArgs>> OnDetachAction { get; set; }

        public void OnEvent(TSource source, TEventArgs eventArgs)
        {
            TInstance target = (TInstance)_weakInstance.Target;

            if (target != null)
                OnEventAction?.Invoke(target, source, eventArgs);
            else
                Detach();
        }

        public void Detach()
        {
            OnDetachAction?.Invoke(this);
            OnDetachAction = null;
        }
    }
}