using System;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
    public class ShellNavigatingDeferral
    {
        Action _completed;

        internal ShellNavigatingDeferral(Action completed)
        {
            _completed = completed;
        }

        public void Complete()
        {
            _completed?.Invoke();
            _completed = null;
        }
    }
}