using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Forms.Internals
{
	public interface IFormsInit
	{
		void Init();
		void PostInit(Action action);
		void PreInit(Action action);
	}

	public class FormsInit : IFormsInit
	{
		readonly List<Action> _pre = new List<Action>();
		readonly List<Action> _post = new List<Action>();
		Action _init;

		public FormsInit(Action init)
		{
			_init = init;
		}

		public void PreInit(Action action)
		{
			_pre.Add(action);
		}

		public void PostInit(Action action)
		{
			_post.Add(action);
		}

		public void Init()
		{
			foreach (var initAction in _pre)
			{
				initAction();
			}

			_init();

			foreach (var initAction in _post)
			{
				initAction();
			}

			_init = null;
			_pre.Clear();
			_post.Clear();

		}
	}

	public static class FormsInitExtensions
	{
		public static IFormsInit WithFlags(this IFormsInit init, params string[] flags)
		{
			//init.PreInit(() => Forms.SetFlags(flags));
			return init;
		}
	}
}