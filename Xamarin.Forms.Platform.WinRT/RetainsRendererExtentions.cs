using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
    public static class RetainsRendererExtentions
    {
        public static Xamarin.Forms.Page SetRetainsRenderer(this Xamarin.Forms.Page page, bool value)
        {
            page.SetValue(Page.RetainsRendererProperty, value);
            return page;
        }
    }

}
