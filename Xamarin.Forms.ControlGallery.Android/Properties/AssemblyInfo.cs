﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Android.App;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android;
       
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle ("Xamarin.Forms.ControlGallery.Android")]
[assembly: AssemblyDescription ("")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("")]
[assembly: AssemblyProduct ("Xamarin.Forms.ControlGallery.Android")]
[assembly: AssemblyCopyright ("Copyright ©  2013")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyCulture ("")]
[assembly: ComVisible (false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion ("1.0.0.0")]
[assembly: AssemblyFileVersion ("1.0.0.0")]

// Add some common permissions, these can be removed if not needed
[assembly: UsesPermission (Android.Manifest.Permission.Internet)]
[assembly: UsesPermission (Android.Manifest.Permission.WriteExternalStorage)]

[assembly: Android.App.MetaData("com.google.android.maps.v2.API_KEY", Value = "AIzaSyAdstcJQswxEjzX5YjLaMcu2aRVEBJw39Y")]
[assembly: Xamarin.Forms.ResolutionGroupName ("XamControl")]

// Deliberately broken image source and handler so we can test handling of image loading errors
[assembly: ExportImageSourceHandler(typeof(FailImageSource), typeof(BrokenImageSourceHandler))]
#if TEST_LEGACY_RENDERERS
[assembly: ExportRenderer(typeof(Button), typeof(Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer))]
[assembly: ExportRenderer(typeof(Image), typeof(Xamarin.Forms.Platform.Android.ImageRenderer))]
[assembly: ExportRenderer(typeof(Label), typeof(Xamarin.Forms.Platform.Android.LabelRenderer))]
[assembly: ExportRenderer(typeof(Frame), typeof(Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer))]
#endif