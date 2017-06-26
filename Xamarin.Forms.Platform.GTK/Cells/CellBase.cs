﻿using Gtk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Xamarin.Forms.Platform.GTK.Cells
{
    public abstract class CellBase : EventBox
    {
        private Cell _cell;
        private IList<MenuItem> _contextActions;

        public Action<object, PropertyChangedEventArgs> PropertyChanged;

        protected CellBase()
        {
            ButtonReleaseEvent += OnClick;
        }

        public Cell Cell
        {
            get { return _cell; }
            set
            {
                if (_cell == value)
                    return;

                if (_cell != null)
                    Device.BeginInvokeOnMainThread(_cell.SendDisappearing);

                _cell = value;
                UpdateCell();
                _contextActions = Cell.ContextActions;

                if (_cell != null)
                    Device.BeginInvokeOnMainThread(_cell.SendAppearing);
            }
        }

        public void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public override void Dispose()
        {
            base.Dispose();

            ButtonReleaseEvent -= OnClick;
        }

        protected virtual void UpdateCell()
        {
        }

        private void OnClick(object o, ButtonReleaseEventArgs args)
        {
            if (args.Event.Button != 3)  // Right button
            {
                return;
            }

            if (_contextActions.Any())
            {
                OpenContextMenu();
            }
        }

        private void OpenContextMenu()
        {
            var menu = new Menu();

            SetupMenuItems(menu);
            menu.ShowAll();
            menu.Popup();
        }

        private void SetupMenuItems(Menu menu)
        {
            foreach (MenuItem item in Cell.ContextActions)
            {
                var menuItem = new Gtk.ImageMenuItem(item.Text);

                string icon = item.Icon;

                if (!string.IsNullOrEmpty(icon))
                {
                    menuItem.Image = new Gtk.Image(icon);
                }

                menuItem.ButtonPressEvent += (sender, args) =>
                {
                    item.Activate();
                };

                menu.Add(menuItem);
            }
        }
    }
}
