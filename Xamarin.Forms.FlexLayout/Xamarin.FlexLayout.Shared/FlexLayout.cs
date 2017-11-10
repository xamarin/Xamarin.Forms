using System;
using System.ComponentModel;
using System.Linq;

#if FORMS
using NativeView = Xamarin.Forms.View;

namespace Xamarin.Forms
#else
namespace Xamarin.FlexLayout
#endif
{
	public partial class FlexLayout
	{
		const string _FlexOrderPropertyName = "Order";
		const string _FlexPropertyName = "Flex";
		const string _FlexGrowPropertyName = "Grow";
		const string _FlexShrinkPropertyName = "Shrink";
		const string _FlexBasisPropertyName = "Basis";
		const string _FlexAlignSelfPropertyName = "AlignSelf";
		const string _FlexIsIncludedPropertyName = "IsIncluded";
		const string _FlexNodePropertyName = "Node";
		static Type s_engineType;

		IFlexNode _root;

		public FlexLayout()
		{
			InitNode();
		}

		public void ApplyLayout(double x, double y, double width, double height)
		{
			_root.Left = (float)x;
			_root.Top = (float)y;
			UpdateRootNode();
			AttachNodesFromViewHierachy(this);
			CalculateLayoutWithSize((float)width, (float)height);
			ApplyLayoutToViewHierarchy(this);
		}

		public static void RegisterEngine(Type engineType)
		{
			s_engineType = engineType;
		}

		static bool NodeHasExactSameChildren(IFlexNode node, NativeView[] subviews)
		{
			if (node.Count() != subviews.Length)
				return false;

			for (int i = 0; i < subviews.Length; i++)
			{
				var childNode = GetNode(subviews[i]);
				if (node.ElementAt(i) != childNode)
				{
					return false;
				}
			}
			return true;
		}

		protected virtual IFlexNode InitNode()
		{
			_root = GetNewFlexNode();
			SetNode(this, _root);
			_root.Data = this;
			UpdateRootNode();
			return _root;
		}

		void ApplyLayoutToViewHierarchy(NativeView view)
		{
			if (!GetIsIncluded(view))
				return;

			var node = GetNode(view);

			if (view != this && node != null)
				ApplyLayoutToNativeView(view, node);

			if (view.IsLeaf())
				return;

			foreach (var subView in FlexLayoutExtensions.GetChildren(view))
				ApplyLayoutToViewHierarchy(subView);
		}

		void RegisterChild(NativeView view)
		{
			if (view == null)
				throw new ArgumentNullException(nameof(view));
			IFlexNode node = GetNode(view);

			if (node == null)
			{
				node = GetNewFlexNode();
				SetNode(view, node);
				node.Data = view;
			}

			var viewINPC = view as INotifyPropertyChanged;
			if (viewINPC != null)
				viewINPC.PropertyChanged += ChildPropertyChanged;

		}

		void UnregisterChild(NativeView view)
		{
			if (view == null)
				throw new ArgumentNullException(nameof(view));
			var node = GetNode(view);
			SetNode(view, null);
			node.Data = null;
			var viewINPC = view as INotifyPropertyChanged;
			if (viewINPC != null)
				viewINPC.PropertyChanged -= ChildPropertyChanged;
		}
		void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateNode(this, sender, e.PropertyName);
		}

		Size CalculateLayoutWithSize(float width, float height)
		{
			var node = _root;

			if (!float.IsPositiveInfinity((width)))
				node.Width = width;

			if (!float.IsPositiveInfinity((height)))
				node.Height = height;

			node.CalculateLayout();
			return new Size { Width = node.LayoutWidth, Height = node.LayoutHeight };
		}

		IFlexNode GetNewFlexNode()
		{
			if (s_engineType == null)
				throw new InvalidOperationException("You must call FlexLayout.RegisterEngine");
			var instance = Activator.CreateInstance(s_engineType);
			return instance as IFlexNode;
		}
	}
}
