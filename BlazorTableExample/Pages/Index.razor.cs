using Microsoft.AspNetCore.Components.Server.Circuits;

namespace BlazorTableExample
{
    public partial class Index : ComponentBase
    {
        [Inject] IJSRuntime Script { get; set; }
        public List<Names> lstNames = new();
        private Names EditName = new();
        private int EditIndex = -1;
        private string NewName = string.Empty;

        private async Task SaveRow()
        {
            Names rec = lstNames.ElementAt(EditIndex);
            rec.fName = NewName;
            EditIndex = -1;
        }

        protected override async Task OnInitializedAsync()
        {
            GetNames();
            foreach (var rec in lstNames.Select((value, i) => (value, i)))
            {
                int index = rec.i;
                rec.value.nameID = index + 1;
            }

            await base.OnInitializedAsync();
        }

        private void GetNames()
        {
            lstNames.Clear();
            lstNames.Add(new Names { fName = "Liam", lName = "Karter", nameID = 0 });
            lstNames.Add(new Names { fName = "Noah     ", lName = "Timothy", nameID = 0 });
            lstNames.Add(new Names { fName = "Oliver   ", lName = "Abraham", nameID = 0 });
            lstNames.Add(new Names { fName = "Elijah   ", lName = "Jesse", nameID = 0 });
            lstNames.Add(new Names { fName = "James    ", lName = "Zayden", nameID = 0 });
            lstNames.Add(new Names { fName = "William  ", lName = "Blake", nameID = 0 });
            lstNames.Add(new Names { fName = "Benjamin ", lName = "Alejandro", nameID = 0 });
            lstNames.Add(new Names { fName = "Lucas    ", lName = "Dawson", nameID = 0 });
            lstNames.Add(new Names { fName = "Henry    ", lName = "Tristan", nameID = 0 });
            lstNames.Add(new Names { fName = "Theodore ", lName = "Victor", nameID = 0 });
            lstNames.Add(new Names { fName = "Jack     ", lName = "Avery", nameID = 0 });
            lstNames.Add(new Names { fName = "Levi     ", lName = "Joel", nameID = 0 });
            lstNames.Add(new Names { fName = "Alexander", lName = "Grant", nameID = 0 });
            lstNames.Add(new Names { fName = "Jackson  ", lName = "Eric", nameID = 0 });
            lstNames.Add(new Names { fName = "Mateo    ", lName = "Patrick", nameID = 0 });
            lstNames.Add(new Names { fName = "Daniel   ", lName = "Peter", nameID = 0 });
            lstNames.Add(new Names { fName = "Michael  ", lName = "Richard", nameID = 0 });
            lstNames.Add(new Names { fName = "Mason    ", lName = "Edward", nameID = 0 });
            lstNames.Add(new Names { fName = "Sebastian", lName = "Andres", nameID = 0 });
            lstNames.Add(new Names { fName = "Ethan    ", lName = "Emilio", nameID = 0 });
            lstNames.Add(new Names { fName = "Logan    ", lName = "Colt", nameID = 0 });
            lstNames.Add(new Names { fName = "Owen     ", lName = "Knox", nameID = 0 });
            lstNames.Add(new Names { fName = "Samuel   ", lName = "Beckham", nameID = 0 });
            lstNames.Add(new Names { fName = "Jacob    ", lName = "Adonis", nameID = 0 });
            lstNames.Add(new Names { fName = "Asher    ", lName = "Kyrie", nameID = 0 });
            lstNames.Add(new Names { fName = "Aiden    ", lName = "Matias", nameID = 0 });
            lstNames.Add(new Names { fName = "John     ", lName = "Oscar", nameID = 0 });
            lstNames.Add(new Names { fName = "Joseph   ", lName = "Lukas", nameID = 0 });
            lstNames.Add(new Names { fName = "Wyatt    ", lName = "Marcus", nameID = 0 });
            lstNames.Add(new Names { fName = "David    ", lName = "Hayes", nameID = 0 });
            lstNames.Add(new Names { fName = "Leo      ", lName = "Caden", nameID = 0 });
            lstNames.Add(new Names { fName = "Luke     ", lName = "Remington", nameID = 0 });
            lstNames.Add(new Names { fName = "Julian   ", lName = "Griffin", nameID = 0 });
            lstNames.Add(new Names { fName = "Hudson   ", lName = "Nash", nameID = 0 });
            lstNames.Add(new Names { fName = "Grayson  ", lName = "Israel", nameID = 0 });
            lstNames.Add(new Names { fName = "Matthew  ", lName = "Steven", nameID = 0 });
            lstNames.Add(new Names { fName = "Ezra     ", lName = "Holden", nameID = 0 });
            lstNames.Add(new Names { fName = "Gabriel  ", lName = "Rafael", nameID = 0 });
            lstNames.Add(new Names { fName = "Carter   ", lName = "Zane", nameID = 0 });
            lstNames.Add(new Names { fName = "Isaac    ", lName = "Jeremy", nameID = 0 });
            lstNames.Add(new Names { fName = "Jayden   ", lName = "Kash", nameID = 0 });
            lstNames.Add(new Names { fName = "Luca     ", lName = "Preston", nameID = 0 });
            lstNames.Add(new Names { fName = "Anthony  ", lName = "Kyler", nameID = 0 });
            lstNames.Add(new Names { fName = "Dylan    ", lName = "Jax", nameID = 0 });
            lstNames.Add(new Names { fName = "Lincoln  ", lName = "Jett", nameID = 0 });
            lstNames.Add(new Names { fName = "Thomas   ", lName = "Kaleb", nameID = 0 });
            lstNames.Add(new Names { fName = "Maverick ", lName = "Riley", nameID = 0 });
            lstNames.Add(new Names { fName = "Elias    ", lName = "Simon", nameID = 0 });
            lstNames.Add(new Names { fName = "Josiah   ", lName = "Phoenix", nameID = 0 });
            lstNames.Add(new Names { fName = "Charles  ", lName = "Javier", nameID = 0 });
            lstNames.Add(new Names { fName = "Caleb    ", lName = "Bryce", nameID = 0 });
            lstNames.Add(new Names { fName = "Christopher        ", lName = "Louis", nameID = 0 });
            lstNames.Add(new Names { fName = "Ezekiel  ", lName = "Mark", nameID = 0 });
            lstNames.Add(new Names { fName = "Miles    ", lName = "Cash", nameID = 0 });
            lstNames.Add(new Names { fName = "Jaxon    ", lName = "Lennox", nameID = 0 });
            lstNames.Add(new Names { fName = "Isaiah   ", lName = "Paxton", nameID = 0 });
            lstNames.Add(new Names { fName = "Andrew   ", lName = "Malakai", nameID = 0 });
            lstNames.Add(new Names { fName = "Joshua   ", lName = "Paul", nameID = 0 });
            lstNames.Add(new Names { fName = "Nathan   ", lName = "Kenneth", nameID = 0 });
            lstNames.Add(new Names { fName = "Nolan    ", lName = "Nico", nameID = 0 });
            lstNames.Add(new Names { fName = "Adrian   ", lName = "Kaden", nameID = 0 });
            lstNames.Add(new Names { fName = "Cameron  ", lName = "Lane", nameID = 0 });
            lstNames.Add(new Names { fName = "Santiago ", lName = "Kairo", nameID = 0 });
            lstNames.Add(new Names { fName = "Eli      ", lName = "Maximus", nameID = 0 });
            lstNames.Add(new Names { fName = "Aaron    ", lName = "Omar", nameID = 0 });
            lstNames.Add(new Names { fName = "Ryan     ", lName = "Finley", nameID = 0 });
            lstNames.Add(new Names { fName = "Angel    ", lName = "Atticus", nameID = 0 });
            lstNames.Add(new Names { fName = "Cooper   ", lName = "Crew", nameID = 0 });
            lstNames.Add(new Names { fName = "Waylon   ", lName = "Brantley", nameID = 0 });
            lstNames.Add(new Names { fName = "Easton   ", lName = "Colin", nameID = 0 });
            lstNames.Add(new Names { fName = "Kai      ", lName = "Dallas", nameID = 0 });
            lstNames.Add(new Names { fName = "Christian", lName = "Walter", nameID = 0 });
            lstNames.Add(new Names { fName = "Landon   ", lName = "Brady", nameID = 0 });
            lstNames.Add(new Names { fName = "Colton   ", lName = "Callum", nameID = 0 });
            lstNames.Add(new Names { fName = "Roman    ", lName = "Ronan", nameID = 0 });
            lstNames.Add(new Names { fName = "Axel     ", lName = "Hendrix", nameID = 0 });
            lstNames.Add(new Names { fName = "Brooks   ", lName = "Jorge", nameID = 0 });
            lstNames.Add(new Names { fName = "Jonathan ", lName = "Tobias", nameID = 0 });
            lstNames.Add(new Names { fName = "Robert   ", lName = "Clayton", nameID = 0 });
            lstNames.Add(new Names { fName = "Jameson  ", lName = "Emerson", nameID = 0 });
            lstNames.Add(new Names { fName = "Ian      ", lName = "Damien", nameID = 0 });
            lstNames.Add(new Names { fName = "Everett  ", lName = "Zayn", nameID = 0 });
            lstNames.Add(new Names { fName = "Greyson  ", lName = "Malcolm", nameID = 0 });
            lstNames.Add(new Names { fName = "Wesley   ", lName = "Kayson", nameID = 0 });
            lstNames.Add(new Names { fName = "Jeremiah ", lName = "Bodhi", nameID = 0 });
            lstNames.Add(new Names { fName = "Hunter   ", lName = "Bryan", nameID = 0 });
            lstNames.Add(new Names { fName = "Leonardo ", lName = "Aidan", nameID = 0 });
            lstNames.Add(new Names { fName = "Jordan   ", lName = "Cohen", nameID = 0 });
            lstNames.Add(new Names { fName = "Jose     ", lName = "Brian", nameID = 0 });
            lstNames.Add(new Names { fName = "Bennett  ", lName = "Cayden", nameID = 0 });
            lstNames.Add(new Names { fName = "Silas    ", lName = "Andre", nameID = 0 });
            lstNames.Add(new Names { fName = "Nicholas ", lName = "Niko", nameID = 0 });
            lstNames.Add(new Names { fName = "Parker   ", lName = "Maximiliano", nameID = 0 });
            lstNames.Add(new Names { fName = "Beau     ", lName = "Zander", nameID = 0 });
            lstNames.Add(new Names { fName = "Weston   ", lName = "Khalil", nameID = 0 });
            lstNames.Add(new Names { fName = "Austin   ", lName = "Rory", nameID = 0 });
            lstNames.Add(new Names { fName = "Connor   ", lName = "Francisco", nameID = 0 });
            lstNames.Add(new Names { fName = "Carson   ", lName = "Cruz", nameID = 0 });
            lstNames.Add(new Names { fName = "Dominic  ", lName = "Kobe", nameID = 0 });
            lstNames.Add(new Names { fName = "Xavier   ", lName = "Reid", nameID = 0 });
        }


        private void EditIndexChanged(Names name)
        {
            EditIndex = lstNames.IndexOf(name);
        }

        public class Names
        {
            public string fName { get; set; } = string.Empty;
            public string lName { get; set; } = string.Empty;
            public int nameID { get; set; } = -1;

        }

    }

}
