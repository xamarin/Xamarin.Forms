using System;

namespace Xamarin.Forms
{
	public interface IFormsInit
	{
		void Init();
		void PostInit(Action action);
		void PreInit(Action action);
	}
}