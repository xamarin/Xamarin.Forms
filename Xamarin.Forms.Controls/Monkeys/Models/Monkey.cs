namespace Xamarin.Forms.Controls.Monkeys.Models
{
    public class Monkey
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Details { get; set; }
        public string Image { get; set; }
        public string NameSort => Name[0].ToString();
    }
}