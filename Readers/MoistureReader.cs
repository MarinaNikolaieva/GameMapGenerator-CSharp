using FastExcel;
using MapGeneration.Utility;
using System.Drawing;

namespace MapGeneration.Readers
{
    public class MoistureReader
    {
        //Lower limit for generation - Name - Color
        private Dictionary<float, MoistureObject> moistures;
        private string folderPath = "";

        public MoistureReader(string path)
        {
            moistures = new Dictionary<float, MoistureObject>();
            folderPath = path;
        }

        public void readMoisturesXlsx()
        {
            string filePath = folderPath + "\\MoistureChart.xlsx";
            var fileInfo = new FileInfo(filePath);
            FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(fileInfo, true);

            Worksheet worksheet = fastExcel.Read(1);

            int rCount = worksheet.Rows.Count();
            int cCount = worksheet.Rows.ElementAt(0).Cells.Count();

            int limitIndex = 0;
            int nameIndex = 0;
            int colorIndex = 0;
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
                }
                else
                {
                    float lowerLimit = 0;
                    if (row.Cells.ElementAt(limitIndex).ToString().Equals("MinInf"))
                        lowerLimit = float.MinValue;
                    else
                        lowerLimit = (float)double.Parse(row.Cells.ElementAt(limitIndex).ToString());
                    Color color = ColorTranslator.FromHtml(row.Cells.ElementAt(colorIndex).ToString());
                    string name = row.Cells.ElementAt(nameIndex).ToString();
                    moistures.Add(lowerLimit, new MoistureObject(name, color));
                }
                cellValues.Clear();
            }
        }

        public Dictionary<float, MoistureObject> getMoistureChart()
        {
            return moistures;
        }
    }
}
