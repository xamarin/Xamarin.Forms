using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 9539, "ListView with Label that uses FormattedText, MaxLines and LineBreakMode=\"TailTruncation\" leads to crash: offset(x) should be less than line limit(y)", PlatformAffected.Android)]
    public partial class Issue9539 : ContentPage
    {
        public Issue9539()
        {
            InitializeComponent();

            BindingContext = new Issue9539ViewModel();
        }
    }

    public class Issue9539ViewModel : INotifyPropertyChanged
    {
	    public Issue9539ViewModel()
	    {
		    Items = new ObservableCollection<string>
		    {
			    @"Hors cadre du contrôle, hauteur du garde sur la cabine non conforme, y remédier. Cette observation a un délai 90 jours depuis le 30/08/2013 (rapport n°21097.01-LE-15-002)",
			    @"Hors cadre du contrôle, placer la tôle de protection du 
				contre-poids en cuvette. Cette observation a un délai 90 jours depuis le 30/08/2013 (rapport n°21097.01-LE-15-002)",
			    @"Veuillez remédier aux observations du rapport de 1er contrôle CE et y placer le rapport de recontrôle dans le registre de sécurité (réf. Luxcontrol du 01.08.2007).
	(Réserve supérieure insuffisante). C",
			    @"La main courante n'est pas correctement fixée en cabine, y remédier. Cette observation a un délai 30 jours depuis le 13/10/2016 (rapport n°21097.02-LE-16-003)"
		    };

			GroupedItems = new ObservableCollection<ObservableCollection<string>>();
			GroupedItems.Add(Items);

	    }

	    public ObservableCollection<string> Items { get; set; }

	    public ObservableCollection<ObservableCollection<string>> GroupedItems { get; set; } 


	    public event PropertyChangedEventHandler PropertyChanged;

	    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }
    }
}