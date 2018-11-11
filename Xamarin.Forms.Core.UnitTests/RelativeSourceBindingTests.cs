using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Xamarin.Forms;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class RelativeSourceBindingTests : BaseTestFixture
	{
		[SetUp]
		public override void Setup()
		{
			base.Setup();
			Device.PlatformServices = new MockPlatformServices();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
			Device.PlatformServices = null;
		}

		[Test]
		public void RelativeSourceSelfBinding()
		{
			Label label = new Label
			{
				StyleId = "label1"
			};
			label.SetBinding(Label.TextProperty, new Binding(nameof(Label.StyleId))
			{
				RelativeSource = RelativeBindingSource.Self
			});
			Assert.AreEqual(label.Text, label.StyleId);
		}

		[Test]
		public void RelativeSourceAncestorTypeBinding()
		{
			StackLayout stack0 = new StackLayout
			{
				StyleId = "stack0"
			};

			StackLayout stack1 = new StackLayout
			{
				StyleId = "stack1"
			};

			Label label0 = new Label();
			Label label1 = new Label();
			Label label2 = new Label();

			stack0.Children.Add(stack1);
			stack1.Children.Add(label0);
			stack1.Children.Add(label1);

			label0.SetBinding(Label.TextProperty, new Binding
			{
				Path = nameof(StackLayout.StyleId),
				RelativeSource = new RelativeBindingSource
				{
					AncestorType = typeof(StackLayout)
				}
			});
			label1.SetBinding(Label.TextProperty, new Binding
			{
				Path = nameof(StackLayout.StyleId),
				RelativeSource = new RelativeBindingSource
				{
					AncestorType = typeof(StackLayout),
					AncestorLevel = 2
				}
			});
			label2.SetBinding(Label.TextProperty, new Binding
			{
				Path = nameof(StackLayout.StyleId),
				RelativeSource = new RelativeBindingSource
				{
					AncestorType = typeof(StackLayout),
					AncestorLevel = 10
				}
			});

			Assert.AreEqual(label0.Text, stack1.StyleId);
			Assert.AreEqual(label1.Text, stack0.StyleId);
			Assert.IsNull(label2.Text);
		}

		[Test]
		public void RelativeSourceTemplatedParentBinding()
		{
			var cc = new CustomControl
			{
				CustomText = "RelativeSource Binding!",
				ControlTemplate = new ControlTemplate(typeof(MyCustomControlTemplate))				
			};

			var realLabel = cc.LogicalChildren[0] as Label;
			Assert.AreEqual(realLabel.Text, cc.CustomText);
		}
	}

	public class CustomControl : ContentView
	{
		public CustomControl()
		{
		}

		#region string CustomText bindable property
		public static BindableProperty CustomTextProperty = BindableProperty.Create(
			"CustomText",
			typeof(string),
			typeof(CustomControl),
			null);
		public string CustomText
		{
			get
			{
				return (string)GetValue(CustomTextProperty);
			}
			set
			{
				SetValue(CustomTextProperty, value);
			}
		}
		#endregion
	}

	public class MyCustomControlTemplate : Label
	{
		public MyCustomControlTemplate()
		{
			this.SetBinding(TextProperty, new Binding()
			{
				RelativeSource = RelativeBindingSource.TemplatedParent,
				Path = nameof(CustomControl.CustomText)
			});
		}
	}
}
