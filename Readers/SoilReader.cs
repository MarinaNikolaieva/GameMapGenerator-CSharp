using FastExcel;
using MapGeneration.Utility;
using System.Drawing;

namespace MapGeneration.Readers
{
    public class SoilReader
    {
        private List<Soil> Soils;

        private string folderPath;

        public SoilReader(string folderPath)
        {
            this.folderPath = folderPath;
            Soils = new List<Soil>();
        }

        public void readSoilsXlsx(BiomeReader biomeReader)
        {
            string filePath = folderPath + "\\SoilBiomeChart.xlsx";
            var fileInfo = new FileInfo(filePath);
            FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(fileInfo, true);

            Worksheet worksheet = fastExcel.Read(1);

            int rCount = worksheet.Rows.Count();
            List<Biome> biomes = biomeReader.getBiomes();

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
                    Soil curSoil = new Soil(name, color);
                    Soils.Add(curSoil);
                    //Then go the cells with the resources the biome has
                    //If the cell is not the resource name, it will be skipped
                    for (int j = 0; j < cellValues.Count; j++)
                    {
                        Biome biome = biomes.Where(r => r.getName().Equals(cellValues.ElementAt(j))).FirstOrDefault();
                        if (biome != null)
                            biome.addSoil(curSoil);
                    }
                }
                cellValues.Clear();  //Clear the array of values before going to the next row
            }
        }

        public List<Soil> getSoils()
        {
            return Soils;
        }
    }
}
