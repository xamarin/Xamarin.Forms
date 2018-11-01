using System;
using System.Linq;
using static System.String;

namespace Xamarin.Forms
{
	public static class CoreFlags
	{
		internal const string CollectionViewExperimental = "CollectionView_Experimental";

		public static void VerifyCollectionViewFlagEnabled(
			string constructorHint = null,
			[System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
		{
			if (!Application.Flags.Contains(CoreFlags.CollectionViewExperimental))
			{
				if (!IsNullOrEmpty(memberName))
				{
					if (!IsNullOrEmpty(constructorHint))
					{
						constructorHint = constructorHint + " ";
					}

					var call = $"('{constructorHint}{memberName}')";

					var errorMessage = $"The class, property, or method you are attempting to use {call} is part of "
						+ "CollectionView; to use it, you must opt-in by calling "
						+  $"Forms.SetFlags(\"{CoreFlags.CollectionViewExperimental}\") before setting your Application's main page.";
					throw new InvalidOperationException(errorMessage);
				}

				var genericErrorMessage = 
					$"To use CollectionView or associated classes, you must opt-in by calling " 
					+ $"Forms.SetFlags(\"{CoreFlags.CollectionViewExperimental}\") before setting your Application's main page.";
				throw new InvalidOperationException(genericErrorMessage);
			}
		}
	}
}