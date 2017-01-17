using System;

namespace Xamarin.Forms.Xaml
{
	interface IXamlNodeVisitor
	{
		TreeVisitingMode VisitingMode { get; }

		bool StopOnDataTemplate { get; }

		bool StopOnResourceDictionary { get; }

		void Visit(ValueNode node, INode parentNode);
		void Visit(MarkupNode node, INode parentNode);
		void Visit(ElementNode node, INode parentNode);
		void Visit(RootNode node, INode parentNode);
		void Visit(ListNode node, INode parentNode);
	}

	enum TreeVisitingMode {
		TopDown,
		BottomUp
	}

	class XamlNodeVisitor : IXamlNodeVisitor
	{
		readonly Action<INode, INode> action;

		public XamlNodeVisitor(Action<INode, INode> action, TreeVisitingMode visitingMode = TreeVisitingMode.TopDown, bool stopOnDataTemplate = false)
		{
			this.action = action;
			VisitingMode = visitingMode;
			StopOnDataTemplate = stopOnDataTemplate;
		}

		public TreeVisitingMode VisitingMode { get; }

		public bool StopOnDataTemplate { get; }

		public bool StopOnResourceDictionary { get; private set; }

		public void Visit(ValueNode node, INode parentNode)
		{
			action(node, parentNode);
		}

		public void Visit(MarkupNode node, INode parentNode)
		{
			action(node, parentNode);
		}

		public void Visit(ElementNode node, INode parentNode)
		{
			action(node, parentNode);
		}

		public void Visit(RootNode node, INode parentNode)
		{
			action(node, parentNode);
		}

		public void Visit(ListNode node, INode parentNode)
		{
			action(node, parentNode);
		}
	}
}