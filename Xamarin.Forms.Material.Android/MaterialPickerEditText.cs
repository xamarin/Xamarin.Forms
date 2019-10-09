
using Android.Support.V4.View;
using Android.Text;
using Java.Lang;
#if __ANDROID_28__
using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Xamarin.Forms.Platform.Android;

namespace Xamarin.Forms.Material.Android
{
	public class MaterialPickerEditText : MaterialFormsEditTextBase
	{
		bool _disposed = false;

		public MaterialPickerEditText(Context context) : base(context) => PickerManager.Init(this);

		protected MaterialPickerEditText(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) => PickerManager.Init(this);

		public MaterialPickerEditText(Context context, IAttributeSet attrs) : base(context, attrs) => PickerManager.Init(this);

		public MaterialPickerEditText(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr) => PickerManager.Init(this);

		public override bool OnTouchEvent(MotionEvent e)
		{
			PickerManager.OnTouchEvent(this, e);
			return base.OnTouchEvent(e); // raises the OnClick event if focus is already received
		}

		protected override void OnFocusChanged(bool gainFocus, [GeneratedEnum] FocusSearchDirection direction, Rect previouslyFocusedRect)
		{
			base.OnFocusChanged(gainFocus, direction, previouslyFocusedRect);
			PickerManager.OnFocusChanged(gainFocus, this, (IPopupTrigger)Parent.Parent);
		}

		public override void SetText(ICharSequence text, BufferType type)
		{
			if (ViewCompat.IsLaidOut(this) && text != null)
			{
				int textWidth = Width - CompoundPaddingLeft - CompoundPaddingRight;

				string fullText = text.ToString();
				string ellipsizedText = TextUtils.Ellipsize(fullText, Paint, textWidth, TextUtils.TruncateAt.End);

				if (!string.IsNullOrEmpty(ellipsizedText))
				{
					text = new Java.Lang.String(ellipsizedText);
				}
			}

			base.SetText(text, type);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_disposed = true;
				PickerManager.Dispose(this);
			}

			base.Dispose(disposing);
		}
	}
}
#endif