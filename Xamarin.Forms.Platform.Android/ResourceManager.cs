using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Path = System.IO.Path;
using Xamarin.Forms.Internals;
using AndroidAppCompat = Android.Support.V7.Content.Res.AppCompatResources;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;

namespace Xamarin.Forms.Platform.Android
{
	public static class ResourceManager
	{
		static Dictionary<object, string> _resourceTypeNames = new Dictionary<object, string>();

		public static Type DrawableClass { get; set; }

		public static Type ResourceClass { get; set; }

		public static Type StyleClass { get; set; }

		public static Type LayoutClass { get; set; }

		internal static async Task<Drawable> GetFormsDrawable(this Context context, ImageSource imageSource)
		{
			if (imageSource is FileImageSource fileSource)
				return context.GetFormsDrawable(fileSource);

			var handler = Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(imageSource);
			var icon = await handler.LoadImageAsync(imageSource, context);
			var drawable = new BitmapDrawable(context.Resources, icon);
			return drawable;
		}

		internal static Drawable GetFormsDrawable(this Context context, FileImageSource fileImageSource)
		{
			var file = fileImageSource.File;
			Drawable drawable = context.GetDrawable(fileImageSource);
			if (drawable == null)
			{
				var bitmap = GetBitmap(context.Resources, file) ?? BitmapFactory.DecodeFile(file);
				if (bitmap == null)
				{
					var source = Registrar.Registered.GetHandlerForObject<IImageSourceHandler>(fileImageSource);
					bitmap = source.LoadImageAsync(fileImageSource, context).GetAwaiter().GetResult();
				}
				if (bitmap != null)
					drawable = new BitmapDrawable(context.Resources, bitmap);
			}
			return drawable;
		}

		public static Bitmap GetBitmap(this Resources resource, FileImageSource fileImageSource)
		{
			var file = fileImageSource.File;

			var bitmap = GetBitmap(resource, file);
			if (bitmap != null)
				return bitmap;

			return BitmapFactory.DecodeFile(file);
		}

		public static Bitmap GetBitmap(this Resources resource, string name)
		{
			return BitmapFactory.DecodeResource(resource, IdFromTitle(name, DrawableClass, resource));
		}

		public static Task<Bitmap> GetBitmapAsync(this Resources resource, string name)
		{
			return BitmapFactory.DecodeResourceAsync(resource, IdFromTitle(name, DrawableClass, resource));
		}

		[Obsolete("GetDrawable(this Resources, string) is obsolete as of version 2.5. "
			+ "Please use GetDrawable(this Context, string) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Drawable GetDrawable(this Resources resource, string name)
		{
			int id = IdFromTitle(name, DrawableClass, resource);
			if (id == 0)
			{
				Log.Warning("Could not load image named: {0}", name);
				return null;
			}

			return AndroidAppCompat.GetDrawable(Forms.Context, id);
		}

		public static Drawable GetDrawable(this Context context, string name)
		{
			int id = IdFromTitle(name, DrawableClass, context);

			if (id == 0)
			{
				Log.Warning("Could not load image named: {0}", name);
				return null;
			}

			return AndroidAppCompat.GetDrawable(context, id);
		}

		public static int GetDrawableByName(string name)
		{
			return IdFromTitle(name, DrawableClass);
		}

		public static int GetResourceByName(string name)
		{
			return IdFromTitle(name, ResourceClass);
		}

		public static int GetLayoutByName(string name)
		{
			return IdFromTitle(name, LayoutClass);
		}

		public static int GetStyleByName(string name)
		{
			return IdFromTitle(name, StyleClass);
		}

		public static void Init(Assembly masterAssembly)
		{
			DrawableClass = masterAssembly.GetTypes().FirstOrDefault(x => x.Name == "Drawable" || x.Name == "Resource_Drawable");
			ResourceClass = masterAssembly.GetTypes().FirstOrDefault(x => x.Name == "Id" || x.Name == "Resource_Id");
			StyleClass = masterAssembly.GetTypes().FirstOrDefault(x => x.Name == "Style" || x.Name == "Resource_Style");
			LayoutClass = masterAssembly.GetTypes().FirstOrDefault(x => x.Name == "Layout" || x.Name == "Resource_Layout");

			// If the above are null then these are just used for mapping types to names
			// This mainly applies for the case of the designer
			_resourceTypeNames.Add(DrawableClass ?? new object(), "drawable");
			_resourceTypeNames.Add(ResourceClass ?? new object(), "id");
			_resourceTypeNames.Add(StyleClass ?? new object(), "style");
			_resourceTypeNames.Add(LayoutClass ?? new object(), "layout");
		}

		static int IdFromTitle(string title, Type type)
		{
			if (title == null)
				return 0;

			string name = Path.GetFileNameWithoutExtension(title);
			int id = GetId(type, name);
			return id;
		}

		static int IdFromTitle(string title, Type type, Resources resource)
		{
			return IdFromTitle(title, type, resource, Platform.PackageName);
		}

		static int IdFromTitle(string title, Type type, Context context)
		{
			return IdFromTitle(title, type, context.Resources, context.PackageName);
		}

		static int IdFromTitle(string title, Type type, Resources resource, string packageName)
		{
			int id = 0;
			if (title == null)
				return id;

			string name = Path.GetFileNameWithoutExtension(title);

			id = GetId(type, name);

			if (id > 0)
				return id;

			if (_resourceTypeNames.TryGetValue(type, out string value))
				id = resource.GetIdentifier(name, value, packageName);

			if (id > 0)
				return id;

			id = resource.GetIdentifier(name, value, null);

			return id;
		}

		static int GetId(Type type, string memberName)
		{
			// This may legitimately be null in designer scenarios
			if (type == null)
				return 0;

			object value = null;
			var fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				var field = fields[i];
				if (field.Name == memberName)
				{
					value = field.GetValue(type);
					break;
				}
			}

			if (value == null)
			{
				var properties = type.GetProperties();
				for (int i = 0; i < properties.Length; i++)
				{
					var prop = properties[i];
					if (prop.Name == memberName)
					{
						value = prop.GetValue(type);
						break;
					}
				}
			}

			if (value is int result)
				return result;
			return 0;
		}
	}
}
