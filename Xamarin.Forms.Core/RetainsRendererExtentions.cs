using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
    public static class RetainsRendererExtentions
    {
        public static Xamarin.Forms.Page SetRetainsRendererValue(this Xamarin.Forms.Page page, bool value)
        {
            SetRetainsRenderer(page, value);
            return page;
        }

        public static bool GetRetainsRendererValue(this Xamarin.Forms.Page page)
        {
            return GetRetainsRenderer(page);
        }


        public static readonly BindableProperty RetainsRendererProperty = BindableProperty.CreateAttached("RetainsRenderer", typeof(bool), typeof(RetainsRendererExtentions), false);

        public static bool GetRetainsRenderer(BindableObject view)
        {
            return (bool)view.GetValue(RetainsRendererExtentions.RetainsRendererProperty);
        }

        public static void SetRetainsRenderer(BindableObject view, bool value)
        {
            view.SetValue(RetainsRendererExtentions.RetainsRendererProperty, value);
        }
    }

}
