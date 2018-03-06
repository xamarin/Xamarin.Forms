using System.Runtime.CompilerServices;

using Xamarin.Forms.Internals;

[assembly: InternalsVisibleTo("Xamarin.Forms.Xaml.UnitTests")]
[assembly: InternalsVisibleTo("Xamarin.Forms.Build.Tasks")]
[assembly: InternalsVisibleTo("Xamarin.Forms.Xaml.Design")]
[assembly: InternalsVisibleTo("Xamarin.Forms.Loader")]

[assembly: Preserve]

[assembly: XmlnsDefinition("http://xamarin.com/schemas/2014/forms", "Xamarin.Forms.Xaml")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml", "Xamarin.Forms.Xaml")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml", "System", AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml", "System", AssemblyName = "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2009/xaml", "Xamarin.Forms.Xaml")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2009/xaml", "System", AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2009/xaml", "System", AssemblyName = "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]

[assembly: TypeForwardedTo(typeof(Xamarin.Forms.Xaml.Internals.INameScopeProvider))]

/// <summary>
/// This class documents the internal members that are used by other teams and 
/// that would require a heads-up if they need to be changed.
/// </summary>
/// <remarks>
/// Xamarin.Forms.Loader:<see cref="Xamarin.Forms.Xaml.XamlLoader.Load(object, string)">kzu@microsoft.com</see>
/// </remarks>
static class InternalsVisibleTo
{
}