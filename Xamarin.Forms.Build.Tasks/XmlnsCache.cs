using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using IOPath = System.IO.Path;

namespace Xamarin.Forms.Build.Tasks
{
	class XmlnsCache : IDisposable
	{
		readonly string[] _references;
		readonly List<XmlnsDefinitionAttribute> _xmlnsDefinitions = new List<XmlnsDefinitionAttribute>();
		readonly Dictionary<string, ModuleDefinition> _xmlnsModules = new Dictionary<string, ModuleDefinition>();
		bool _initialized;

		public List<XmlnsDefinitionAttribute> XmlnsDefinitions
		{
			get { Initialize(); return _xmlnsDefinitions; }
		}

		public Dictionary<string, ModuleDefinition> XmlnsModules
		{
			get { Initialize(); return _xmlnsModules; }
		}

		public XmlnsCache(string[] references)
		{
			_references = references;
		}

		void Initialize()
		{
			if (_initialized)
				return;
			_initialized = true;

			var paths = _references?.ToList() ?? new List<string>();
			//Load xmlnsdef from Core and Xaml
			paths.Add(typeof(Label).Assembly.Location);
			paths.Add(typeof(Xamarin.Forms.Xaml.Extensions).Assembly.Location);

			foreach (var path in paths.Distinct())
			{
				string asmName = IOPath.GetFileName(path);
				if (AssemblyIsSystem(asmName))
					// Skip the myriad "System." assemblies and others
					continue;

				using (var asmDef = AssemblyDefinition.ReadAssembly(path))
				{
					foreach (var ca in asmDef.CustomAttributes)
					{
						if (ca.AttributeType.FullName == typeof(XmlnsDefinitionAttribute).FullName)
						{
							_xmlnsDefinitions.Add(ca.GetXmlnsDefinition(asmDef));
							_xmlnsModules[asmDef.FullName] = asmDef.MainModule;
						}
					}
				}
			}
		}

		bool AssemblyIsSystem(string name)
		{
			if (name.StartsWith("System.Maui", StringComparison.OrdinalIgnoreCase))
				return false;
			if (name.StartsWith("System.", StringComparison.OrdinalIgnoreCase))
				return true;
			else if (name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase))
				return true;
			else if (name.Equals("netstandard.dll", StringComparison.OrdinalIgnoreCase))
				return true;
			else
				return false;
		}

		public void Dispose()
		{
			foreach (var module in _xmlnsModules.Values)
			{
				module.Dispose();
			}
			_xmlnsModules.Clear();
		}
	}
}
