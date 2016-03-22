using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateUpdater
{
	class Program
	{
		static string[] FilesList = new[] {
			@"Xamarin.Forms.VSTemplate\T\PT\Mobile Apps\Forms\FormsTemplate\FormsTemplate.vstemplate",
			@"Xamarin.Forms.VSTemplate\T\PT\Mobile Apps\Forms\FormsTemplate.Android\FormsTemplate.Android.vstemplate",
			@"Xamarin.Forms.VSTemplate\T\PT\Mobile Apps\Forms\FormsTemplate.iOS\FormsTemplate.iOS.vstemplate",
			@"Xamarin.Forms.VSTemplate\T\PT\Mobile Apps\Forms\FormsTemplate.WinPhone\FormsTemplate.WinPhone.vstemplate",
			@"Xamarin.Forms.VSTemplate\T\PT\Mobile Apps\FormsSAP\FormsTemplate\FormsTemplate.vstemplate",
			@"Xamarin.Forms.VSTemplate\T\PT\Mobile Apps\FormsSAP\FormsTemplate.Android\FormsTemplate.Android.vstemplate",
			@"Xamarin.Forms.VSTemplate\T\PT\Mobile Apps\FormsSAP\FormsTemplate.iOS\FormsTemplate.iOS.vstemplate",
			@"Xamarin.Forms.VSTemplate\T\PT\Mobile Apps\FormsSAP\FormsTemplate.WinPhone\FormsTemplate.WinPhone.vstemplate",
			@"Xamarin.Forms.VSTemplate\Xamarin.Forms.VSTemplate.csproj",
		};

		static string[] NupackDirs = new[] {
			@"Xamarin.Forms.VSTemplate\Packages"
		};

		static string NugetPath = @"Build\NuGet\";

		static void Main (string[] args)
		{
			var packageVersion = args[0];
			bool insertMaster = args[1].ToLower () == "insertmaster";

			var filename = (insertMaster ? "Xamarin.Forms.Master." : "Xamarin.Forms.") + packageVersion + ".nupkg";

			if (!File.Exists (Path.Combine (NugetPath, filename))) {
				Console.WriteLine(Path.Combine (NugetPath, filename));
				Environment.Exit (1);
			}

			foreach (var file in FilesList) {
				var content = File.ReadAllText (file);
				
				if (insertMaster) {
					content = content.Replace ("Xamarin.Forms.1.0.6184", "Xamarin.Forms.Master.1.0.6184")
					                 .Replace ("id=\"Xamarin.Forms\"", "id=\"Xamarin.Forms.Master\"");
				}
				
				content = content.Replace ("1.0.6184", packageVersion);

				File.WriteAllText (file, content);
			}

			foreach (var dir in NupackDirs) {
				File.Copy (Path.Combine (NugetPath, filename), Path.Combine (dir, filename));
			}
		}
	}
}
