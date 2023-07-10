using MapGeneration.Utility;
using MapGeneration.Readers;
using System.Numerics;
using System.Drawing;

namespace MapGeneration.Generators
{
    public class PoliticalMapGenerator
    {
        private List<CountryBase> Countries;
        private List<Vector2> Capitals;
        private MapPart[,] mapParts;
        private int size;
        private int maxDistance = 10;
        private int minHeightForCapital = 0;
        private int maxHeightForCapital = 3000;
        private int minHeightForExpantion = -1000;
        private Random rand;

        public PoliticalMapGenerator(CountryReader reader, MapPart[,] mapParts, int size)
        {
            Countries = reader.GetCountries();
            Capitals = new List<Vector2>();
            this.mapParts = mapParts;
            this.size = size;
            rand = new Random();
        }

        public void ChangeMaxDistance(int dist)
        {
            if (dist > 0)
                maxDistance = 10;
        }

        public void ChangeMinCapitalHeight(int height)
        {
            minHeightForCapital = height;
        }

        public void ChangeMaxCapitalHeight(int height)
        {
            maxHeightForCapital = height;
        }

        public void ChangeMinExpantionHeight(int height)
        {
            minHeightForExpantion = height;
        }

        private double GetDistanceBetweenPoints(Vector2 start, Vector2 end)
        {
            double distance = Math.Pow(end.X - start.X, 2.0) + Math.Pow(end.Y - start.Y, 2.0);
            distance = Math.Sqrt(distance);
            return distance;
        }

        private bool CheckCapitalPresence(int candX, int candY, List<Vector2> confirmedCountries)
        {
            for (int i = 0; i < confirmedCountries.Count; i++)
            {
                double distance = GetDistanceBetweenPoints(confirmedCountries.ElementAt(i), new Vector2(candX, candY));
                if (distance <= maxDistance)
                    return true;
            }
            return false;
        }

        private List<Vector2> PutCapitals()
        {
            List<Vector2> confirmedCountries = new List<Vector2>();

            for (int i = 0; i < Countries.Count; i++)
            {
                while (true)
                {
                    int x = rand.Next(size);
                    int y = rand.Next(size);
                    if (mapParts[y, x].Height <= maxHeightForCapital && mapParts[y, x].Height >= minHeightForCapital)
                    {
                        if (!CheckCapitalPresence(x, y, confirmedCountries))
                        {
                            confirmedCountries.Add(new Vector2(x, y));
                            break;
                        }
                    }
                }
            }

            return confirmedCountries;
        }

        private void SetCountriesToCapitals()
        {
            for (int i = 0; i < Countries.Count; i++)
            {
                mapParts[(int)Capitals.ElementAt(i).Y, (int)Capitals.ElementAt(i).X].Country = Countries.ElementAt(i);
                Countries.ElementAt(i).Capital = new Vector2((int)Capitals.ElementAt(i).X, (int)Capitals.ElementAt(i).Y);
            }
        }

        private void CheckAndExpand(MapPart curPart, CountryBase country, int i, List<Vector2> edgeParts)
        {
            if (curPart.neighbors.ElementAt(i).Height > minHeightForExpantion)
            {
                curPart.neighbors.ElementAt(i).Country = country;
                edgeParts.Add(new Vector2(curPart.neighbors.ElementAt(i).X, curPart.neighbors.ElementAt(i).Y));
            }
        }

        private void FillCapitalSurrounding(object locker, List<Vector2> edgeParts, CountryBase country)
        {
            while (edgeParts.Count > 0)
            {
                Vector2 curCoords = edgeParts.ElementAt(rand.Next(edgeParts.Count));
                MapPart curPart = mapParts[(int)curCoords.Y, (int)curCoords.X];
                lock (locker)
                {
                    for (int i = 0; i < curPart.neighbors.Count; i++)
                        if (curPart.neighbors.ElementAt(i).Country == null)
                        {
                            double distance = GetDistanceBetweenPoints(curCoords, new Vector2(curPart.neighbors.ElementAt(i).X, curPart.neighbors.ElementAt(i).Y));
                            if (distance <= maxDistance)
                            {
                                CheckAndExpand(curPart, country, i, edgeParts);
                            }
                        }
                }
                edgeParts.Remove(curCoords);
            }
        }

        private void FillTerritory(object locker, List<Vector2> edgeParts, CountryBase country)
        {
            while (edgeParts.Count > 0)
            {
                Vector2 curCoords = edgeParts.ElementAt(rand.Next(edgeParts.Count));
                MapPart curPart = mapParts[(int)curCoords.Y, (int)curCoords.X];
                lock (locker)
                {
                    for (int i = 0; i < curPart.neighbors.Count; i++)
                        if (curPart.neighbors.ElementAt(i).Country == null)
                        {
                            CheckAndExpand(curPart, country, i, edgeParts);
                        }
                }
                edgeParts.Remove(curCoords);
            }
        }

        //This method must be run in parallel, a thread for each country
        private void SpreadOneCountry(CountryBase country, Vector2 origin)
        {
            //The origin already has its country added
            object locker = new object();  //This is the thing I'll use for locking the map

            List<Vector2> edgeParts = new List<Vector2>{ origin };  //These are the parts able to spread

            //First thing - fill the circle around the Origin (capital)
            FillCapitalSurrounding(locker, edgeParts, country);

            //And then spread while possible
            FillTerritory(locker, edgeParts, country);
        }

        //One more method - to check if there are parts of land without the countries set
        private void CheckEmptyLand()
        {
            object locker = new object();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (mapParts[i, j].Height > minHeightForCapital && mapParts[i, j].Country == null)
                    {
                        //If such part exists, select a random country
                        CountryBase country = Countries.ElementAt(rand.Next(Countries.Count));
                        //Lock the map just in case
                        lock (locker) {
                            //And spread on the new territory
                            mapParts[i, j].Country = country;
                            SpreadOneCountry(country, new Vector2(j, i));
                        }
                    }
                }
            }
        }

        //And now the method to start the threads and make a map
        public void RunGeneration()
        {
            Capitals = PutCapitals();
            SetCountriesToCapitals();
            if (Countries.Count == 1)
                SpreadOneCountry(Countries.ElementAt(0), Capitals.ElementAt(0));
            else
            {
                List<Thread> threads = new List<Thread>();
                foreach (var country in Countries)
                {
                    threads.Add(new Thread(() => SpreadOneCountry(country, country.Capital)));
                }
                for (int i = 0; i < threads.Count; i++)
                    threads.ElementAt(i).Start();
                for (int i = 0; i < threads.Count; i++)
                    threads.ElementAt(i).Join();
            }
            CheckEmptyLand();
        }

        public Bitmap MakePicture()
        {
            Bitmap image = new Bitmap(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    CountryBase country = mapParts[i, j].Country;
                    if (country != null && Capitals.Contains(new Vector2(j, i)))
                        image.SetPixel(i, j, Color.Black);
                    else if (country != null)
                        image.SetPixel(i, j, country.Color);
                    else
                        image.SetPixel(i, j, Color.White);  //White color = neutral
                }
            }
            return image;
        }
    }
}
