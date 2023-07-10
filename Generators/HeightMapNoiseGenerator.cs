using System;
using System.Drawing;

using MapGeneration;
using MapGeneration.Readers;
using MapGeneration.Utility;

namespace MapGeneration.Generators
{
    public class HeightMapNoiseGenerator
    {
        private int height;
        private int width;
        private int scaledSize;
        private float[,] map;
        private float[,] scaledMap;
        private FastNoiseLite noise;

        public HeightMapNoiseGenerator(int width, int height, int scaledSize)
        {
            this.width = width;
            this.height = height;
            this.scaledSize = scaledSize;
            map = new float[height, width];
            scaledMap = new float[scaledSize, scaledSize];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    map[i, j] = 0.0F;
                }
            }
            for (int i = 0; i < scaledSize; i++)
            {
                for (int j = 0; j < scaledSize; j++)
                {
                    scaledMap[i, j] = 0.0F;
                }
            }
            noise = new FastNoiseLite(new Random().Next());  //Make the seed different with each run
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        }

        public void ChangeOctaves(int octaves)
        {
            if (octaves > 0)
                noise.SetFractalOctaves(octaves);
        }

        public void RunNoise()
        {
            //It's 3 octaves by default
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    map[i, j] += noise.GetNoise(j, i);
                }
            }
        }

        //NEEDED TO REDO THIS AND MAKE IT FUNCTIONAL
        public void RunNoiseWithWrapping()
        {
            //If we wrap a map, it could be rolled in a cylinder, like most world maps
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    //We need a 3D noise here. The next part is copied
                    //Noise range
                    float x1 = -1, x2 = 1;
                    float y1 = -1, y2 = 1;
                    float dx = x2 - x1;
                    float dy = y2 - y1;

                    //Sample noise at smaller intervals
                    float s = (float)j / (float)width;
                    float t = (float)i / (float)height;

                    // Calculate our 3D coordinates
                    float nx = (float)(x1 + Math.Cos(s * 2 * Math.PI) * dx / (2 * Math.PI));
                    float ny = (float)(x1 + Math.Sin(t * 2 * Math.PI) * dx / (2 * Math.PI));
                    float nz = t;
                    map[i, j] += noise.GetNoise(ny, nx, nz);
                }
            }
        }

        public void WeighValueMap(double power)
        {
            double invertedPower = 1.0 / power;
            double biggerPower = power >= 1.0 ? power : invertedPower;
            double lesserPower = power < 1.0 ? power : invertedPower;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bool negative = map[i, j] < 0;
                    map[i, j] = (float)Math.Pow(Math.Abs(map[i, j]), negative ? biggerPower : lesserPower);
                    if (negative)
                        map[i, j] = -map[i, j];
                }
            }
        }

        private void PreDSAInit(int power)
        {
            int scaledX = 0;
            int scaledY = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    scaledMap[scaledY, scaledX] = map[i, j];
                    scaledX += power;
                }
                scaledX = 0;
                scaledY += power;
            }
        }

        private void SquareStep(int hs, int iter)
        {
            float corner1, corner2, corner3, corner4;
            for (int y = hs; y < scaledSize; y += iter)
            {
                for (int x = hs; x < scaledSize; x += iter)
                {
                    //Calculate corners
                    corner1 = scaledMap[y - hs, x - hs];
                    corner2 = scaledMap[y + hs, x - hs];
                    corner3 = scaledMap[y - hs, x + hs];
                    corner4 = scaledMap[y + hs, x + hs];

                    //Center point will be average without offscale
                    scaledMap[y, x] = (corner1 + corner2 + corner3 + corner4) / 4;
                }
            }
        }

        private void DiamondStep(int hs, int iter)
        {
            for (int y = 0; y < scaledSize; y += hs)
            {
                for (int x = y % iter == 0 ? hs : 0; x < scaledSize; x += iter)  // getting offset of x in function of y
                {
                    double sum = 0;
                    double denominator = 0;

                    //Calculate border points
                    try
                    {
                        sum += scaledMap[y + hs, x];
                        denominator++;
                    }
                    catch (Exception) { }
                    try
                    {
                        sum += scaledMap[y - hs, x];
                        denominator++;
                    }
                    catch (Exception) { }
                    try
                    {
                        sum += scaledMap[y, x + hs];
                        denominator++;
                    }
                    catch (Exception) { }
                    try
                    {
                        sum += scaledMap[y, x - hs];
                        denominator++;
                    }
                    catch (Exception) { }

                    //The result is sum average
                    scaledMap[y, x] = (float)(sum / denominator);
                }
            }
        }

        public void ScaleMap()
        {
            //This will be the Diamond-Square algorithm
            //That means the maps will be square with the sides' size of 2^power + 1

            //First - prepare the map for scaling (it will be filled with 0.0 upon init, we init the values here)
            int power = (scaledSize - 1) / (height - 1);
            if (power <= 1)  //no point in scaling if the power is 1 or less
                return;
            PreDSAInit(power);
            //And now - the D-SA
            int hs;
            for (int iter = power; iter > 1; iter /= 2)
            {
                hs = iter / 2;
                //Square step
                SquareStep(hs, iter);
                //Diamond step
                DiamondStep(hs, iter);
            }
        }

        public void MapValueMap(int[,] intMap, float newMin, float newMax)
        {
            float oldMin = -1;
            float oldMax = 1;
            float oldRange = oldMax - oldMin;  //In this library, it is strict
            float newRange = newMax - newMin;
            for (int i = 0; i < scaledSize; i++)
            {
                for (int j = 0; j < scaledSize; j++)
                {
                    intMap[i, j] = (int)((((scaledMap[i, j] - oldMin) * newRange) / oldRange) + newMin);
                }
            }
        }

        public void OtherMapValues(int[,] intMap, float newMin, float newMax)
        {
            float oldMin = -1;
            float oldMid = 0;
            float oldMax = 1;
            float oldRangeUp = oldMax - oldMid;
            float oldRangeDown = oldMid - oldMin;
            //ASSUME newMax is positive, newMin is negative
            //NEEDED add mew Mid as well. It can be BOTH positive and negative!
            //On second thought, do I really need it?
            float newRangeUp = newMax;
            float newRangeDown = Math.Abs(newMin);

            for (int i = 0; i < scaledSize; i++)
            {
                for (int j = 0; j < scaledSize; j++)
                {
                    if (scaledMap[i, j] < 0)
                        intMap[i, j] = (int)((scaledMap[i, j] * newRangeDown) / oldRangeDown);
                    else
                        intMap[i, j] = (int)((scaledMap[i, j] * newRangeUp) / oldRangeUp);
                }
            }
        }

        public Bitmap MakeBitmapColor(int[,] map, HeightReader heightReader)
        {
            Dictionary<int, HeightObject> heightChart = heightReader.getHeightChart();
            Bitmap image = new Bitmap(scaledSize, scaledSize);
            for (int i = 0; i < scaledSize; i++)
            {
                for (int j = 0; j < scaledSize; j++)
                {
                    for (int h = 0; h < heightChart.Count; h++)
                    {
                        int temp = map[i, j];
                        if (map[i, j] > heightChart.ElementAt(h).Key)
                        {
                            image.SetPixel(i, j, heightChart.ElementAt(h).Value.Color);
                            break;
                        }
                    }
                }
            }
            return image;
        }

        public Bitmap MakeBitmapBW(int[,] map)
        {
            Bitmap image = new Bitmap(width, height);
            MapValueMap(map, 0, 255);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    image.SetPixel(i, j, Color.FromArgb(map[i, j], map[i, j], map[i, j]));
                }
            }
            return image;
        }

        public void MakeMapParts(int[,] intMap, MapPart[,] mapParts)
        {
            for (int i = 0; i < scaledSize; i++)
            {
                for (int j = 0; j < scaledSize; j++)
                {
                    mapParts[i, j].Height = intMap[i, j];
                }
            }
        }

        public void OutputHeightsAsNumbers(MapPart[,] mapParts, string path)
        {
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                for (int i = 0; i < scaledSize; i++)
                {
                    for (int j = 0; j < scaledSize; j++)
                    {
                        string word = mapParts[i, j].Height.ToString().PadRight(7);
                        outputFile.Write(word);
                    }
                    outputFile.Write("\n");
                }
            }
        }

        private float UseGaussFilter(float[,] filter, int x, int y)
        {
            float sum = 0.0F;
            int denominator = 0;

            int filterX = 0;
            int filterY = 0;
            for (int i = y - 3; i <= y + 3; i++)
            {
                for (int j = x - 3; j <= x + 3; j++)
                {
                    if (i != y || j != x)
                    {
                        try
                        {
                            float temp = scaledMap[i, j] * filter[filterY, filterX];
                            sum += temp;
                            denominator++;
                            filterX++;
                        }
                        catch (Exception)
                        {
                            filterX++;
                        }
                    }
                }
                filterX = 0;
                filterY++;
            }
            return sum;
        }

        public float GaussBlur(int x, int y)
        {
            float[,] filter = new float[7, 7]
            {
                {0.00F, 0.00F, 0.00F, 0.00F, 0.00F, 0.00F, 0.00F},
                {0.00F, 0.00F, 0.01F, 0.01F, 0.01F, 0.00F, 0.00F},
                {0.00F, 0.01F, 0.05F, 0.11F, 0.05F, 0.01F, 0.00F},
                {0.00F, 0.01F, 0.11F, 0.25F, 0.11F, 0.01F, 0.00F},
                {0.00F, 0.01F, 0.05F, 0.11F, 0.05F, 0.01F, 0.00F},
                {0.00F, 0.00F, 0.01F, 0.01F, 0.01F, 0.00F, 0.00F},
                {0.00F, 0.00F, 0.00F, 0.00F, 0.00F, 0.00F, 0.00F}
            };

            return UseGaussFilter(filter, x, y);
        }

        public float[,] Balance()
        {
            float[,] balancedMap = new float[scaledSize, scaledSize];
            for (int i = 0; i < scaledSize; i++)
            {
                for (int j = 0; j < scaledSize; j++)
                {
                    balancedMap[i, j] = GaussBlur(i, j);
                }
            }
            return balancedMap;
        }

        public void BalanceDirectly()
        {
            for (int i = 0; i < scaledSize; i++)
            {
                for (int j = 0; j < scaledSize; j++)
                {
                    scaledMap[i, j] = GaussBlur(i, j);
                }
            }
        }

        public Bitmap RunGenerationBW(int[,] intMap, float min, float max, double power)
        {
            RunNoise();
            WeighValueMap(power);
            MapValueMap(intMap, min, max);
            return MakeBitmapBW(intMap);
        }

        public Bitmap RunGenerationColorNoWeightsScaleOther(int[,] intMap, HeightReader heightReader,
            float min, float max)
        {
            RunNoise();
            ScaleMap();
            OtherMapValues(intMap, min, max);
            return MakeBitmapColor(intMap, heightReader);
        }

        public Bitmap RunGenerationColorNoWeightsScaleBalance(int[,] intMap, HeightReader heightReader,
            float min, float max)
        {
            RunNoise();
            BalanceDirectly();
            ScaleMap();
            OtherMapValues(intMap, min, max);
            return MakeBitmapColor(intMap, heightReader);
        }

        public Bitmap RunGenerationBWNoWeightsScaleBalance(int[,] intMap, float min, float max)
        {
            RunNoise();
            BalanceDirectly();
            ScaleMap();
            OtherMapValues(intMap, min, max);
            return MakeBitmapBW(intMap);
        }

        //public Bitmap runGenerationColorNoWeightScaleWrap(int[,] intMap, HeightReader heightReader,
        //    float min, float max)
        //{
        //    RunNoiseWithWrapping();
        //    ScaleMap();
        //    OtherMapValues(intMap, min, max);
        //    return MakeBitmapColor(intMap, heightReader);
        //}
    }
}
