using System;
using AppKit;
using Foundation;

namespace Xamarin.Forms.Platform.macOS
{
	[Register("FormsApplication")]
	public class FormsApplication : NSApplication
	{
		public FormsApplication(NSCoder coder) : base(coder)
		{
		}

		protected FormsApplication(NSObjectFlag t) : base(t)
		{
		}

		protected internal FormsApplication(IntPtr handle) : base(handle)
		{
		}

		public override void SendEvent(NSEvent theEvent)
		{
			if (theEvent.Type == NSEventType.KeyDown)
			{
				if ((theEvent.ModifierFlags & NSEventModifierMask.DeviceIndependentModifierFlagsMask) == NSEventModifierMask.CommandKeyMask)
					switch (theEvent.KeyCode)
					{
						case (ushort)NSKey.V:
							if (SendAction(new ObjCRuntime.Selector("paste:"), null, this))
								return;
							break;
						case (ushort)NSKey.C:
							if (SendAction(new ObjCRuntime.Selector("copy:"), null, this))
								return;
							break;
						case (ushort)NSKey.X:
							if (SendAction(new ObjCRuntime.Selector("cut:"), null, this))
								return;
							break;
						case (ushort)NSKey.Z:
							if (SendAction(new ObjCRuntime.Selector("undo:"), null, this))
								return;
							break;
						case (ushort)NSKey.A:
							if (SendAction(new ObjCRuntime.Selector("selectAll:"), null, this))
								return;
							break;
					}
				else if ((theEvent.ModifierFlags & NSEventModifierMask.DeviceIndependentModifierFlagsMask) == (NSEventModifierMask.ShiftKeyMask | NSEventModifierMask.CommandKeyMask))
				{
					switch (theEvent.KeyCode)
					{
						case (ushort)NSKey.Z:
							if (SendAction(new ObjCRuntime.Selector("redo:"), null, this))
								return;
							break;
					}
				}
			}

			base.SendEvent(theEvent);
		}
	}
}
