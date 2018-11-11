using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.Forms
{
	public class RelativeBindingSource
	{
		static RelativeBindingSource _self;
		static RelativeBindingSource _templatedParent;
		Type _ancestorType = null;
		int _ancestorLevel = 1;

		public RelativeBindingSource()
		{
		}

		public RelativeBindingSource(RelativeBindingSourceMode mode)
		{
			this.Mode = mode;
		}

		public RelativeBindingSourceMode Mode
		{
			get;
			set;
		}

		public Type AncestorType
		{
			get => _ancestorType;
			set
			{
				_ancestorType = value;
				if (_ancestorType != null)
					this.Mode = RelativeBindingSourceMode.FindAncestor;
			}
		}

		public int AncestorLevel
		{
			get => _ancestorLevel;
			set
			{
				_ancestorLevel = value;
				if (_ancestorLevel > 0)
					this.Mode = RelativeBindingSourceMode.FindAncestor;
			}
		}

		public static RelativeBindingSource Self
		{
			get
			{
				return _self ?? (_self = new RelativeBindingSource(RelativeBindingSourceMode.Self));
			}
		}

		public static RelativeBindingSource TemplatedParent
		{
			get
			{
				return _templatedParent ?? (_templatedParent = new RelativeBindingSource(RelativeBindingSourceMode.TemplatedParent));
			}
		}
	}
}
