using FastExcel;
using MapGeneration.Utility;
using System.Drawing;

namespace MapGeneration.Readers
{
    public class CountryReader
    {
        private List<CountryBase> countries;
        private string folderPath;

        public CountryReader(string folderPath)
        {
            this.folderPath = folderPath;
            countries = new List<CountryBase>();
        }

        public void readCountriesXlsx()
        {
            string filePath = folderPath + "\\CountryList.xlsx";
            var fileInfo = new FileInfo(filePath);
            FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(fileInfo, true);

            Worksheet worksheet = fastExcel.Read(1);

            int rCount = worksheet.Rows.Count();

            int nameIndex = 0;
            int colorIndex = 0;
            List<string> cellValues = new List<string>();

            for (int i = 0; i < rCount; i++)
            {
                Row row = worksheet.Rows.ElementAt(i);
                for (int j = 0; j < row.Cells.Count(); j++)  //Get all cell values from the table row
                {
                    cellValues.Add(row.Cells.ElementAt(j).Value.ToString());
                }
                if (i == 0)  //Get indexes of the columns - these may vary, but are set in the first row
                {
                    nameIndex = cellValues.FindIndex(c => c.Equals("Name"));
                    colorIndex = cellValues.FindIndex(c => c.Equals("Color Code"));
                }
                else  //Get the values for current biome
                {
                    Color color = ColorTranslator.FromHtml(row.Cells.ElementAt(colorIndex).ToString());
                    string name = row.Cells.ElementAt(nameIndex).ToString();
                    CountryBase curCountry = new CountryBase(name, color);
                    countries.Add(curCountry);
                }
                cellValues.Clear();  //Clear the array of values before going to the next row
            }
        }

        public List<CountryBase> GetCountries()
        {
            return countries;
        }
    }
}
