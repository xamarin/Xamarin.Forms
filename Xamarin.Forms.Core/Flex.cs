// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE.txt file in the project root
// for the license information.

using System;
using System.Collections;
using System.Runtime.InteropServices;
using static Xamarin.Flex.NativeFunctions;

namespace Xamarin.Flex
{
	// Required for instances for be pinnable on Windows.
	[StructLayout(LayoutKind.Sequential)]
	partial class Item : IEnumerable, IDisposable
	{
		private IntPtr item = IntPtr.Zero;

		public Item()
		{
			item = flex_item_new();
			CreateHandle(false);
		}

		public Item(float width, float height) : this()
		{
			this.Width = width;
			this.Height = height;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing) {
				if (item != IntPtr.Zero) {
					ReleaseHandlesWithinItem(item, false);
					flex_item_free(item);
					item = IntPtr.Zero;
				}
			}
		}

		public void Add(Item child)
		{
			ValidateNewChild(child);
			child.CreateHandle(true);
			flex_item_add(item, child.item);
			notify_changed = 1;
		}

		public void InsertAt(uint index, Item child)
		{
			ValidateNewChild(child);
			ValidateIndex(index, true);
			child.CreateHandle(true);
			flex_item_insert(item, index, child.item);
			notify_changed = 1;
		}

		public Item RemoveAt(uint index)
		{
			ValidateIndex(index, false);
			IntPtr child_item = flex_item_delete(item, index);
			Item child = ReleaseHandleForItem(child_item, false);
			child.CreateHandle(false);
			notify_changed = 1;
			return child;
		}

		public uint Count {
			get { return flex_item_count(item); }
		}

		public Item ItemAt(uint index)
		{
			ValidateIndex(index, false);
			return FlexItemFromItem(flex_item_child(item, index));
		}

		// The IndexerName() attribute is required because otherwise we clash
		// with another symbol named `Item' in the builtin library.
		[System.Runtime.CompilerServices.IndexerName("ItemIndexer")]
		public Item this[uint index] {
			get { return ItemAt(index); }
		}

		public Item Parent {
			get { return FlexItemFromItem(flex_item_parent(item)); }
		}

		public Item Root {
			get { return FlexItemFromItem(flex_item_root(item)); }
		}

		public void Layout()
		{
			if (Parent != null) {
				throw new InvalidOperationException("Layout() must be called on a root item (that hasn't been added to another item)");
			}
			if (Double.IsNaN(Width) || Double.IsNaN(Height)) {
				throw new InvalidOperationException("Layout() must be called on an item that has proper values for the Width and Height properties");
			}
			if (self_sizing_cb != IntPtr.Zero) {
				throw new InvalidOperationException("Layout() cannot be called on an item that has the SelfSizing property set");
			}
			flex_layout(item);
		}

		public float Padding {
			set {
				this.PaddingTop = this.PaddingLeft = this.PaddingRight =
					this.PaddingBottom = value;
			}
		}

		public float Margin {
			set {
				this.MarginTop = this.MarginLeft = this.MarginRight =
					this.MarginBottom = value;
			}
		}

		public void Flex(float grow = (float)Double.NaN,
				float shrink = (float)Double.NaN, float basis = (float)Double.NaN)
		{
			if (!Double.IsNaN(grow)) {
				this.Grow = grow;
			}
			if (!Double.IsNaN(shrink)) {
				this.Shrink = shrink;
			}
			if (!Double.IsNaN(basis)) {
				this.Basis = basis;
			}
		}

		public delegate void SelfSizingDelegate(Item item, ref float width,
				ref float height);

		// Keeping this as an `IntPtr' as it's blittable and required for GC
		// pinning on Windows.
		private IntPtr self_sizing_cb = IntPtr.Zero;

		private void SelfSizingFunc(IntPtr item, IntPtr sizebuf)
		{
			float[] size = { 0, 0 };
			Marshal.Copy(sizebuf, size, 0, 2);
			var d = Marshal.GetDelegateForFunctionPointer<SelfSizingDelegate>(self_sizing_cb);
			d(FlexItemFromItem(item), ref size[0], ref size[1]);
			Marshal.Copy(size, 0, sizebuf, 2);
		}

