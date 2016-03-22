using Mono.Addins;
using MonoDevelop;

[assembly: Addin("Forms.Addin",
	Namespace = "Xamarin",
	Version = BuildInfo.Version,
	Category = "Mobile Development")]
[assembly: AddinName("Forms Project Support")]
[assembly: AddinDescription("Support for creating, editing, compiling and running Xamarin.Forms projects")]
[assembly: AddinDependency("::MonoDevelop.Core", BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.Ide", BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.DesignerSupport", BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.XmlEditor", BuildInfo.Version)]