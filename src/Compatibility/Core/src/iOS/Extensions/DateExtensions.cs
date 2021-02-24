using System;
using Foundation;

#if __MOBILE__
namespace Microsoft.Maui.Controls.Compatibility.Platform.iOS
#else

namespace Microsoft.Maui.Controls.Compatibility.Platform.MacOS
#endif
{
	public static class DateExtensions
	{
		public static DateTime ToDateTime(this NSDate date)
		{
			return new DateTime(2001, 1, 1, 0, 0, 0).AddSeconds(date.SecondsSinceReferenceDate);
		}

		public static NSDate ToNSDate(this DateTime date)
		{
			return NSDate.FromTimeIntervalSinceReferenceDate((date - new DateTime(2001, 1, 1, 0, 0, 0)).TotalSeconds);
		}
	}
}