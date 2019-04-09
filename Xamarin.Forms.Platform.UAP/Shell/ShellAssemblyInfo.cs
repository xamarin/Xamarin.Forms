// TODO: Move this into ./Properties/AssemblyInfo.cs
// We keep this here to keep all Shell related code isolated for now

using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Shell), typeof(ShellRenderer))]
