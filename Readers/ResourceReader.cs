using FastExcel;
using MapGeneration.Utility;
using System.Drawing;

namespace MapGeneration.Readers
{
    public class ResourceReader
    {
        private List<BasicResource> resources;
        private string folderPath;

        public ResourceReader(string folderPath)
        {
            this.folderPath = folderPath;
            resources = new List<BasicResource>();
        }

        public void readResourcesXlsx()
        {
            string filePath = folderPath + "\\ResourceChart.xlsx";
            var fileInfo = new FileInfo(filePath);
            FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(fileInfo, true);

            Worksheet worksheet = fastExcel.Read(1);

            int rCount = worksheet.Rows.Count();
            int cCount = worksheet.Rows.ElementAt(0).Cells.Count();

            int idIndex = 0;
            int nameIndex = 0;
            int colorIndex = 0;
            int categIndex = 0;
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
                    idIndex = cellValues.FindIndex(c => c.Equals("ID"));
                    nameIndex = cellValues.FindIndex(c => c.Equals("Name"));
                    colorIndex = cellValues.FindIndex(c => c.Equals("Color Code"));
                    categIndex = cellValues.FindIndex(c => c.Equals("Category"));
                }
                else
                {
                    int id = int.Parse(row.Cells.ElementAt(idIndex).ToString());
                    Color color = ColorTranslator.FromHtml(row.Cells.ElementAt(colorIndex).ToString());
                    string name = row.Cells.ElementAt(nameIndex).ToString();
                    string category = row.Cells.ElementAt(categIndex).ToString();
                    resources.Add(new BasicResource(id, name, color, category));
                }
                cellValues.Clear();
            }
        }

        public List<BasicResource> getResources()
        {
            return resources;
        }
    }
}
