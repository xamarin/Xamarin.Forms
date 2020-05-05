using System;
using System.Linq;
using System.Net;
using Foundation;

#if __MOBILE__
namespace Xamarin.Forms.Platform.iOS
#else

namespace Xamarin.Forms.Platform.MacOS
#endif
{
	internal static class CookieExtensions
	{
		public static Cookie ToCookie(this NSHttpCookie nscookie)
		{
			Cookie cookie = new Cookie()
			{
				Comment = nscookie.Comment,
				CommentUri = nscookie.CommentUrl,
				Domain = nscookie.Domain,
				Expires = nscookie.ExpiresDate.ToDateTime(),
				HttpOnly = nscookie.IsHttpOnly,
				Name = nscookie.Name,
				Path = nscookie.Path,
				Secure = nscookie.IsSecure,
				Value = nscookie.Value,
				Version = (int)nscookie.Version,
				Port = String.Join(",", nscookie.PortList.Select(x => x.ToString()))
			};

			return cookie;
		}
	}
}