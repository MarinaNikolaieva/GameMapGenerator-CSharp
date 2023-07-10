using MapGeneration.Readers;
using MapGeneration.Utility;
using System.Drawing;

namespace MapGeneration.Generators
{
    public class HeightMapGenerator
    {
        private int maxIndex;  //size of one side of the map. The other will be the same. I may need to cut it. 2^n+1
        private int range;  //range from minimum to maximum height of the map
        private float entropy;  //resulting range of heights will be range * entropy
        private Random rand = new Random();

        public HeightMapGenerator(int power, int min, int max, float ent) 
        {
            maxIndex = (int)Math.Pow(2, power);
            range = Math.Abs(min) + Math.Abs(max);
            entropy = ent;
        }

        public void setEntropy(float newEntropy)
        {
            entropy = newEntropy;
        }

        public int getRandomValue()
        {
            return (int)(rand.Next(range + 1) * entropy);
        }

        public int getRandFromRange(int curRange)
        {
            return (int)((rand.NextDouble() * 2 - 1) * curRange * entropy);
        }

        public void init(int[,] map)  //start by initing the corners of the map
        {
            map[0, 0] = rand.Next() % (range + 1);
            map[0, maxIndex] = rand.Next() % (range + 1);
            map[maxIndex, 0] = rand.Next() % (range + 1);
            map[maxIndex, maxIndex] = rand.Next() % (range + 1);
        }

        public void runSquareDiamond(int[,] map)  //run the Square-Diamond algorithm
        {
            int hs;
            int x, y;
            int corner1, corner2, corner3, corner4;

            for (int iter = maxIndex; iter > 1; iter /= 2)  //iteration
            {
                hs = iter / 2;
                //Square step
                for (y = hs; y < maxIndex; y += iter)
                {
                    for (x = hs; x < maxIndex; x += iter)
                    {
                        // each square corner
                        corner1 = map[x - hs, y - hs];
                        corner2 = map[x - hs, y + hs];
                        corner3 = map[x + hs, y - hs];
                        corner4 = map[x + hs, y + hs];

                        // center point will be average plus a random in-range value
                        map[x, y] = ((corner1 + corner2 + corner3 + corner4) / 4) + getRandFromRange(hs);
                    }
                }
                //Diamond step
                for (y = 0; y <= maxIndex; y += hs)
                {
                    for (x = y % iter == 0 ? hs : 0; x <= maxIndex; x += iter)  // getting offset of x in function of y 
                    {
                        int sum = 0;
                        int denominator = 0;

                        //Calculate border points
                        try
                        {
                            sum += map[x + hs, y]; denominator++;
                        }
                        catch (Exception) { }
                        try
                        {
                            sum += map[x - hs, y]; denominator++;
                        }
                        catch (Exception) { }
                        try
                        {
                            sum += map[x, y + hs]; denominator++;
                        }
                        catch (Exception) { }
                        try
                        {
                            sum += map[x, y - hs]; denominator++;
                        }
                        catch (Exception) { }

                        // lets average sum plus random value
                        map[x, y] = sum / denominator + getRandFromRange(hs) / 2;
                    }
                }
            }
        }

        public Bitmap makeBitmap(int[,] map, HeightReader heightReader, int minValue)
        {
            Dictionary<int, HeightObject> heightChart = heightReader.getHeightChart();
            Bitmap image = new Bitmap(maxIndex + 1, maxIndex + 1);
            for (int i = 0; i <= maxIndex; i++)
            {
                for (int j = 0; j <= maxIndex; j++)
                {
                    int value = map[i, j] - Math.Abs(minValue);  //Convert the heights to the original system
                    for (int h = 0; h < heightChart.Count; h++)
                    {
                        if (value > heightChart.ElementAt(h).Key)
                        {
                            image.SetPixel(i, j, heightChart.ElementAt(h).Value.Color);
                            break;
                        }
                    }
                }
            }
            return image;
        }

        //I NEED to try another method, the Noise method
        public Bitmap runGeneration(int[,] map, HeightReader heightReader, int minValue)
        {
            init(map);
            runSquareDiamond(map);
            return makeBitmap(map, heightReader, minValue);
        }
    }
}
