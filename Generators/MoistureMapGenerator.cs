using MapGeneration.Readers;
using MapGeneration.Utility;

using System.Drawing;

namespace MapGeneration.Generators
{
    public class MoistureMapGenerator
    {
        private int size;
        private float[,] map;
        //CommonGenerator is the place where all the generations take place, including the height influence
        TemperMoistPartialGenerator commonGenerator;
        private Dictionary<float, MoistureObject> moistureChart;

        //If you need a map with no heights, run this
        public MoistureMapGenerator(int size, MoistureReader moistReader,
            TemperMoistPartialGenerator temperMoistPartialGenerator, float minMapVal)
        {
            commonGenerator = temperMoistPartialGenerator;
            map = commonGenerator.RunPartialGeneration(minMapVal);
            this.size = size;
            moistureChart = moistReader.getMoistureChart();
        }

        //If you need a height factor included, run this
        public MoistureMapGenerator(int size, MoistureReader moistReader,
            TemperMoistPartialGenerator temperMoistPartialGenerator, float minMapVal, HeightReader heightReader, 
            int[,] heightMap, int minHeight, int heightRange, float maxValueCoef)
        {
            commonGenerator = temperMoistPartialGenerator;
            map = commonGenerator.RunFullGeneration(minMapVal, heightReader, heightMap, minHeight, heightRange, maxValueCoef, 1);
            this.size = size;
            moistureChart = moistReader.getMoistureChart();
        }

        public Bitmap MakePicture()
        {
            Bitmap image = new Bitmap(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int h = 0; h < moistureChart.Count; h++)
                    {
                        float temp = map[i, j];
                        if (map[i, j] >= moistureChart.ElementAt(h).Key)
                        {
                            image.SetPixel(i, j, moistureChart.ElementAt(h).Value.Color);
                            break;
                        }
                    }
                }
            }
            return image;
        }

        public void MakeMapParts(MapPart[,] mapParts)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int h = 0; h < moistureChart.Count; h++)
                    {
                        float temp = map[i, j];
                        if (map[i, j] >= moistureChart.ElementAt(h).Key)
                        {
                            mapParts[i, j].Moisture = moistureChart.ElementAt(h).Value;
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

        public void RunGenerationMapParts(MapPart[,] mapParts)
        {
            MakeMapParts(mapParts);
        }
    }
}
