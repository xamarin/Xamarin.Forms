using AppKit;
using System;
using System.Linq;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.MacOS
{
    public class SwipeViewRenderer : ViewRenderer<SwipeView, NSView>
    {
        SwipeView swipeView;

        protected override void OnElementChanged(ElementChangedEventArgs<SwipeView> e) {
            base.OnElementChanged(e);

            if ( Control == null )
                SetNativeControl(new NSView());

            if ( e.NewElement is SwipeView sv ) {
                swipeView = sv;
                if ( sv.IsVisible )
                    Control.Menu = BuildMenu();
            }
        }

        NSMenu BuildMenu() {
            var menu = new NSMenu("SwipeView");
            swipeView.LeftItems
                     .Union(swipeView.RightItems)
                     .Union(swipeView.TopItems)
                     .Union(swipeView.BottomItems)
                     .Select(item => item as SwipeItem)
                     .ForEach(item => menu.AddItem(new NSMenuItemWithCommand(item.Text, (Command)item.Command, item.CommandParameter, (s, e) => {
                         if ( s is NSMenuItemWithCommand menuItem )
                             menuItem.Command?.SafelyExecute(menuItem.CommandParameter);
                     })));
            return menu;
        }

        class NSMenuItemWithCommand : NSMenuItem
        {

            public NSMenuItemWithCommand(string title, Command command, object commandParameter, EventHandler handler) : base(title, handler) {
                Command = command;
                CommandParameter = commandParameter;
            }

            public Command Command { get; set; }
            public object CommandParameter { get; set; }
        }
    }
}
