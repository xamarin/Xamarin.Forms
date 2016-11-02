using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 36911, "[Android] Cannot Remove Separator Line From A TableView", PlatformAffected.Android)]
	public class Bugzilla36911 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			var tableView = new TableView
			{
				Intent = TableIntent.Form,
				Root = new TableRoot("Table Title")
				{
					new TableSection("Section 1 Title")
					{
						new TextCell
						{
							Text = "TextCell Text",
							Detail = "TextCell Detail"
						},
						new EntryCell
						{
							Label = "EntryCell:",
							Placeholder = "default keyboard",
							Keyboard = Keyboard.Default
						}
					},
					new TableSection("Section 2 Title")
					{
						new EntryCell
						{
							Label = "Another EntryCell:",
							Placeholder = "phone keyboard",
							Keyboard = Keyboard.Telephone
						},
						new SwitchCell
						{
							Text = "SwitchCell:"
						}
					}
				}
			};

			PlatformConfiguration.AndroidSpecific.TableView.SetSectionHeaderDividerBackgroundColor(tableView.On<Android>(), Color.Red);
			PlatformConfiguration.AndroidSpecific.TableView.SetSectionDividerBackgroundColor(tableView.On<Android>(), Color.Blue);
			PlatformConfiguration.AndroidSpecific.TableView.SetDividerBackgroundColor(tableView.On<Android>(), Color.Green);
			PlatformConfiguration.AndroidSpecific.TableView.SetDividerHeight(tableView.On<Android>(), 10);

			Content = tableView;
		}
	}
}