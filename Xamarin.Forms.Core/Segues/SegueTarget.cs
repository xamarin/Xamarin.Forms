using System;
using System.ComponentModel;

using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
	public class SegueTarget
	{
		readonly Page target;
		readonly DataTemplate template;

		public bool IsTemplate => template != null;

		public SegueTarget(Page target)
		{
			if (target == null)
				throw new ArgumentNullException (nameof(target));
			this.target = target;
		}

		public SegueTarget(DataTemplate template)
		{
			if (template == null)
				throw new ArgumentNullException(nameof(template));
			this.template = template;
		}

		protected SegueTarget()
		{
		}

		/// <summary>
		/// Gets or creates the <see cref="Page"/> for this <see cref="SegueTarget"/>
		/// </summary>
		/// <remarks>
		/// If this instance was created directly from a <see cref="Page"/>, returns that instance.
		///  Otherwise, attempts to instantiate a new page from the <see cref="DataTemplate"/>.
		/// </remarks>
		public virtual Page ToPage()
		{
			if (target != null)
				return target;

			var obj = template.CreateContent();
			if (obj is Page page)
				return page;

			// This could be a native object that we can convert to a page..
			return obj.ConvertTo(typeof(Page), (Func<object>)null, null) as Page;
		}

		public static implicit operator SegueTarget(Type ty) => (ty == null) ? null : new SegueTarget(new DataTemplate(ty));
		public static implicit operator SegueTarget(DataTemplate dt) => (dt == null) ? null : new SegueTarget(dt);

		// The conversion from Page is explicit because we don't want people accidently
		//  using raw Page in XAML when they should be using DataTemplate..
		public static explicit operator SegueTarget(Page page) => (page == null) ? null : new SegueTarget(page);
	}
}
