using MapGeneration.Readers;
using MapGeneration.Utility;
using System.Drawing;

namespace MapGeneration.Generators
{
    public class BiomeMapGenerator
    {
        private Dictionary<int, List<Biome>> heightSpecificBiomes;
        private Biome[,] biomeMap;
        private int size;
        private Random rand;
        private int changeStateLimit = 5;
        private int iterationsNumber = 20;
        private MapPart[,] mapParts;

        public BiomeMapGenerator(BiomeReader biomeReader, int size, MapPart[,] mapParts)
        {
            heightSpecificBiomes = biomeReader.getHeightFixedBiomes();
            biomeMap = new Biome[size, size];
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

        private Dictionary<Biome, int> FindNeighborBiomes(int x, int y)
        {
            Dictionary<Biome, int> neighborCount = new Dictionary<Biome, int>();
            //I need to specify all the values, including the border ones
            for (int i = y - 1; i <= y + 1; i++)
            {
                for (int j = x - 1; j <= x + 1; j++)
                {
                    if (i != y || j != x)
                    {
                        try  //This block is here for simplifying. The OutOfBorders cells will be skipped
                        {
                            if (!neighborCount.ContainsKey(biomeMap[i, j]))
                                neighborCount.Add(biomeMap[i, j], 0);
                            neighborCount[biomeMap[i, j]] += 1;
                        }
                        catch (Exception) { }
                    }
                }
            }
            return neighborCount;
        }

        private void RemoveNotFitByHeight(Dictionary<Biome, int> neighbors, int x, int y)
        {
            if (!mapParts[y, x].NoHeightSpecific)  //If this is true, the part gets the value not-dependent on height
            {
                for (int h = 0; h < heightSpecificBiomes.Count; h++)
                {
                    if (mapParts[y, x].Height >= heightSpecificBiomes.ElementAt(h).Key)
                    {
                        for (int i = 0; i < neighbors.Count; i++)
                        {
                            if (!heightSpecificBiomes.ElementAt(h).Value.Contains(neighbors.ElementAt(i).Key))
                            {
                                neighbors.Remove(neighbors.ElementAt(i).Key);
                                i--;
                                if (i < 0)
                                    i = -1;
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void RemoveNotFitByTemperatureAndMoisture(Dictionary<Biome, int> neighbors, int x, int y)
        {
            for (int i = 0; i < neighbors.Count; i++)
            {
                bool fine = true;
                if (neighbors.ElementAt(i).Key.getTemperatures().Count() > 0)
                {
                    //...by temperature
                    if (!neighbors.ElementAt(i).Key.getTemperatures().Contains(mapParts[y, x].TemperatureBasic))
                    {
                        fine = false;
                    }
                }
                if (neighbors.ElementAt(i).Key.getMoistures().Count() > 0)
                {
                    //...and by moisture
                    if (!neighbors.ElementAt(i).Key.getMoistures().Contains(mapParts[y, x].Moisture))
                    {
                        fine = false;
                    }
                }
                if (!fine)
                {
                    neighbors.Remove(neighbors.ElementAt(i).Key);
                    i--;
                    if (i < 0)
                        i = -1;
                }
            }
        }

        private List<Biome> GetCandidatesFromAquired(Dictionary<Biome, int> neighbors)
        {
            List<Biome> states = new List<Biome>();
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

        //I'll use the cellular automata technique
        public Biome PutBiome(int x, int y)
        {
            Dictionary<Biome, int> neighborCount = FindNeighborBiomes(x, y);

            //We also need to get rid of biomes that can't fit the current cell...
            //...by height, temperature and moisture
            RemoveNotFitByHeight(neighborCount, x, y);

            RemoveNotFitByTemperatureAndMoisture(neighborCount, x, y);

            //And now we choose the candidate from the ones that remain
            List<Biome> states = GetCandidatesFromAquired(neighborCount);

            //And return it, giving a bit of randomness
            if (states.Count == 0)
                return biomeMap[y, x];
            else
                return states.ElementAt(rand.Next(states.Count));
        }

        private void BalanceOneIter()
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    biomeMap[y, x] = PutBiome(x, y);
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

        private void FilterByPatametersFirstInit(List<Biome> biomeCandidates, int i, int j)
        {
            for (int k = 0; k < biomeCandidates.Count; k++)
            {
                bool fine = true;
                if (biomeCandidates.ElementAt(k).getTemperatures().Count() > 0)
                {
                    if (!biomeCandidates.ElementAt(k).getTemperatures().Contains(mapParts[i, j].TemperatureBasic))
                    {
                        fine = false;
                    }
                }
                if (biomeCandidates.ElementAt(k).getMoistures().Count() > 0)
                {
                    if (!biomeCandidates.ElementAt(k).getMoistures().Contains(mapParts[i, j].Moisture))
                    {
                        fine = false;
                    }
                }
                if (!fine)
                {
                    biomeCandidates.Remove(biomeCandidates.ElementAt(k));
                    k--;
                    if (biomeCandidates.Count == 0)
                        break;
                }
            }
        }

        private void FilterByHeightFirstInit(List<Biome> biomeCandidates, ref bool firstDetect, ref bool traceback, int i, int j)
        {
            for (int h = 0; h < heightSpecificBiomes.Count; h++)
            {
                if (traceback)
                {
                    biomeCandidates.AddRange(heightSpecificBiomes.ElementAt(h).Value);
                }
                else if (!traceback && mapParts[i, j].Height >= heightSpecificBiomes.ElementAt(h).Key)
                {
                    biomeCandidates.AddRange(heightSpecificBiomes.ElementAt(h).Value);
                    firstDetect = true;
                }

                //Then, we specify the temperature and moisture if it's needed
                FilterByPatametersFirstInit(biomeCandidates, i, j);

                //And then we put the biome in place if there's something to place and break
                //If there are no candidates, make a step up
                if (biomeCandidates.Count > 0)
                {
                    biomeMap[i, j] = biomeCandidates.ElementAt(rand.Next(biomeCandidates.Count));
                    break;
                }
                else if (firstDetect)
                {
                    traceback = true;
                    mapParts[i, j].NoHeightSpecific = true;
                    h -= 2;
                }
            }
        }

        private void FillWithPartialRandom()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    List<Biome> biomeCandidates = new List<Biome>();
                    bool traceback = false;  //If this becomes true, we no longer check the height for the current cell
                    bool firstDetect = false;
                    //First, we specify the height
                    FilterByHeightFirstInit(biomeCandidates, ref firstDetect, ref traceback, i, j);
                }
            }
        }

        private void PutBiomesToMapParts()
        {
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    mapParts[i, j].Biome = biomeMap[i, j];
        }

        private Bitmap MakePicture()
        {
            Bitmap image = new Bitmap(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    image.SetPixel(i, j, biomeMap[i, j].getColor());
                }
            }
            return image;
        }

        public void RunGeneration()
        {
            FillWithPartialRandom();
            Balance();
            PutBiomesToMapParts();
        }

        public Bitmap GetPicture()
        {
            return MakePicture();
        }
    }
}
