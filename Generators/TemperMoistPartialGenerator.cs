using MapGeneration.Readers;
using MapGeneration.Utility;

namespace MapGeneration.Generators
{
    public class TemperMoistPartialGenerator
    {
        //STAGES:
        //Generate number gradient (1 - middle, 0 - poles, float format)  DONE
        //Generate color gradient using temperature units  DONE
        //Generate a fractal noise  DONE
        //Multiply the noise and the gradient  DONE
        //Include heights if needed  DONE

        private int size;
        private float[,] map;
        private float[,] noiseMap;

        public TemperMoistPartialGenerator(int size)
        {
            this.size = size;
            map = new float[size, size];
            noiseMap = new float[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    map[i, j] = 0F;
                    noiseMap[i, j] = 0F;
                }
        }

        public void GenerateNumberGradient()
        {
            int middle = size / 2;
            int side = size / 2;
            float step = 1F / (float)side;
            for (int i = 0; i < size; i++)  //We go line by line
            {
                map[i, middle] = 1F;  //Putting the value to the middle
                for (int j = 0; j < side; j++)  //And going from the poles, increasing the temperature
                {
                    map[i, j] = step * j;
                    map[i, size - j - 1] = step * j;
                }
            }
        }

        public void GenerateNoise()
        {
            //REMEMBER! Noise goes from -1 to 1, temperature grad - from 0 to 1!
            //Let it be the default 3 octaves
            FastNoiseLite noise = new FastNoiseLite(new Random().Next());
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    noiseMap[i, j] += noise.GetNoise(j, i);
                }
            }
        }

        public void MapNoise(float newMinVal)
        {
            //Mapping to (0; 1) interval
            //MAYBE make the min val greater than 0
            float oldMin = -1;
            float oldMax = 1;
            float oldRange = oldMax - oldMin;  //In this library, it is strict
            float newMin = newMinVal;
            float newMax = 1;
            float newRange = newMax - newMin;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    noiseMap[i, j] = (((noiseMap[i, j] - oldMin) * newRange) / oldRange) + newMin;
                }
            }
        }

        public void MultiplyTheGrads()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    map[i, j] *= noiseMap[i, j];
                }
            }
        }

        //Generator parameter is the detector of the caller. 0 = temperature, 1 = moisture
        public void IncludeHeight(HeightReader heightReader, int[,] iHeightMap, int minVal, int range, float maxCoef, int generator)
        {
            Dictionary<int, HeightObject> heightChart = heightReader.getHeightChart();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int h = 0; h < heightChart.Count; h++)
                    {
                        if (iHeightMap[i, j] >= heightChart.ElementAt(h).Key)
                        {
                            float newMin, newMax;
                            if (generator == 0)
                            {
                                newMin = heightChart.ElementAt(h).Value.TemperatureCoeffitient;
                                newMax = h == 0 ? maxCoef : heightChart.ElementAt(h - 1).Value.TemperatureCoeffitient;
                            }
                            else if (generator == 1) {
                                newMin = heightChart.ElementAt(h).Value.MoistureCoeffitient;
                                newMax = h == 0 ? maxCoef : heightChart.ElementAt(h - 1).Value.MoistureCoeffitient;
                            }
                            else
                            {
                                newMin = 0.0F;
                                newMax = 0.0F;
                            }
                            float newRange = newMax - newMin;
                            float coef = ((iHeightMap[i, j] - minVal) * newRange / range) + newMin;
                            map[i, j] -= coef;
                            break;
                        }
                    }
                }
            }
        }

        //Get the "basic" map without height impact
        public float[,] RunPartialGeneration(float mapMinVal)
        {
            GenerateNumberGradient();
            GenerateNoise();
            MapNoise(mapMinVal);
            MultiplyTheGrads();
            return map;
        }

        //Add height impact to previously generated map
        public float[,] RunGenerWithHeightsFromPrior(HeightReader heightReader, int[,] heightMap, int minHeight,
            int heightRange, float maxValueCoef, int generatorCode)
        {
            IncludeHeight(heightReader, heightMap, minHeight, heightRange, maxValueCoef, generatorCode);
            return map;
        }

        //Run full generation without splittin the map in two
        public float[,] RunFullGeneration(float mapMinVal, HeightReader heightReader, int[,] heightMap, int minHeight, 
            int heightRange, float maxValueCoef, int generatorCode)
        {
            GenerateNumberGradient();
            GenerateNoise();
            MapNoise(mapMinVal);
            MultiplyTheGrads();
            IncludeHeight(heightReader, heightMap, minHeight, heightRange, maxValueCoef, generatorCode);
            return map;
        }
    }
}
