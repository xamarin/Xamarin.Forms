using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.Forms;
//using Xamarin.Forms.Core.UnitTests;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.TabIndexTest
{
	public partial class TabIndex : ContentPage
	{
		public TabIndex()
		{
			InitializeComponent();
		}

		public TabIndex(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		//[TestFixture]
		//public class Tests
		//{
		//	[SetUp]
		//	public void Setup()
		//	{
		//		//Device.PlatformServices = new MockPlatformServices();
		//	}

		//	[TearDown]
		//	public void TearDown()
		//	{
		//		Device.PlatformServices = null;
		//	}

		//	[TestCase(false)]
		//	[TestCase(true)]
		//	public void TabIndexIsProperlySet(bool useCompiledXaml)
		//	{
		//		var layout = new TabIndex(useCompiledXaml);

		//		SortedDictionary<int, List<ITabStopElement>> tabIndexes = null;
		//		foreach (var child in layout.LogicalChildren)
		//		{
		//			if (!(child is VisualElement ve))
		//				continue;

		//			tabIndexes = ve.GetSortedTabIndexesOnParentPage();
		//			break;
		//		}

		//		if (tabIndexes == null)
		//			return;

		//		Assert.That(tabIndexes.Any());

		//		Assert.AreEqual(3, tabIndexes[0].Count, "Too many items in group 0");
		//		Assert.AreEqual(tabIndexes[0][0], layout.scroll);
		//		Assert.AreEqual(tabIndexes[0][1], layout.stack);
		//		Assert.AreEqual(tabIndexes[0][2], layout.composite);

		//		Assert.AreEqual(2, tabIndexes[1].Count, "Too many items in group 1");
		//		Assert.AreEqual(tabIndexes[1][0], layout.label);
		//		Assert.IsAssignableFrom(typeof(Frame), tabIndexes[1][1]);
		//		Assert.AreEqual("Sunday", AutomationProperties.GetName(((Frame)tabIndexes[1][1])));

		//		Assert.AreEqual(1, tabIndexes[2].Count, "Too many items in group 2");
		//		Assert.IsAssignableFrom(typeof(Frame), tabIndexes[2][0]);
		//		Assert.AreEqual("Monday", AutomationProperties.GetName(((Frame)tabIndexes[2][0])));

		//		Assert.AreEqual(1, tabIndexes[3].Count, "Too many items in group 3");
		//		Assert.IsAssignableFrom(typeof(Frame), tabIndexes[3][0]);
		//		Assert.AreEqual("Tuesday", AutomationProperties.GetName(((Frame)tabIndexes[3][0])));

		//		Assert.AreEqual(1, tabIndexes[4].Count, "Too many items in group 4");
		//		Assert.IsAssignableFrom(typeof(Frame), tabIndexes[4][0]);
		//		Assert.AreEqual("Wednesday", AutomationProperties.GetName(((Frame)tabIndexes[4][0])));

		//		Assert.AreEqual(1, tabIndexes[5].Count, "Too many items in group 5");
		//		Assert.IsAssignableFrom(typeof(Frame), tabIndexes[5][0]);
		//		Assert.AreEqual("Thursday", AutomationProperties.GetName(((Frame)tabIndexes[5][0])));

		//		Assert.AreEqual(1, tabIndexes[6].Count, "Too many items in group 6");
		//		Assert.IsAssignableFrom(typeof(Frame), tabIndexes[6][0]);
		//		Assert.AreEqual("Friday", AutomationProperties.GetName(((Frame)tabIndexes[6][0])));

		//		Assert.AreEqual(1, tabIndexes[7].Count, "Too many items in group 7");
		//		Assert.IsAssignableFrom(typeof(Frame), tabIndexes[7][0]);
		//		Assert.AreEqual("Saturday", AutomationProperties.GetName(((Frame)tabIndexes[7][0])));

		//		Assert.IsFalse(tabIndexes.ContainsKey(8), "Something unexpected in group 8");
		//		Assert.IsFalse(tabIndexes.ContainsKey(9), "Something unexpected in group 9");

		//		Assert.AreEqual(1, tabIndexes[10].Count, "Too many items in group 10");
		//		Assert.AreEqual(tabIndexes[10][0], layout.label2);
		//		Assert.AreEqual(1, tabIndexes[11].Count, "Too many items in group 11");
		//		Assert.AreEqual(tabIndexes[11][0], layout.timePicker);
		//		Assert.AreEqual(1, tabIndexes[12].Count, "Too many items in group 12");
		//		Assert.AreEqual(tabIndexes[12][0], layout.label3);
		//		Assert.AreEqual(1, tabIndexes[13].Count, "Too many items in group 13");
		//		Assert.AreEqual(tabIndexes[13][0], layout.timePicker2);
		//	}
		//}
	}
}