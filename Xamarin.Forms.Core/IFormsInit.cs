using System;

namespace Xamarin.Forms
{
	public interface IFormsInit
	{
		IFormsBuilder Init();
		void PostInit(Action action);
		void PreInit(Action action);
	}
}