using System;
using System.ComponentModel;
using Android.Content;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.Graphics.Drawable;
using ARect = Android.Graphics.Rect;

namespace Xamarin.Forms.Platform.Android
{
	public class FormsAppCompatEditText : FormsAppCompatEditTextBase, IFormsEditText
	{
		public FormsAppCompatEditText(Context context) : base(context)
		{
		}


		public override bool OnKeyPreIme(Keycode keyCode, KeyEvent e)
		{
			if (keyCode != Keycode.Back || e.Action != KeyEventActions.Down)
			{
				return base.OnKeyPreIme(keyCode, e);
			}

			this.HideKeyboard();

			_onKeyboardBackPressed?.Invoke(this, EventArgs.Empty);
			return true;
		}

		protected override void OnSelectionChanged(int selStart, int selEnd)
		{
			base.OnSelectionChanged(selStart, selEnd);
			_selectionChanged?.Invoke(this, new SelectionChangedEventArgs(selStart, selEnd));
		}

		event EventHandler _onKeyboardBackPressed;
		event EventHandler IFormsEditText.OnKeyboardBackPressed
		{
			add => _onKeyboardBackPressed += value;
			remove => _onKeyboardBackPressed -= value;
		}

		event EventHandler<SelectionChangedEventArgs> _selectionChanged;
		event EventHandler<SelectionChangedEventArgs> IFormsEditText.SelectionChanged
		{
			add => _selectionChanged += value;
			remove => _selectionChanged -= value;
		}
	}

	public class FormsAppCompatEditTextBase : AppCompatEditText, IDescendantFocusToggler
	{
		DescendantFocusToggler _descendantFocusToggler;

		public FormsAppCompatEditTextBase(Context context) : base(context)
		{
			DrawableCompat.Wrap(Background);
		}

		bool IDescendantFocusToggler.RequestFocus(global::Android.Views.View control, Func<bool> baseRequestFocus)
		{
			_descendantFocusToggler = _descendantFocusToggler ?? new DescendantFocusToggler();

			return _descendantFocusToggler.RequestFocus(control, baseRequestFocus);
		}


		public override bool RequestFocus(FocusSearchDirection direction, ARect previouslyFocusedRect)
		{
			return (this as IDescendantFocusToggler).RequestFocus(this, () => base.RequestFocus(direction, previouslyFocusedRect));
		}


	}

	[Obsolete("EntryEditText is obsolete as of version 2.4.0. Please use Xamarin.Forms.Platform.Android.FormsEditText instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class EntryAppCompatEditText : FormsAppCompatEditText
	{
		public EntryAppCompatEditText(Context context) : base(context)
		{
		}
	}

	[Obsolete("EditorEditText is obsolete as of version 2.4.0. Please use Xamarin.Forms.Platform.Android.FormsEditText instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class EditorAppCompatEditText : FormsAppCompatEditText
	{
		public EditorAppCompatEditText(Context context) : base(context)
		{
		}
	}
}