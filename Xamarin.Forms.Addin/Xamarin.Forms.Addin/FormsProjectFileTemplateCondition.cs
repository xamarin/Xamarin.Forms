using System.Xml;
using MonoDevelop.Ide.Templates;
using MonoDevelop.Projects;

namespace XamarinStudio.Forms
{
	public class FormsProjectFileTemplateCondition : FileTemplateCondition
	{
		public override void Load(XmlElement element)
		{
		}

		public override bool ShouldEnableFor(Project proj, string projectPath)
		{
			return true;
		}
	}
}