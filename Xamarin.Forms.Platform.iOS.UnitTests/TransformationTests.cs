using System.Collections;
using System.Threading.Tasks;
using CoreAnimation;
using NUnit.Framework;

namespace Xamarin.Forms.Platform.iOS.UnitTests
{
	[TestFixture]
	public class TransformationTests : PlatformTestFixture
	{
		static IEnumerable TransformationCases
		{
			get
			{
				foreach (var element in BasicViews)
				{
					element.TranslationX = 10.0;
					element.TranslationY = 30.0;
					element.Rotation = 27.0;
					element.Scale = 2.0;
					yield return CreateTestCase(element);
				}
			}
		}

		[Test, Category("Transformation"), TestCaseSource(nameof(TransformationCases))]
		[Description("View transformation should match renderer transformation")]
		public async Task TransformationConsistent(View view)
		{
			var expected = new CATransform3D
			{
				m11 = 1.78f,
				m12 = 0.91f,
				m21 = -0.91f,
				m22 = 1.78f,
				m33 = 2f,
				m41 = 10f,
				m42 = 30f,
				m44 = 1f,
			};
			var actual = await GetRendererProperty(view, r => r.NativeView.Layer.Transform, requiresLayout: true);
			AssertTransform3DEqual(expected, actual, 0.01);
		}

		private static void AssertTransform3DEqual(CATransform3D expected, CATransform3D actual, double delta)
		{
			Assert.AreEqual(expected.m11, actual.m11, delta);
			Assert.AreEqual(expected.m12, actual.m12, delta);
			Assert.AreEqual(expected.m13, actual.m13, delta);
			Assert.AreEqual(expected.m14, actual.m14, delta);

			Assert.AreEqual(expected.m21, actual.m21, delta);
			Assert.AreEqual(expected.m22, actual.m22, delta);
			Assert.AreEqual(expected.m23, actual.m23, delta);
			Assert.AreEqual(expected.m24, actual.m24, delta);

			Assert.AreEqual(expected.m31, actual.m31, delta);
			Assert.AreEqual(expected.m32, actual.m32, delta);
			Assert.AreEqual(expected.m33, actual.m33, delta);
			Assert.AreEqual(expected.m34, actual.m34, delta);

			Assert.AreEqual(expected.m41, actual.m41, delta);
			Assert.AreEqual(expected.m42, actual.m42, delta);
			Assert.AreEqual(expected.m43, actual.m43, delta);
			Assert.AreEqual(expected.m44, actual.m44, delta);
		}
	}
}