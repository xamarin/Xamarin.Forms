﻿using System.Threading.Tasks;
using Android.Widget;
using Xamarin.Forms;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public partial class LabelHandlerTests
	{
		TextView GetNativeLabel(LabelHandler labelHandler) =>
			(TextView)labelHandler.View;

		string GetNativeText(LabelHandler labelHandler) =>
			GetNativeLabel(labelHandler).Text;

		Color GetNativeTextColor(LabelHandler labelHandler) =>
		   ((uint)GetNativeLabel(labelHandler).CurrentTextColor).ToColor();

		Task ValidateNativeBackgroundColor(ILabel label, Color color)
		{
			return InvokeOnMainThreadAsync(() =>
			{
				GetNativeLabel(CreateHandler(label)).AssertContainsColor(color);
			});
		}
	}
}