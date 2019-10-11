using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.GroupingGalleries
{
	[Preserve(AllMembers = true)]
	class Team : List<Member>
	{
		public Team(string name, string city, List<Member> members) : base(members)
		{
			Name = name;
			City = city;
		}

		public string Name { get; set; }
		public string City { get; }

		public override string ToString()
		{
			return Name;
		}
	}

	[Preserve(AllMembers = true)]
	class Member
	{
		public Member(string name) => Name = name;

		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}

	[Preserve(AllMembers = true)]
	class SuperTeams : List<Team>
	{
		public SuperTeams()
		{
			Add(new Team("Avengers", "New York City",
				new List<Member>
				{
					new Member("Thor"),
					new Member("Captain America"),
					new Member("Iron Man"),
					new Member("The Hulk"),
					new Member("Ant-Man"),
					new Member("Wasp"),
					new Member("Hawkeye"),
					new Member("Black Panther"),
					new Member("Black Widow"),
					new Member("Doctor Druid"),
					new Member("She-Hulk"),
					new Member("Mockingbird"),
				}
			));

			Add(new Team("Fantastic Four", "New York City",
				new List<Member>
				{
					new Member("The Thing"),
					new Member("The Human Torch"),
					new Member("The Invisible Woman"),
					new Member("Mr. Fantastic"),
				}
			));

			Add(new Team("Defenders", "New York City",
				new List<Member>
				{
					new Member("Doctor Strange"),
					new Member("Namor"),
					new Member("Hulk"),
					new Member("Silver Surfer"),
					new Member("Hellcat"),
					new Member("Nighthawk"),
					new Member("Yellowjacket"),
				}
			));
			
			Add(new Team("Heroes for Hire", "New York City",
				new List<Member>
				{
					new Member("Luke Cage"),
					new Member("Iron Fist"),
					new Member("Misty Knight"),
					new Member("Colleen Wing"),
					new Member("Shang-Chi"),
				}
			));

			Add(new Team("West Coast Avengers", "Los Angeles",
				new List<Member>
				{
					new Member("Hawkeye"),
					new Member("Mockingbird"),
					new Member("War Machine"),
					new Member("Wonder Man"),
					new Member("Tigra"),
				}
			));

			Add(new Team("Great Lakes Avengers", "Milwaukee",
				new List<Member>
				{
					new Member("Squirrel Girl"),
					new Member("Dinah Soar"),
					new Member("Mr. Immortal"),
					new Member("Flatman"),
					new Member("Doorman"),
				}
			));
		}
	}

	[Preserve(AllMembers = true)]
	class ObservableTeam : ObservableCollection<Member>
	{
		public ObservableTeam(string name, List<Member> members) : base(members)
		{
			Name = name;
		}

		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}

	[Preserve(AllMembers = true)]
	class ObservableSuperTeams : ObservableCollection<ObservableTeam>
	{
		public ObservableSuperTeams ()
		{
			Add(new ObservableTeam("Avengers", 
				new List<Member>
				{
					new Member("Thor"),
					new Member("Captain America"),
					new Member("Iron Man"),
					new Member("The Hulk"),
					new Member("Ant-Man"),
					new Member("Wasp"),
					new Member("Hawkeye"),
					new Member("Black Panther"),
					new Member("Black Widow"),
					new Member("Doctor Druid"),
					new Member("She-Hulk"),
					new Member("Mockingbird"),
				}
			));

			Add(new ObservableTeam("Fantastic Four", 
				new List<Member>
				{
					new Member("The Thing"),
					new Member("The Human Torch"),
					new Member("The Invisible Woman"),
					new Member("Mr. Fantastic"),
				}
			));

			Add(new ObservableTeam("Defenders", 
				new List<Member>
				{
					new Member("Doctor Strange"),
					new Member("Namor"),
					new Member("Hulk"),
					new Member("Silver Surfer"),
					new Member("Hellcat"),
					new Member("Nighthawk"),
					new Member("Yellowjacket"),
				}
			));
			
			Add(new ObservableTeam("Heroes for Hire", 
				new List<Member>
				{
					new Member("Luke Cage"),
					new Member("Iron Fist"),
					new Member("Misty Knight"),
					new Member("Colleen Wing"),
					new Member("Shang-Chi"),
				}
			));

			Add(new ObservableTeam("West Coast Avengers", 
				new List<Member>
				{
					new Member("Hawkeye"),
					new Member("Mockingbird"),
					new Member("War Machine"),
					new Member("Wonder Man"),
					new Member("Tigra"),
				}
			));

			Add(new ObservableTeam("Great Lakes Avengers", 
				new List<Member>
				{
					new Member("Squirrel Girl"),
					new Member("Dinah Soar"),
					new Member("Mr. Immortal"),
					new Member("Flatman"),
					new Member("Doorman"),
				}
			));
		}
	}

	[Preserve(AllMembers = true)]
	class City
	{
		SuperTeams _teams = new SuperTeams();

		public City(string name)
		{
			Name = name;
		}

		public string Name { get; set; }

		public List<Team> Teams
		{
			get { return _teams.Where(t => t.City == Name).ToList(); }
		}

		public List<Member> Heroes
		{
			get { return Teams.SelectMany(t => t).ToList(); }
		}
	}

	[Preserve(AllMembers = true)]
	class Cities : List<City>
	{
		public Cities()
		{
			var cityNames = new SuperTeams().Select(team => team.City).Distinct().OrderBy(city => city);

			foreach (var superCity in cityNames.Select(name => new City(name)))
			{
				Add(superCity);
			}
		}
	}
}
