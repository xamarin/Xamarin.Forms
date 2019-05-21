using NUnit.Framework;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Xamarin.Forms.Xaml.UnitTests
{
	[TestFixture]
	public class AssemblyInfoTests
	{
		static readonly string[] references = new[]
		{
			"Xamarin.Forms.Core",
			"Xamarin.Forms.Maps",
			"Xamarin.Forms.Xaml",
			"Xamarin.Forms.Build.Tasks",
			"Xamarin.Forms.Platform",
		};

		const string s_productName = "Xamarin.Forms";

		const string s_company = "Microsoft";

		string gitInfo => File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "..", "GitInfo.txt"));

		[Test, TestCaseSource("references")]
		public void AssemblyTitle(string assemblyName)
		{
			Assembly testAssembly = System.Reflection.Assembly.Load(assemblyName);
			Assert.AreEqual(assemblyName, testAssembly.GetName().Name);
		}

		[Test, TestCaseSource("references")]
		public void AssemblyVersion(string assemblyName)
		{
			Assembly testAssembly = System.Reflection.Assembly.Load(assemblyName);			
			Version version = testAssembly.GetName().Version;
			Version gitInfoVersion = Version.Parse(gitInfo);
			Assert.AreEqual(version.Major, gitInfoVersion.Major);
			Assert.AreEqual(version.Minor, gitInfoVersion.Minor);
			Assert.AreEqual(version.Build, gitInfoVersion.Build);
		}

		[Test, TestCaseSource("references")]
		public void FileVersion(string assemblyName)
		{
			Assembly testAssembly = System.Reflection.Assembly.Load(assemblyName);
			FileVersionInfo version = FileVersionInfo.GetVersionInfo(testAssembly.Location);
			Version gitInfoVersion = Version.Parse(gitInfo);
			Assert.AreEqual(version.FileMajorPart, gitInfoVersion.Major);
			Assert.AreEqual(version.FileMinorPart, gitInfoVersion.Minor);
			Assert.AreEqual(version.FileBuildPart, gitInfoVersion.Build);
			Assert.AreEqual(version.ProductName, s_productName);
			Assert.AreEqual(version.CompanyName, s_company);
		}

		[Test, TestCaseSource("references")]
		public void ProductAndCompany(string assemblyName)
		{
			Assembly testAssembly = System.Reflection.Assembly.Load(assemblyName);
			FileVersionInfo version = FileVersionInfo.GetVersionInfo(testAssembly.Location);
			Assert.AreEqual(version.ProductName, s_productName);
			Assert.AreEqual(version.CompanyName, s_company);
		}
	}
}
