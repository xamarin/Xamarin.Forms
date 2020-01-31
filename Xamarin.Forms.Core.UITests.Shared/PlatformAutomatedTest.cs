using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Xamarin.Forms.Core.UITests.Shared
{
	internal class PlatformAutomatedTest : BaseTestFixture
	{
		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.PlatformAutomatedTestsGallery);
		}

		[Test]
		[Category(UITestCategories.ViewBaseTests)]
		public void AutomatedTests()
		{
			App.WaitForElement("SUCCESS", timeout: TimeSpan.FromMinutes(1));
		}

		protected override void TestTearDown()
		{
			base.TestTearDown();

			System.Diagnostics.Debug.WriteLine($">>>>>> Attempting to parse external test result XML");

#if __IOS__
			var crossPlatformTestXml = (string)App.Invoke("getCrossPlatformTestResults");
			var nativePlatformTestXml = (string)App.Invoke("getNativePlatformTestResults");
#elif __ANDROID__
			var crossPlatformTestXml = (string)App.Invoke("GetCrossPlatformTestResults");
			var nativePlatformTestXml = (string)App.Invoke("GetNativePlatformTestResults");
#else
			var crossPlatformTestXml = "";
			var nativePlatformTestXml = "";
#endif

			try
			{
				if (string.IsNullOrEmpty(crossPlatformTestXml))
				{
					throw new Exception(">>>>>> crossPlatformTestXml value came back empty from Invoke");
				}

				if (string.IsNullOrEmpty(nativePlatformTestXml))
				{
					throw new Exception(">>>>>> nativePlatformTestXml value came back empty from Invoke");
				}

				System.Diagnostics.Debug.WriteLine($"crossPlatformTestXml: ${crossPlatformTestXml}");
				System.Diagnostics.Debug.WriteLine($"nativePlatformTestXml: ${nativePlatformTestXml}");

				var executionContext = TestExecutionContext.CurrentContext;

				var crossPlatformResult = new PlatformTestResult(executionContext.CurrentTest, crossPlatformTestXml);
				var nativePlatformResult = new PlatformTestResult(executionContext.CurrentTest, nativePlatformTestXml);

				executionContext.CurrentResult = new CombinedTestResult(executionContext.CurrentTest, crossPlatformResult, nativePlatformResult);
			}
			catch (Exception ex) 
			{
				var message = $"{ex.Message}| crossPlatformTestXml: ${crossPlatformTestXml}| nativePlatformTestXml: ${nativePlatformTestXml}";

				throw new Exception(message, ex);
			}
		}
	}

	public class CombinedTestResult : TestResult
	{
		public override int FailCount { get => Children.Sum(t => t.FailCount); }
		public override int WarningCount { get => Children.Sum(t => t.WarningCount); }
		public override int PassCount { get => Children.Sum(t => t.PassCount); }
		public override int SkipCount { get => Children.Sum(t => t.SkipCount); }
		public override int InconclusiveCount { get => Children.Sum(t => t.InconclusiveCount);  }
		public override bool HasChildren { get => true; }
		public override IEnumerable<ITestResult> Children { get; }

		public CombinedTestResult(ITest test, TestResult a, TestResult b) : base(test)
		{
			Children = new List<ITestResult>() { a, b };
		}
	}

	public class PlatformTestResult : TestResult
	{
		readonly XmlNode _node;

		public override int FailCount { get; }
		public override int WarningCount { get; }
		public override int PassCount { get; }
		public override int SkipCount { get; }
		public override int InconclusiveCount { get; }
		public override bool HasChildren { get => false; }
		public override IEnumerable<ITestResult> Children { get; }

		public PlatformTestResult(ITest test, string externalResultXml) : base(test)
		{
			var doc = new XmlDocument();
			doc.LoadXml(externalResultXml);
			_node = doc.FirstChild;

			// Take care of the override values
			FailCount = Convert.ToInt32(_node.Attributes["failed"].Value);
			WarningCount = Convert.ToInt32(_node.Attributes["warnings"].Value);
			PassCount = Convert.ToInt32(_node.Attributes["passed"].Value);
			SkipCount = Convert.ToInt32(_node.Attributes["skipped"].Value);
			InconclusiveCount = Convert.ToInt32(_node.Attributes["inconclusive"].Value);
		}

		public override TNode AddToXml(TNode parentNode, bool recursive)
		{
			var node = base.AddToXml(parentNode, recursive);

			Replace(node, _node, "result");
			Replace(node, _node, "start-time");
			Replace(node, _node, "end-time");
			Replace(node, _node, "duration");
			Replace(node, _node, "total");
			Replace(node, _node, "asserts");

			Replace(node, "failed", FailCount.ToString());
			Replace(node, "warnings", WarningCount.ToString());
			Replace(node, "passed", PassCount.ToString());
			Replace(node, "skipped", SkipCount.ToString());
			Replace(node, "inconclusive", InconclusiveCount.ToString());

			// Inject the external test results into the children of this TNode
			var testSuiteNodes = ToTNodes(_node);

			foreach (var internalNode in testSuiteNodes)
			{
				node.ChildNodes.Add(internalNode);
			}

			return node;
		}

		static IEnumerable<TNode> ToTNodes(XmlNode xmlNode)
		{
			foreach (var child in xmlNode.ChildNodes)
			{
				var node = child as XmlNode;
				if (node.NodeType == XmlNodeType.Element)
				{
					yield return ToTNode(node);
				}
			}
		}

		static TNode ToTNode(XmlNode xmlNode)
		{
			switch (xmlNode.Name)
			{
				case "message":
				case "stack-trace":
					return ToTextElement(xmlNode);
				case "assertion":
				case "assertions":
				case "failure":
					return ToComplexElement(xmlNode);
				case "test-case":
					return ToTestCase(xmlNode);
				default:
					break;
			}

			return TNode.FromXml(xmlNode.OuterXml);
		}

		static TNode ToTextElement(XmlNode xmlNode)
		{
			return new TNode(xmlNode.Name, xmlNode.InnerText, true);
		}

		static TNode ToComplexElement(XmlNode xmlNode)
		{
			var tnode = new TNode(xmlNode.Name);

			foreach (var a in xmlNode.Attributes)
			{
				var attribute = (XmlAttribute)a;
				tnode.AddAttribute(attribute.Name, attribute.Value);
			}

			foreach (var c in xmlNode.ChildNodes)
			{
				var childNode = (XmlNode)c;
				tnode.ChildNodes.Add(ToTNode(childNode));
			}

			return tnode;
		}

		static TNode ToTestCase(XmlNode xmlNode)
		{
			if (xmlNode.ChildNodes.Count == 0)
			{
				return TNode.FromXml(xmlNode.OuterXml);
			}

			var clone = xmlNode.CloneNode(false);
			var tnode = TNode.FromXml(clone.OuterXml);

			foreach (var c in xmlNode.ChildNodes)
			{
				var n = c as XmlNode;
				tnode.ChildNodes.Add(ToTNode(n));
			}

			return tnode;
		}

		void Replace(TNode thisNode, XmlNode externalNode, string key)
		{
			thisNode.Attributes.Remove(key);
			thisNode.AddAttribute(key, externalNode.Attributes[key].Value);
		}

		void Replace(TNode thisNode, string key, string value)
		{
			thisNode.Attributes.Remove(key);
			thisNode.AddAttribute(key, value);
		}
	}
}
