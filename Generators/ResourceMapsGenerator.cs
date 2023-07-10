using System.Drawing;
using MapGeneration.Utility;
using MapGeneration.Readers;

namespace MapGeneration.Generators
{
    public class ResourceMapsGenerator
    {
        private int size;
        private MapPart[,] mapParts;
        private List<BasicResource> resources;
        private Random rand;
        private int changeStateLimit = 5;
        private int iterationsNumber = 5;

        public ResourceMapsGenerator(MapPart[,] mapParts, int size, ResourceReader resourceReader)
        {
            this.size = size;
            this.mapParts = mapParts;
            resources = resourceReader.getResources();
            rand = new Random();
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

        private Dictionary<BasicResource, int> GetNeighborResources(int x, int y, string Categ)
        {
            Dictionary<BasicResource, int> neighborCount = new Dictionary<BasicResource, int>();
            //I need to specify all the values, including the border ones
            for (int i = y - 1; i <= y + 1; i++)
            {
                for (int j = x - 1; j <= x + 1; j++)
                {
                    if (i != y || j != x)
                    {
                        try  //This block is here for simplifying. The OutOfBorders cells will be skipped
                        {
                            BasicResource resource = mapParts[i, j].Resources.Where(r => r.Category.Equals(Categ)).First() as BasicResource;
                            if (resource != null && !neighborCount.ContainsKey(resource))
                                neighborCount.Add(resource, 0);
                            neighborCount[resource] += 1;
                        }
                        catch (Exception) { }
                    }
                }
            }
            return neighborCount;
        }

        private void RemoveNotFitByBiome(Dictionary<BasicResource, int> neighborCount, int x, int y)
        {
            for (int i = 0; i < neighborCount.Count; i++)
            {
                if (!mapParts[y, x].Biome.getResources().Contains(neighborCount.ElementAt(i).Key))
                {
                    neighborCount.Remove(neighborCount.ElementAt(i).Key);
                    i--;
                    if (neighborCount.Count == 0)
                        break;
                    if (i < 0 && neighborCount.Count != 0)
                        i = -1;
                }
            }
        }

        private List<IResource> GetCandidatesFromAquired(Dictionary<BasicResource, int> neighborCount)
        {
            List<IResource> states = new List<IResource>();
            int stateVal = 0;
            for (int i = 0; i < neighborCount.Count; i++)
            {
                if (neighborCount.ElementAt(i).Value >= changeStateLimit)
                {
                    states.Clear();
                    states.Add(neighborCount.ElementAt(i).Key);
                    break;
                }
                else if (neighborCount.ElementAt(i).Value > stateVal)
                {
                    stateVal = neighborCount.ElementAt(i).Value;
                    states.Clear();
                    states.Add(neighborCount.ElementAt(i).Key);
                }
                else if (neighborCount.ElementAt(i).Value == stateVal)
                {
                    states.Add(neighborCount.ElementAt(i).Key);
                }
            }
            return states;
        }

        private IResource PutResource(int x, int y, string Categ)
        {
            Dictionary<BasicResource, int> neighborCount = GetNeighborResources(x, y, Categ);

            //We also need to get rid of resources that can't fit the current cell by biome
            RemoveNotFitByBiome(neighborCount, x, y);

            //And now we choose the candidate from the ones that remain
            List<IResource> states = GetCandidatesFromAquired(neighborCount);

            //And return it, giving a bit of randomness
            if (states.Count == 0)
                return mapParts[y, x].Resources.FirstOrDefault(r => r.Category.Equals(Categ));
            else
                return states.ElementAt(rand.Next(states.Count));
        }

        private void BalanceOneIter(string Categ)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    IResource currentRes = mapParts[y, x].Resources.FirstOrDefault(x => x.Category.Equals(Categ));
                    IResource newRes = PutResource(x, y, Categ);
                    if (newRes != null)  //Avoiding null values here, they must not be added
                        mapParts[y, x].Resources.Add(newRes);
                    if (currentRes != null)
                    {
                        //I need to avoid duplicates by category too
                        mapParts[y, x].Resources.Remove(currentRes);
                    }
                }
            }
        }

        private void Balance(string Categ)
        {
            for (int i = 0; i < iterationsNumber; i++)
            {
                BalanceOneIter(Categ);
            }
        }

        private float[,] MakeMap()
        {
            FastNoiseLite noise = new FastNoiseLite(new Random().Next());
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(5);
            float[,] map = new float[size, size];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++)
                {
                    map[i, j] = noise.GetNoise(i, j);
                }
            }
            return map;
        }

        private List<float[,]> MakeResourceMaps(List<BasicResource> resCandidates)
        {
            List<float[,]> mapsForElements = new List<float[,]>();  //All map elements are first kept separately
            for (int i = 0; i < resCandidates.Count; i++)
            {
                mapsForElements.Add(MakeMap());  //Add the element maps to list
            }
            return mapsForElements;
        }

        private void AddMissingWeights(List<float> coefs, List<BasicResource> resCandidates)
        {
            if (coefs.Count < resCandidates.Count)
            {
                int difference = resCandidates.Count - coefs.Count;
                for (int k = 0; k < difference; k++)
                    coefs.Add(0.0F);  //This will be a default value.
                                      //Half will be thrown away immediately as noise goes from -1 to 1
            }
        }

        private List<int> FindElementsToPutInASpot(List<float[,]> mapsForElements, List<float> coefs, int i, int j)
        {
            List<int> maxElementIndexes = new List<int>();
            float maxElementWeight = -1.1F;
            for (int k = 0; k < mapsForElements.Count; k++)
            {
                //The element must have enough weight to be included
                if (mapsForElements.ElementAt(k)[i, j] >= coefs.ElementAt(k))  //If it does...
                {
                    //Check if the element has more weight then the ones checked previously
                    if (mapsForElements.ElementAt(k)[i, j] > maxElementWeight)
                    {
                        maxElementWeight = mapsForElements.ElementAt(k)[i, j];
                        maxElementIndexes.Clear();
                        maxElementIndexes.Add(k);
                    }
                    else if (mapsForElements.ElementAt(k)[i, j] == maxElementWeight)
                    {
                        maxElementIndexes.Add(k);
                    }
                }
            }
            return maxElementIndexes;
        }

        private void CreateMapForCategory(string Categ, List<float> coefs)
        {
            List<BasicResource> resCandidates = resources.Where(r => r.Category.Equals(Categ)).ToList();
            List<float[,]> mapsForElements = MakeResourceMaps(resCandidates);

            //Now we need to find what element to put on each spot
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    AddMissingWeights(coefs, resCandidates);
                    List<int> maxElementIndexes = FindElementsToPutInASpot(mapsForElements, coefs, i, j);
                    if (maxElementIndexes.Count > 0)
                    {
                        mapParts[i, j].Resources.Add(resCandidates.ElementAt(maxElementIndexes.ElementAt(rand.Next(maxElementIndexes.Count))));
                    }
                }
            }
        }

        private void FillWithPartialRandom(string Categ, List<float> coefs)
        {
            if (!Categ.Equals("O"))
            {
                //Coefs must NOT get here when they're NULL
                if (coefs != null)
                    CreateMapForCategory(Categ, coefs);
                else
                    throw new Exception("Weight coefficients for underground resources must not be Null!");
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        List<BasicResource> resCandidates = mapParts[i, j].Biome.getResources().Where(r => r.Category.Equals(Categ)).ToList();
                        if (resCandidates.Count > 0)
                            mapParts[i, j].Resources.Add(resCandidates.ElementAt(rand.Next(resCandidates.Count)));
                    }
                }
            }
        }

        public Bitmap MakePicture(string Categ)
        {
            Bitmap image = new Bitmap(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    BasicResource basicResource = mapParts[i, j].Resources.FirstOrDefault(r => r.Category.Equals(Categ)) as BasicResource;
                    if (basicResource != null)
                        image.SetPixel(i, j, basicResource.getColor());
                    else
                        image.SetPixel(i, j, Color.White);
                }
            }
            return image;
        }

        public void GenerateGlobalMap()
        {
            string Categ = "G";
            List<BasicResource> globalResources = resources.Where(r => r.Category.Equals(Categ)).ToList();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    List<BasicResource> resources = globalResources.Intersect(mapParts[i, j].Biome.getResources()).ToList();
                    if (resources.Count() > 0)
                        mapParts[i, j].Resources.Add(resources.First());
                }
            }
        }

        public void GenerateOnGroundMap()
        {
            string Categ = "O";
            //List<IResource> onGroundResources = resources.Where(r => r.Category.Equals(Categ)).ToList();
            //I'll need to balance here. Maybe using the cellular automata again?  DONE
            FillWithPartialRandom(Categ, null);
            Balance(Categ);
        }

        public void GenerateConcreteUnderGroundMap(string Categ, List<float> coefs)
        {
            bool exists = resources.Where(r => r.Category.Equals(Categ)).Any();
            if (exists)
            {
                FillWithPartialRandom(Categ, coefs);
                //Balance(Categ);
            }
        }

        public void GenerateUnderGroundMap(List<float> coefs)
        {
            //The coefficients are the values for weighting the elements.
            //The greater the coef, the less is the chance for the element to be included,
            //But the greater is the chance for the element to stay selected
            List<string> categories = resources.Select(r => r.Category).Distinct().ToList();
            for (int i = 0; i < categories.Count; i++)
            {
                if (!categories.ElementAt(i).Equals("G") && !categories.ElementAt(i).Equals("O"))
                {
                    List<int> indexes = resources.Where(r => r.Category.Equals(categories.ElementAt(i))).Select(r => r.getID()).ToList();
                    List<float> categoryCoefs = indexes.Select(index => coefs[index]).ToList();
                    FillWithPartialRandom(categories.ElementAt(i), categoryCoefs);
                    //Balance(categories.ElementAt(i));
                }
            }
        }
    }
}
