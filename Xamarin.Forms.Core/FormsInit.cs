using System;
using System.Collections.Generic;

namespace Xamarin.Forms
{
	public class FormsInit : IFormsInit
	{
		readonly List<Action> _post = new List<Action>();
		readonly List<Action> _pre = new List<Action>();
		Action _init;

		public FormsInit(Action init)
		{
			_init = init;
		}

		public IFormsBuilder Init()
		{
			foreach (Action initAction in _pre)
			{
				initAction();
			}

			_init();

			foreach (Action initAction in _post)
			{
				initAction();
			}

			_init = null;
			_pre.Clear();
			_post.Clear();

			return new FormsBuilder();
		}

		public void PostInit(Action action)
		{
			_post.Add(action);
		}

		public void PreInit(Action action)
		{
			_pre.Add(action);
		}
	}
}