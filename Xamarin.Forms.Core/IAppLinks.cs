using System;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public interface IAppLinks
	{
		Task DeregisterLink(IAppLinkEntry appLink);
		Task DeregisterLink(Uri appLinkUri);
		Task RegisterLink(IAppLinkEntry appLink);
	}
}