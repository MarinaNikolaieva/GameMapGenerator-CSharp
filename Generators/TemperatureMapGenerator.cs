using MapGeneration.Readers;
using MapGeneration.Utility;

using System.Drawing;

namespace MapGeneration.Generators
{
    public class TemperatureMapGenerator
    {
        private int size;
        private float[,] map;
        //CommonGenerator is the place where all the generations take place, including the height influence
        public TemperMoistPartialGenerator commonGenerator;
        private Dictionary<float, TemperatureObject> temperatureChart;

        //If you need the map without heights, run this
        public TemperatureMapGenerator(int size, TemperatureReader temperReader, 
            TemperMoistPartialGenerator temperMoistPartialGenerator, float minMapVal)
        {
            commonGenerator = temperMoistPartialGenerator;
            map = commonGenerator.RunPartialGeneration(minMapVal);
            this.size = size;
            temperatureChart = temperReader.getTemperatureChart();
        }

        //If you need the map with heights included, run this
        public TemperatureMapGenerator(int size, TemperatureReader temperReader, 
            TemperMoistPartialGenerator temperMoistPartialGenerator, float minMapVal, HeightReader heightReader, 
            int[,] heightMap, int minHeight, int heightRange, float maxValueCoef)
        {
            commonGenerator = temperMoistPartialGenerator;
            map = commonGenerator.RunFullGeneration(minMapVal, heightReader, heightMap, minHeight, heightRange, maxValueCoef, 0);
            this.size = size;
            temperatureChart = temperReader.getTemperatureChart();
        }

        public Bitmap MakePicture()
        {
            Bitmap image = new Bitmap(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int h = 0; h < temperatureChart.Count; h++)
                    {
                        float temp = map[i, j];
                        if (map[i, j] >= temperatureChart.ElementAt(h).Key)
                        {
                            image.SetPixel(i, j, temperatureChart.ElementAt(h).Value.Color);
                            break;
                        }
                    }
                }
            }
            return image;
        }

        public void MakeMapPartsBasic(MapPart[,] mapParts)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int h = 0; h < temperatureChart.Count; h++)
                    {
                        float temp = map[i, j];
                        if (map[i, j] >= temperatureChart.ElementAt(h).Key)
                        {
                            mapParts[i, j].TemperatureBasic = temperatureChart.ElementAt(h).Value;
                            break;
                        }
                    }
                }
            }
        }

        public void MakeMapPartsFinal(MapPart[,] mapParts)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int h = 0; h < temperatureChart.Count; h++)
                    {
                        float temp = map[i, j];
                        if (map[i, j] >= temperatureChart.ElementAt(h).Key)
                        {
                            mapParts[i, j].TemperatureFinal = temperatureChart.ElementAt(h).Value;
                            break;
                        }
                    }
                }
            }
        }

        public Bitmap RunGeneration()
        {
            return MakePicture();
        }

        public void RunGenerationMapPartsFinal(MapPart[,] mapParts)
        {
            MakeMapPartsFinal(mapParts);
        }

        public void RunGenerationMapPartsBasic(MapPart[,] mapParts)
        {
            MakeMapPartsBasic(mapParts);
        }
    }
}
