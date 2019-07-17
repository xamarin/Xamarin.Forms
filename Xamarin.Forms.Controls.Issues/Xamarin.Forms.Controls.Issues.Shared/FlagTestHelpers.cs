﻿using System.Collections.Generic;

namespace Xamarin.Forms.Controls.Issues
{
	internal static class FlagTestHelpers
	{
		public static void SetTestFlag(string flag)
		{
			Device.SetFlags(new List<string>(Device.Flags ?? new List<string>()) { flag });
		}

		public static void SetCollectionViewTestFlag()
		{
			SetTestFlag("CollectionView_Experimental");
		}
	}
}