		public SelfSizingDelegate SelfSizing {
			set {
				if (value != null) {
					self_sizing_cb = Marshal.GetFunctionPointerForDelegate(value);
					flex_item_set_self_sizing(item, SelfSizingFunc);
				}
				else {
					self_sizing_cb = IntPtr.Zero;
					flex_item_set_self_sizing(item, null);
				}
			}
		}

		public class ItemEnumerator : IEnumerator
		{
			private Item item;
			private int index;

			public ItemEnumerator(Item _item)
			{
				item = _item;
				index = -1;
			}

			object IEnumerator.Current {
				get { return Current; }
			}

			public Item Current {
				get { return item.ItemAt((uint)index); }
			}

			public void Reset()
			{
				index = -1;
			}

			public bool MoveNext()
			{
				if (item.notify_changed == 1) {
					throw new InvalidOperationException("the item's children list was modified; enumeration cannot proceed");
				}
				index++;
				return index < item.Count;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		// Not a boolean as we need a blittable type for GC pinning on Windows.
		private int notify_changed = 0;
		public ItemEnumerator GetEnumerator()
		{
			notify_changed = 0;
			return new ItemEnumerator(this);
		}

		private static GCHandle? HandleOfItem(IntPtr item)
		{
			if (item == IntPtr.Zero) {
				return null;
			}
			IntPtr ptr = flex_item_get_managed_ptr(item);
			if (ptr == IntPtr.Zero) {
				return null;
			}
			return GCHandle.FromIntPtr(ptr);
		}

		private static Item FlexItemFromItem(IntPtr item)
		{
			if (item != IntPtr.Zero) {
				var ret = HandleOfItem(item);
				if (ret.HasValue) {
					return (Item)ret.Value.Target;
				}
			}
			return null;
		}

		private static Item ReleaseHandleForItem(IntPtr item, bool reset)
		{
			var ret = HandleOfItem(item);
			if (ret.HasValue) {
				GCHandle handle = ret.Value;
				Item child = (Item)handle.Target;
				if (reset) {
					child.item = IntPtr.Zero;
				}
				handle.Free();
				flex_item_set_managed_ptr(item, IntPtr.Zero);
				return child;
			}
			return null;
		}

		private static void ReleaseHandlesWithinItem(IntPtr item, bool reset)
		{
			for (uint i = 0, count = flex_item_count(item); i < count; i++) {
				ReleaseHandlesWithinItem(flex_item_child(item, i), true);
			}
			ReleaseHandleForItem(item, reset);
		}

		private void CreateHandle(bool strong)
		{
			ReleaseHandleForItem(item, false);
			GCHandle handle = GCHandle.Alloc(this,
					strong ? GCHandleType.Pinned : GCHandleType.Weak);
			flex_item_set_managed_ptr(item, GCHandle.ToIntPtr(handle));
		}

		private void ValidateNewChild(Item child)
		{
			if (this == child) {
				throw new ArgumentException("cannot add item into self");
			}
			if (child.Parent != null) {
				throw new ArgumentException("child already has a parent");
			}
		}

		private void ValidateIndex(uint index, bool inc)
		{
			uint max = Count;
			if (inc) {
				max++;
			}
			if (index >= max) {
				throw new ArgumentOutOfRangeException();
			}
		}

		partial void ValidatePropertyValue(Properties property, int value)
		{
			bool invalid_align = false;
			if (property == Properties.JustifyContent) {
				if (value == (int)Align.Auto || value == (int)Align.Stretch) {
					invalid_align = true;
				}
			}
			else if (property == Properties.AlignContent) {
				if (value == (int)Align.Auto) {
					invalid_align = true;
				}
			}
			else if (property == Properties.AlignItems) {
				if (value == (int)Align.Auto) {
					invalid_align = true;
				}
			}
			if (invalid_align) {
				throw new ArgumentException("invalid Align value");
			}
		}

		partial void ValidatePropertyValue(Properties property, float value)
		{
			if (property == Properties.PaddingLeft
					|| property == Properties.PaddingRight
					|| property == Properties.PaddingTop
					|| property == Properties.PaddingBottom
					|| property == Properties.Grow
					|| property == Properties.Shrink
					|| property == Properties.Basis) {
				if (value < 0) {
					throw new ArgumentException("invalid value (should be positive or zero)");
				}
			}
		}
	}
}
