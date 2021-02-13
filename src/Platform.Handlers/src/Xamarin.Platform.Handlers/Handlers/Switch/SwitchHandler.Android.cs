﻿using Android.Graphics.Drawables;
using Android.Widget;
using Xamarin.Forms;
using ASwitch = AndroidX.AppCompat.Widget.SwitchCompat;

namespace Xamarin.Platform.Handlers
{
	public partial class SwitchHandler : AbstractViewHandler<ISwitch, ASwitch>
	{
		CheckedChangeListener ChangeListener { get; } = new CheckedChangeListener();

		protected override ASwitch CreateNativeView()
		{
			return new ASwitch(Context);
		}

		protected override void ConnectHandler(ASwitch nativeView)
		{
			ChangeListener.Handler = this;
			nativeView.SetOnCheckedChangeListener(ChangeListener);
		}

		protected override void DisconnectHandler(ASwitch nativeView)
		{
			ChangeListener.Handler = null;
			nativeView.SetOnCheckedChangeListener(null);
		}

		protected override void SetupDefaults(ASwitch nativeView)
		{
		}

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			Size size = base.GetDesiredSize(widthConstraint, heightConstraint);

			if (size.Width == 0)
			{
				int width = (int)widthConstraint;

				if (widthConstraint <= 0)
					width = Context != null ? (int)Context.GetThemeAttributeDp(global::Android.Resource.Attribute.SwitchMinWidth) : 0;

				size = new Size(width, size.Height);
			}

			return size;
		}

		public static void MapIsToggled(SwitchHandler handler, ISwitch view)
		{
			ViewHandler.CheckParameters(handler, view);
			handler.TypedNativeView?.UpdateIsToggled(view);
		}

		public static void MapOnColor(SwitchHandler handler, ISwitch view)
		{
			ViewHandler.CheckParameters(handler, view);
			handler.TypedNativeView?.UpdateOnColor(view);
		}

		public static void MapThumbColor(SwitchHandler handler, ISwitch view)
		{
			ViewHandler.CheckParameters(handler, view);
			handler.TypedNativeView?.UpdateThumbColor(view);
		}

		void OnCheckedChanged(bool isToggled)
		{
			if (VirtualView == null)
				return;

			VirtualView.IsToggled = isToggled;
			TypedNativeView?.UpdateOnColor(VirtualView);
		}

		void UpdateOnColor()
		{
			if (VirtualView == null || TypedNativeView == null)
				return;

			TypedNativeView?.UpdateThumbColor(VirtualView);
		}

		class CheckedChangeListener : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
		{
			public SwitchHandler? Handler { get; set; }

			void CompoundButton.IOnCheckedChangeListener.OnCheckedChanged(CompoundButton? buttonView, bool isToggled)
			{
				Handler?.OnCheckedChanged(isToggled);
				Handler?.UpdateOnColor();
			}
		}
	}
}