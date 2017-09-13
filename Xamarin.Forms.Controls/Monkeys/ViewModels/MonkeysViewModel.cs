using System.Collections.ObjectModel;
using Xamarin.Forms.Controls.Monkeys.Helpers;
using Xamarin.Forms.Controls.Monkeys.Models;

namespace Xamarin.Forms.Controls.Monkeys.ViewModels
{
    public class MonkeysViewModel : BindableObject
    {
        public MonkeysViewModel()
        {
            Monkeys = MonkeyHelper.Monkeys;
            MonkeysGrouped = MonkeyHelper.MonkeysGrouped;
        }

        public ObservableCollection<Helpers.Grouping<string, Monkey>> MonkeysGrouped { get; set; }

        public ObservableCollection<Monkey> Monkeys { get; set; }
    }
}
