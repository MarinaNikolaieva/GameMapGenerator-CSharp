using FastExcel;
using MapGeneration.Utility;
using System.Drawing;
using System.IO;
using System.Text;

namespace MapGeneration
{
    public class CountryWriter
    {
        string folderPath;
        List<CountryBase> countries;

        public CountryWriter(string folderPath, List<CountryBase> countries)
        {
            this.folderPath = folderPath;
            this.countries = countries;
        }

        public void Run()
        {
            string outputFile = folderPath + "\\CountryTable.tsv";

            List<string> strings = new List<string>();
            for (int i = -1; i < countries.Count; i++)
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (i == -1)
                {
                    stringBuilder.Append("Name\t");
                    stringBuilder.Append("Color Code\t");
                    stringBuilder.Append("CapitalX\t");
                    stringBuilder.Append("CapitalY");
                }
                else
                {
                    stringBuilder.Append(countries.ElementAt(i).Name + "\t");
                    stringBuilder.Append(ColorTranslator.ToHtml(Color.FromArgb(countries.ElementAt(i).Color.ToArgb())) + "\t");
                    stringBuilder.Append(countries.ElementAt(i).Capital.X + "\t");
                    stringBuilder.Append(countries.ElementAt(i).Capital.Y);
                }
                strings.Add(stringBuilder.ToString());
            }
            using (StreamWriter sw = new StreamWriter(outputFile))
            {
                for (int i = 0; i < strings.Count; i++)
                {
                    sw.WriteLine(strings.ElementAt(i));
                }
            }
        }
    }
}
