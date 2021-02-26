using System;

namespace Microsoft.Maui
{
	public interface IApp
	{
		IServiceProvider? Services { get; }

		void Create();
		void Resume();
		void Pause();
		void Destroy();
	}
}