﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github5000)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3847, "RelativeSource Binding",
		PlatformAffected.All)]
	public partial class Github3847 : ContentPage
	{
#if APP
		public Github3847()
		{
			InitializeComponent();
		}
#endif

		public CompanyViewModel Company { get; } = new CompanyViewModel
		{
			Name = "Evilcorp",
			Employees = new ObservableCollection<PersonViewModel>
				{
					new PersonViewModel
					{
						FirstName = "John",
						LastName = "Doe"
					},
					new PersonViewModel
					{
						FirstName = "George",
						LastName = "Washington"
					},
					new PersonViewModel
					{
						FirstName = "Santa",
						LastName = "Claus",
					}
				}
		};
	}

	public class PersonViewModel
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string FullName => $"{FirstName} {LastName}";
	}

	public class CompanyViewModel
	{
		public string Name { get; set; }

		public ObservableCollection<PersonViewModel> Employees { get; set; }

		Command _deleteEmployeeCommand;
		public Command DeleteEmployeeCommand
		{
			get
			{
				return _deleteEmployeeCommand ?? (_deleteEmployeeCommand = new Command((employee) =>
				{
					this.Employees?.Remove(employee as PersonViewModel);
				}));
			}
		}
	}
}
