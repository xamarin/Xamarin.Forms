using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Maui.Handlers
{
	public partial class LayoutHandler : ILayoutHandler
	{
		public static PropertyMapper<ILayout> LayoutMapper = new PropertyMapper<ILayout>(ViewHandler.ViewMapper)
		{
		};

		public LayoutHandler() : base(LayoutMapper)
		{

		}

		public LayoutHandler(PropertyMapper mapper) : base(mapper ?? LayoutMapper)
		{

		}
	}
}
