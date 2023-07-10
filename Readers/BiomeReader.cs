using FastExcel;
using MapGeneration.Utility;
using System.Drawing;
using System.Resources;

namespace MapGeneration.Readers
{
    public class BiomeReader
    {
        private List<Biome> biomes;  //List of biomes
        private Dictionary<int, List<Biome>> biomeHeightsDictionary;  //List of biomes with specified heights;
                                                                      //made for simplier reading later
        private string folderPath;

        //This is a complex reader
        //It reads the biome list itself, with resources it provides  DONE
        //It also reads the biome-height table and assigns the corresponding values  DONE
        //And it reads the moisture-temperature table and assigns the values  DONE
        public BiomeReader(string folderPath)
        {
            this.folderPath = folderPath;
            biomes = new List<Biome>();
            biomeHeightsDictionary = new Dictionary<int, List<Biome>>();
        }

        public void readBiomesXlsx(ResourceReader resourceReader)
        {
            string filePath = folderPath + "\\BiomeChart.xlsx";
            var fileInfo = new FileInfo(filePath);
            FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(fileInfo, true);

            Worksheet worksheet = fastExcel.Read(1);

            int rCount = worksheet.Rows.Count();
            List<BasicResource> resources = resourceReader.getResources();

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
                    biomes.Add(new Biome(name, color));
                    //Then go the cells with the resources the biome has
                    //If the cell is not the resource name, it will be skipped
                    for (int j = 0; j < cellValues.Count; j++)
                    {
                        BasicResource resource = resources.Where(r => r.Name.Equals(cellValues.ElementAt(j))).FirstOrDefault();
                        if (resource != null)
                            biomes.ElementAt(i - 1).addResource(resource);
                    }
                }
                cellValues.Clear();  //Clear the array of values before going to the next row
            }
        }

        public void readBiomeHeightsXlsx(TemperatureReader temperatureReader)
        {
            string filePath = folderPath + "\\BiomeHeightTable.xlsx";
            var fileInfo = new FileInfo(filePath);
            FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(fileInfo, true);

            Worksheet worksheet = fastExcel.Read(1);

            int rCount = worksheet.Rows.Count();

            int biomeIndex = 0;
            int limitIndex = 0;
            int temperIndex = 0;
            List<string> cellValues = new List<string>();
            List<TemperatureObject> temperatures = temperatureReader.getTemperatureChart().Values.ToList();

            for (int i = 0; i < rCount; i++)
            {
                Row row = worksheet.Rows.ElementAt(i);
                for (int j = 0; j < row.Cells.Count(); j++)
                {
                    cellValues.Add(row.Cells.ElementAt(j).Value.ToString());
                }
                if (i == 0)
                {
                    biomeIndex = cellValues.FindIndex(c => c.Equals("Biome"));
                    limitIndex = cellValues.FindIndex(c => c.Equals("Lower Limit"));
                    temperIndex = cellValues.FindIndex(c => c.Equals("Temperatures"));
                }
                else
                {
                    //There may be multiple biomes in one cell, so split them to process
                    string[] names = row.Cells.ElementAt(biomeIndex).ToString().Split(new char[] { ',', ' ' });
                    string[] tempers;
                    try
                    {
                        tempers = row.Cells.ElementAt(temperIndex).ToString().Split(new char[] { ',', ' ' });
                    }
                    catch (Exception)
                    {
                        tempers = new string[0];
                    }
                    int heightLimit;
                    string limit = cellValues.ElementAt(limitIndex).ToString();
                    if (limit.Equals("MinInf"))
                        heightLimit = int.MinValue;
                    else
                        heightLimit = int.Parse(limit);
                    List<Biome> tempList = new List<Biome>();
                    for (int k = 0; k < names.Length; k++)
                    {
                        if (!string.IsNullOrEmpty(names[k]))
                        {
                            //Find needed biomes by their names
                            Biome biome = biomes.Where(b => b.getName().Equals(names[k])).FirstOrDefault();
                            if (biome != null)  //If found
                            {
                                biome.addHeight(heightLimit);  //Set the min height to the biome
                                for (int t = 0; t < tempers.Length; t++)  //Set the temperatures
                                {
                                    if (!string.IsNullOrEmpty(tempers[t]))
                                    {
                                        TemperatureObject temper = temperatures.Where(tem => tem.Name.Equals(tempers[t])).
                                            FirstOrDefault();
                                        if (temper != null)  //If found, apply
                                            biome.addTemperature(temper);
                                    }
                                }
                                tempList.Add(biome);  //Add the biome to the list
                            }
                        }
                    }
                    biomeHeightsDictionary.Add(heightLimit, tempList);
                }
                cellValues.Clear();
            }

            for (int i = 0; i < biomes.Count; i++)  //Some biomes may not aquire height until now
            {
                if (biomes.ElementAt(i).getHeights().Count() == 0)
                {
                    biomeHeightsDictionary[0].Add(biomes.ElementAt(i));  //Add the biome to the list
                    biomes.ElementAt(i).addHeight(0);  //Set the height to 0
                }
            }
        }

        public void readMoistTempersXlsx(TemperatureReader temperatureReader, MoistureReader moistureReader)
        {
            string filePath = folderPath + "\\BiomeTemperatureMoistureTable.xlsx";
            var fileInfo = new FileInfo(filePath);
            FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(fileInfo, true);

            Worksheet worksheet = fastExcel.Read(1);

            int rCount = worksheet.Rows.Count();

            //We need to ALREADY know what temperature and moisture objects we are dealing with
            Dictionary<float, TemperatureObject> temperatures = temperatureReader.getTemperatureChart();
            Dictionary<float, MoistureObject> moistures = moistureReader.getMoistureChart();

            List<string> cellValues = new List<string>();
            List<TemperatureObject> temperatureOrder = new List<TemperatureObject>();

            for (int i = 0; i < rCount; i++)
            {
                Row row = worksheet.Rows.ElementAt(i);
                for (int j = 0; j < row.Cells.Count(); j++)
                {
                    cellValues.Add(row.Cells.ElementAt(j).Value.ToString());
                }
                if (i == 0)
                {
                    //The object 0 is dropped as it's the term-setting cell
                    //All others are temperature values and must be inspected
                    for (int j = 1; j < cellValues.Count(); j++)
                    {
                        //Find the corresponding temperature object
                        TemperatureObject temper = temperatures.Values.Where(t => t.Name.Equals(cellValues.ElementAt(j)))
                            .FirstOrDefault();
                        if (temper != null)
                            temperatureOrder.Add(temper);
                        else  //Or set to None if not found - "None" is the REQUIRED value to be in the table
                            temperatureOrder.Add(temperatures.Values.Where(t => t.Name.Equals("None")).FirstOrDefault());
                    }
                }
                else
                {
                    //Same with moistures as with temperatures
                    MoistureObject moist = moistures.Values.Where(m => m.Name.Equals(cellValues.ElementAt(0))).FirstOrDefault();
                    if (moist == null)
                        moist = moistures.Values.Where(m => m.Name.Equals("None")).FirstOrDefault();

                    for (int j = 1; j < cellValues.Count(); j++)
                    {
                        //Multiple biomes must be split
                        string[] names = cellValues.ElementAt(j).Split(new char[] { ',', ' ' });
                        for (int k = 0; k < names.Length; k++)
                        {
                            if (!string.IsNullOrEmpty(names[k]))
                            {
                                //Find the biome by the name
                                Biome biome = biomes.Where(b => b.getName().Equals(names[k])).FirstOrDefault();
                                if (biome != null)
                                {
                                    biome.addTemperature(temperatureOrder.ElementAt(j - 1));  //Set the temperature
                                    biome.addMoisture(moist);  //Set the moisture
                                }
                            }
                        }
                    }
                }
                cellValues.Clear();
                //Height-specific biomes depend only on height, so other parameters aren't specified
                //MAYBE do this in the future
            }
        }

        public List<Biome> getBiomes()
        {
            return biomes;
        }

        public Dictionary<int, List<Biome>> getHeightFixedBiomes()
        {
            return biomeHeightsDictionary;
        }
    }
}
