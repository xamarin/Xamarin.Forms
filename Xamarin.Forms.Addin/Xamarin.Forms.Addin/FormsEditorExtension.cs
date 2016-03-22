using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using ICSharpCode.NRefactory.TypeSystem;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Xml.StateEngine;
using MonoDevelop.XmlEditor.Gui;

namespace XamarinStudio.Forms
{
	public class FormsEditorExtension : BaseXmlEditorExtension
	{
		protected override void GetElementCompletions(CompletionDataList list)
		{
			base.GetElementCompletions(list);

			AddFormsTagCompletionData(list, GetParentElementName(0), Document.Compilation);
			AddMiscBeginTags(list);
		}

		protected override CompletionDataList GetAttributeCompletions(IAttributedXObject attributedOb,
			Dictionary<string, string> existingAtts)
		{
			var list = base.GetAttributeCompletions(attributedOb, existingAtts) ?? new CompletionDataList();
			if (!existingAtts.ContainsKey("x:Name"))
				list.Add("x:Name");

			GetType(attributedOb, delegate(IType type, ICompilation dom) { AddControlMembers(list, type, existingAtts); },
				Document.Compilation);
			return list.Count > 0 ? list : null;
		}

		static void AddFormsTagCompletionData(CompletionDataList list, XName parentName, ICompilation db)
		{
			if (db == null)
				return;
			foreach (var namespc in new[] { "Xamarin.Forms" })
			{
				var ns = db.RootNamespace;
				foreach (var sn in namespc.Split('.'))
					ns = ns.GetChildNamespace(sn);
				foreach (var t in ListControlClasses(db, ns))
					list.Add(t.Name, Stock.GoForward, t.GetDefinition().Documentation);
			}
		}

		static IEnumerable<IType> ListControlClasses(ICompilation database, INamespace namespac)
		{
			if (database == null)
				yield break;

			foreach (var cls in namespac.Types.Where(t => t.Accessibility == Accessibility.Public && !t.IsAbstract))
				yield return cls;
		}

		static IEnumerable<T> GetUniqueMembers<T>(IEnumerable<T> members) where T : IMember
		{
			var existingItems = new Dictionary<string, bool>();
			foreach (var item in members)
			{
				if (existingItems.ContainsKey(item.Name))
					continue;
				existingItems[item.Name] = true;
				yield return item;
			}
		}

		void GetType(IAttributedXObject attributedOb, Action<IType, ICompilation> action, ICompilation db)
		{
			if (db == null)
				return;

			foreach (var namespc in new[] { "Xamarin.Forms" })
			{
				var controlType = db.FindType(new FullTypeName(new TopLevelTypeName(namespc, attributedOb.Name.Name, 0)));

				if (controlType != null)
				{
					action(controlType, db);
					break;
				}
			}
		}

		static void AddControlMembers(CompletionDataList list, IType controlClass,
			Dictionary<string, string> existingAtts)
		{
			//add atts only if they're not already in the tag
			foreach (var prop in GetUniqueMembers(controlClass.GetProperties()))
			{
				if (prop.IsPublic && (existingAtts == null || !existingAtts.ContainsKey(prop.Name)))
					list.Add(prop.Name, Stock.GoForward, prop.Documentation);
			}

			//similarly add events
			foreach (var eve 
				in GetUniqueMembers(controlClass.GetEvents()))
			{
				var eveName = eve.Name;
				if (eve.IsPublic && (existingAtts == null || !existingAtts.ContainsKey(eveName)))
					list.Add(eveName, Stock.GoForward, eve.Documentation);
			}
		}
	}
}