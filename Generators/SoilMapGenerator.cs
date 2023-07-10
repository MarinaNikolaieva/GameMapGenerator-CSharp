using MapGeneration.Utility;
using System.Drawing;

namespace MapGeneration.Generators
{
    public class SoilMapGenerator
    {
        private Soil[,] soilMap;
        private int size;
        private Random rand;
        private int changeStateLimit = 5;
        private int iterationsNumber = 10;
        private MapPart[,] mapParts;

        public SoilMapGenerator(int size, MapPart[,] mapParts)
        {
            soilMap = new Soil[size, size];
            this.size = size;
            rand = new Random();
            this.mapParts = mapParts;
        }

        public void ChangeItersNum(int number)
        {
            if (number > 0)
                iterationsNumber = number;
        }

        public void ChangeStateShiftLimit(int number)
        {
            if (number > 0)
                changeStateLimit = number;
        }

        private Dictionary<Soil, int> FindNeighborSoils(int x, int y)
        {
            Dictionary<Soil, int> neighborCount = new Dictionary<Soil, int>();
            //I need to specify all the values, including the border ones
            for (int i = y - 1; i <= y + 1; i++)
            {
                for (int j = x - 1; j <= x + 1; j++)
                {
                    if (i != y || j != x)
                    {
                        try  //This block is here for simplifying. The OutOfBorders cells will be skipped
                        {
                            if (!neighborCount.ContainsKey(soilMap[i, j]))
                                neighborCount.Add(soilMap[i, j], 0);
                            neighborCount[soilMap[i, j]] += 1;
                        }
                        catch (Exception) { }
                    }
                }
            }
            return neighborCount;
        }

        private void RemoveNotFitByBiome(Dictionary<Soil, int> neighborCount, int x, int y)
        {
            for (int i = 0; i < neighborCount.Count; i++)
            {
                if (!mapParts[y, x].Biome.getSoils().Contains(neighborCount.ElementAt(i).Key))
                {
                    neighborCount.Remove(neighborCount.ElementAt(i).Key);
                    i--;
                    if (i < 0)
                        i = 0;
                }
            }
        }

        private List<Soil> GetCandidatesFromAquired(Dictionary<Soil, int> neighbors)
        {
            List<Soil> states = new List<Soil>();
            int stateVal = 0;
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors.ElementAt(i).Value >= changeStateLimit)
                {
                    states.Clear();
                    states.Add(neighbors.ElementAt(i).Key);
                    break;
                }
                else if (neighbors.ElementAt(i).Value > stateVal)
                {
                    stateVal = neighbors.ElementAt(i).Value;
                    states.Clear();
                    states.Add(neighbors.ElementAt(i).Key);
                }
                else if (neighbors.ElementAt(i).Value == stateVal)
                {
                    states.Add(neighbors.ElementAt(i).Key);
                }
            }
            return states;
        }

        public Soil PutSoil(int x, int y)
        {
            Dictionary<Soil, int> neighborCount = FindNeighborSoils(x, y);

            //We also need to get rid of soils that can't fit the current cell by biome
            RemoveNotFitByBiome(neighborCount, x, y);

            //And now we choose the candidate from the ones that remain
            List<Soil> states = GetCandidatesFromAquired(neighborCount);

            //And return it, giving a bit of randomness
            if (states.Count == 0)
                return soilMap[y, x];
            else
                return states.ElementAt(rand.Next(states.Count));
        }

        private void BalanceOneIter()
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    soilMap[y, x] = PutSoil(x, y);
                }
            }
        }

        private void Balance()
        {
            for (int i = 0; i < iterationsNumber; i++)
            {
                BalanceOneIter();
            }
        }

        private void FillWithPartialRandom()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    List<Soil> soilCandidates = mapParts[i, j].Biome.getSoils();
                    soilMap[i, j] = soilCandidates.ElementAt(rand.Next(soilCandidates.Count));
                }
            }
        }

        private void PutSoilsToMapParts()
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    mapParts[i, j].Soil = soilMap[i, j];
        }

        private Bitmap MakePicture()
        {
            Bitmap image = new Bitmap(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    image.SetPixel(i, j, soilMap[i, j].getColor());
                }
            }
            return image;
        }

        public void RunGeneration()
        {
            FillWithPartialRandom();
            Balance();
            PutSoilsToMapParts();
        }

        public Bitmap GeneratePictureNoBalance()
        {
            FillWithPartialRandom();
            return MakePicture();
        }

        public Bitmap GetPicture()
        {
            return MakePicture();
        }
    }
}
