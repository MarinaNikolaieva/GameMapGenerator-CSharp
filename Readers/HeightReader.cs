using MapGeneration.Utility;
using FastExcel;
using System.Drawing;

namespace MapGeneration.Readers
{
    public class HeightReader
    {
        private Dictionary<int, HeightObject> heights;
        private string folderPath = "";

        public HeightReader(string path)
        {
            heights = new Dictionary<int, HeightObject>();
            folderPath = path;
        }

        public void readHeightsTxt()
        {
            string filePath = folderPath + "\\HeightChart.txt";
            int counter = 0;
            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] parts = line.Split(' ');
                int lowerLimit = 0;
                if (parts[0].Equals("MinInf"))
                    lowerLimit = int.MinValue;
                else
                    lowerLimit = int.Parse(parts[0]);
                Color color = ColorTranslator.FromHtml(parts[1]);
                float temperatureCoef = (float)double.Parse(parts[2]);
                heights.Add(lowerLimit, new HeightObject("Height" + counter, color, temperatureCoef, 0F));
                counter++;
            }
        }

        public void readHeightsXlsx()
        {
            string filePath = folderPath + "\\HeightChart.xlsx";
            var fileInfo = new FileInfo(filePath);
            FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(fileInfo, true);

            Worksheet worksheet = fastExcel.Read(1);

            int rCount = worksheet.Rows.Count();
            int cCount = worksheet.Rows.ElementAt(0).Cells.Count();

            int limitIndex = 0;
            int nameIndex = 0;
            int colorIndex = 0;
            int temperIndex = 0;
            int moistIndex = 0;
            List<string> cellValues = new List<string>();

            for (int i = 0; i < rCount; i++)
            {
                Row row = worksheet.Rows.ElementAt(i);
                for (int j = 0; j < row.Cells.Count(); j++)
                {
                    cellValues.Add(row.Cells.ElementAt(j).Value.ToString());
                }
                if (i == 0)
                {
                    limitIndex = cellValues.FindIndex(c => c.Equals("Lower Limit"));
                    nameIndex = cellValues.FindIndex(c => c.Equals("Name"));
                    colorIndex = cellValues.FindIndex(c => c.Equals("Color Code"));
                    temperIndex = cellValues.FindIndex(c => c.Equals("Temperature Coef"));
                    moistIndex = cellValues.FindIndex(c => c.Equals("Moisture Coef"));
                }
                else
                {
                    int lowerLimit = 0;
                    if (row.Cells.ElementAt(limitIndex).ToString().Equals("MinInf"))
                        lowerLimit = int.MinValue;
                    else
                        lowerLimit = int.Parse(row.Cells.ElementAt(limitIndex).ToString());
                    Color color = ColorTranslator.FromHtml(row.Cells.ElementAt(colorIndex).ToString());
                    float temperatureCoef = (float)double.Parse(row.Cells.ElementAt(temperIndex).ToString());
                    float moistureCoef = (float)double.Parse(row.Cells.ElementAt(moistIndex).ToString());
                    string name = row.Cells.ElementAt(nameIndex).ToString();
                    heights.Add(lowerLimit, new HeightObject(name, color, temperatureCoef, moistureCoef));
                }
                cellValues.Clear();
            }
        }

        public Dictionary<int, HeightObject> getHeightChart()
        {
            return heights;
        }
    }
}
