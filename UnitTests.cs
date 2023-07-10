using MapGeneration.Generators;
using MapGeneration.Readers;
using MapGeneration.Utility;
using Newtonsoft.Json;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace UnitTests
{
    [TestClass]
    public class HeightGenTest
    {
        string folderPath = "C:\\Users\\Marina Nik\\Documents\\Visual Studio projects\\Diploma Parts - D.E.W\\Data Files\\Map Generator";
        [TestMethod]
        public void HeightReadTxtTest()
        {
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsTxt();
            Dictionary<int, HeightObject> heights = heightReader.getHeightChart();
            Assert.AreEqual(12, heights.Count);
        }

        [TestMethod]
        public void HeightReadExcelTest()
        {
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            Dictionary<int, HeightObject> heights = heightReader.getHeightChart();
            Assert.AreEqual(12, heights.Count);
        }

        [TestMethod]
        public void HeightGenerationTest()
        {
            int minVal = -15000;
            int maxVal = 10000;
            int power = 12;
            float entropy = 1.0F;

            int size = (int)Math.Pow(2, power) + 1;
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsTxt();

            int[,] map = new int[size, size];
            HeightMapGenerator heightMapGenerator = new HeightMapGenerator(power, minVal, maxVal, entropy);
            Bitmap image = heightMapGenerator.runGeneration(map, heightReader, minVal);
            image.Save(folderPath + "\\HeightImage.png");

            Assert.IsNotNull(image);
        }

        [TestMethod]
        public void HeightGenerationNoiseBWTest()
        {
            int minVal = 0;
            int maxVal = 255;

            int[,] map = new int[500, 500];
            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(500, 500, 500);
            heightMapGenerator.ChangeOctaves(7);
            Bitmap image = heightMapGenerator.RunGenerationBW(map, minVal, maxVal, 1.0);
            image.Save(folderPath + "\\HeightNoiseImage7Octaves.png");

            Assert.IsNotNull(image);
        }

        [TestMethod]
        public void HeightGenerationNoiseSizeMapDifferTest()
        {
            int minVal = -15000;
            int maxVal = 10000;
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsTxt();
            int height = 257;
            int width = 257;
            int scale = 513;

            int[,] map = new int[scale, scale];
            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(width, height, scale);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleOther(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HeightNoiseImage5OctSize257Scale513MapDiffer.png");

            Assert.IsNotNull(image);
        }
    }

    [TestClass]
    public class TemperatureGenTest
    {
        string folderPath = "C:\\Users\\Marina Nik\\Documents\\Visual Studio projects\\Diploma Parts - D.E.W\\Data Files\\Map Generator";

        [TestMethod]
        public void TemperatureGenerTest()
        {
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();

            int size = 513;
            float minMapVal = 0.65F;
            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            TemperatureMapGenerator generator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator, minMapVal);
            Bitmap image = generator.RunGeneration();
            image.Save(folderPath + "\\TemperatureGradientMultNoiseMap065.png");

            Assert.IsNotNull(image);
        }

        [TestMethod]
        public void TemperatureWithHeightGenerTest()
        {
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();

            int size = 513;
            int smallSize = 257;
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.65F;
            float maxTemperCoef = 0.55F;

            int[,] map = new int[size, size];
            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleOther(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HeightAndTemperH10.png");

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            TemperatureMapGenerator generator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator, 
                minMapVal, heightReader, map, minVal, range, maxTemperCoef);
            Bitmap temperImage = generator.RunGeneration();
            temperImage.Save(folderPath + "\\HeightAndTemperT10.png");

            Assert.IsNotNull(image);
        }
    }

    [TestClass]
    public class MoistureGenTest
    {
        string folderPath = "C:\\Users\\Marina Nik\\Documents\\Visual Studio projects\\Diploma Parts - D.E.W\\Data Files\\Map Generator";

        [TestMethod]
        public void MoistureGenerTest()
        {
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();

            int size = 513;
            float minMapVal = 0.75F;
            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            MoistureMapGenerator generator = new MoistureMapGenerator(size, moistureReader, temperMoistPartialGenerator, minMapVal);
            Bitmap image = generator.RunGeneration();
            image.Save(folderPath + "\\MoistureGradientMap075New.png");

            Assert.IsNotNull(image);
        }

        [TestMethod]
        public void MoistureWithHeightGenerTest()
        {
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();

            int size = 513;
            int smallSize = 257;
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.75F;
            float maxMoistCoef = 0.5F;

            int[,] map = new int[size, size];
            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleOther(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HeightAndMoistH4.png");

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            MoistureMapGenerator generator = new MoistureMapGenerator(size, moistureReader, temperMoistPartialGenerator, minMapVal, 
                heightReader, map, minVal, range, maxMoistCoef);
            Bitmap temperImage = generator.RunGeneration();
            temperImage.Save(folderPath + "\\HeightAndMoistM4.png");

            Assert.IsNotNull(image);
        }
    }

    [TestClass]
    public class ComplexGenTest
    {
        string folderPath = "C:\\Users\\Marina Nik\\Documents\\Visual Studio projects\\Diploma Parts - D.E.W\\Data Files\\Map Generator";

        [TestMethod]
        public void TemperatureSeparateFromHeightGenTest()
        {
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();

            int size = 513;
            int smallSize = 257;
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.75F;
            float maxMoistCoef = 0.5F;
            float maxTemperCoef = 0.55F;

            int[,] map = new int[size, size];
            MapPart[,] complexMap = new MapPart[size, size];
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;
                }
            }

            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleOther(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HeightTemperSeparateH1.png");
            heightMapGenerator.MakeMapParts(map, complexMap);

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);

            TemperatureMapGenerator temperMapGenerator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator,
                minMapVal);
            temperMapGenerator.MakeMapPartsBasic(complexMap);
            Bitmap temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HeightTemperSeparateTB1.png");
            temperMapGenerator.commonGenerator.RunGenerWithHeightsFromPrior(heightReader, map, minVal, range, maxTemperCoef, 0);
            temperMapGenerator.MakeMapPartsFinal(complexMap);
            temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HeightTemperSeparateTF1.png");

            Assert.IsNotNull(temperImage);
        }

        [TestMethod]
        public void HeightGenerNumbersTest()
        {
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            int size = 513;
            int smallSize = 257;
            int minVal = -15000;
            int maxVal = 10000;

            int[,] map = new int[size, size];
            MapPart[,] complexMap = new MapPart[size, size];
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;
                }
            }
            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleOther(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HeightNumberGen.png");
            heightMapGenerator.MakeMapParts(map, complexMap);
            heightMapGenerator.OutputHeightsAsNumbers(complexMap, folderPath + "\\Numbers.txt");
        }

        [TestMethod]
        public void HeightTemperMoistureGenerTest()
        {
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx(); 
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();

            int size = 513;
            int smallSize = 257;
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.75F;
            float maxMoistCoef = 0.5F;
            float maxTemperCoef = 0.55F;

            int[,] map = new int[size, size];
            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleOther(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HeightTemperMoistH1.png");

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            MoistureMapGenerator moistureMapGenerator = new MoistureMapGenerator(size, moistureReader, temperMoistPartialGenerator, minMapVal,
                heightReader, map, minVal, range, maxMoistCoef);
            Bitmap moistImage = moistureMapGenerator.RunGeneration();
            moistImage.Save(folderPath + "\\HeightTemperMoistM1.png");

            TemperatureMapGenerator temperMapGenerator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator,
                minMapVal, heightReader, map, minVal, range, maxTemperCoef);
            Bitmap temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HeightTemperMoistT1.png");

            Assert.IsNotNull(image);
        }
    }

    [TestClass]
    public class BiomeGenTest
    {
        string folderPath = "C:\\Users\\Marina Nik\\Documents\\Visual Studio projects\\Diploma Parts - D.E.W\\Data Files\\Map Generator";

        [TestMethod]
        public void ResourceReadTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            List<BasicResource> resources = resourceReader.getResources();
            Assert.IsNotNull(resources);
        }

        [TestMethod]
        public void BiomeReadTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();

            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);
            List<Biome> biomes = biomeReader.getBiomes();
            Assert.IsNotNull(biomes);
        }

        [TestMethod]
        public void BiomeHeightReadTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();

            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);
            biomeReader.readBiomeHeightsXlsx(temperatureReader);
            List<Biome> biomes = biomeReader.getBiomes();
            Assert.IsNotNull(biomes);
        }

        [TestMethod]
        public void BiomeHeightTemperMoistReadTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();

            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);
            biomeReader.readBiomeHeightsXlsx(temperatureReader);
            biomeReader.readMoistTempersXlsx(temperatureReader, moistureReader);
            List<Biome> biomes = biomeReader.getBiomes();
            Assert.IsNotNull(biomes);
        }

        [TestMethod]
        public void BiomeGenerationTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();

            int size = 513;
            int smallSize = 257;
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.75F;
            float maxMoistCoef = 0.5F;
            float maxTemperCoef = 0.55F;

            int[,] map = new int[size, size];
            MapPart[,] complexMap = new MapPart[size, size];
            int counter = 0;
            for (int i =  0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;
                }
            }

            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleOther(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HeightTemperMoistBiomeH4.png");
            heightMapGenerator.MakeMapParts(map, complexMap);

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            MoistureMapGenerator moistureMapGenerator = new MoistureMapGenerator(size, moistureReader, temperMoistPartialGenerator, minMapVal,
                heightReader, map, minVal, range, maxMoistCoef);
            Bitmap moistImage = moistureMapGenerator.RunGeneration();
            moistImage.Save(folderPath + "\\HeightTemperMoistBiomeM4.png");
            moistureMapGenerator.MakeMapParts(complexMap);

            TemperatureMapGenerator temperMapGenerator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator,
                minMapVal, heightReader, map, minVal, range, maxTemperCoef);
            Bitmap temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HeightTemperMoistBiomeT4.png");
            temperMapGenerator.MakeMapPartsFinal(complexMap);

            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);
            biomeReader.readBiomeHeightsXlsx(temperatureReader);
            biomeReader.readMoistTempersXlsx(temperatureReader, moistureReader);

            BiomeMapGenerator biomeMapGenerator = new BiomeMapGenerator(biomeReader, size, complexMap);
            biomeMapGenerator.RunGeneration();
            Bitmap biomeImage = biomeMapGenerator.GetPicture();
            biomeImage.Save(folderPath + "\\HeightTemperMoistBiomeB4.png");
        }

        [TestMethod]
        public void BiomeGenHarderTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();

            int size = 513;
            int smallSize = 257;
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.75F;
            float maxMoistCoef = 0.5F;
            float maxTemperCoef = 0.55F;

            int[,] map = new int[size, size];
            MapPart[,] complexMap = new MapPart[size, size];
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;
                }
            }

            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleOther(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HeightTemperMoistBiomeH4.png");
            heightMapGenerator.MakeMapParts(map, complexMap);

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            MoistureMapGenerator moistureMapGenerator = new MoistureMapGenerator(size, moistureReader, temperMoistPartialGenerator, minMapVal,
                heightReader, map, minVal, range, maxMoistCoef);
            Bitmap moistImage = moistureMapGenerator.RunGeneration();
            moistImage.Save(folderPath + "\\HeightTemperMoistBiomeM4.png");
            moistureMapGenerator.MakeMapParts(complexMap);

            TemperatureMapGenerator temperMapGenerator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator,
                minMapVal);
            temperMapGenerator.MakeMapPartsBasic(complexMap);
            Bitmap temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HeightTemperMoistBiomeSeparateTB1.png");
            temperMapGenerator.commonGenerator.RunGenerWithHeightsFromPrior(heightReader, map, minVal, range, maxTemperCoef, 0);
            temperMapGenerator.MakeMapPartsFinal(complexMap);
            temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HeightTemperMoistBiomeSeparateTF1.png");

            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);
            biomeReader.readBiomeHeightsXlsx(temperatureReader);
            biomeReader.readMoistTempersXlsx(temperatureReader, moistureReader);

            BiomeMapGenerator biomeMapGenerator = new BiomeMapGenerator(biomeReader, size, complexMap);
            biomeMapGenerator.RunGeneration();
            Bitmap biomeImage = biomeMapGenerator.GetPicture();
            biomeImage.Save(folderPath + "\\HeightTemperMoistBiomeB4.png");
        }
    }

    [TestClass]
    public class SoilGenTest
    {
        string folderPath = "C:\\Users\\Marina Nik\\Documents\\Visual Studio projects\\Diploma Parts - D.E.W\\Data Files\\Map Generator";

        [TestMethod]
        public void SoilReaderTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);

            SoilReader soilReader = new SoilReader(folderPath);
            soilReader.readSoilsXlsx(biomeReader);
            List<Soil> soils = soilReader.getSoils();
            Assert.IsNotNull(soils);
        }

        [TestMethod]
        public void SoilFullGenerationTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);
            biomeReader.readBiomeHeightsXlsx(temperatureReader);
            biomeReader.readMoistTempersXlsx(temperatureReader, moistureReader);

            SoilReader soilReader = new SoilReader(folderPath);
            soilReader.readSoilsXlsx(biomeReader);

            int size = 1025;
            int smallSize = 257;
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.75F;
            float maxMoistCoef = 0.5F;
            float maxTemperCoef = 0.55F;

            int[,] map = new int[size, size];
            MapPart[,] complexMap = new MapPart[size, size];
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;
                }
            }

            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleOther(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HeightTemperMoistBiomeSoilH2.png");
            heightMapGenerator.MakeMapParts(map, complexMap);

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            MoistureMapGenerator moistureMapGenerator = new MoistureMapGenerator(size, moistureReader, temperMoistPartialGenerator, minMapVal,
                heightReader, map, minVal, range, maxMoistCoef);
            Bitmap moistImage = moistureMapGenerator.RunGeneration();
            moistImage.Save(folderPath + "\\HeightTemperMoistBiomeSoilM2.png");
            moistureMapGenerator.MakeMapParts(complexMap);

            TemperatureMapGenerator temperMapGenerator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator,
                minMapVal, heightReader, map, minVal, range, maxTemperCoef);
            Bitmap temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HeightTemperMoistBiomeSoilT2.png");
            temperMapGenerator.MakeMapPartsFinal(complexMap);

            BiomeMapGenerator biomeMapGenerator = new BiomeMapGenerator(biomeReader, size, complexMap);
            biomeMapGenerator.RunGeneration();
            Bitmap biomeImage = biomeMapGenerator.GetPicture();
            biomeImage.Save(folderPath + "\\HeightTemperMoistBiomeSoilB2.png");

            SoilMapGenerator soilMapGenerator = new SoilMapGenerator(size, complexMap);
            soilMapGenerator.RunGeneration();
            Bitmap soilImage = soilMapGenerator.GetPicture();
            soilImage.Save(folderPath + "\\HeightTemperMoistBiomeSoilS2.png");
        }

        [TestMethod]
        public void SoilHarderGenTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);
            biomeReader.readBiomeHeightsXlsx(temperatureReader);
            biomeReader.readMoistTempersXlsx(temperatureReader, moistureReader);
            SoilReader soilReader = new SoilReader(folderPath);
            soilReader.readSoilsXlsx(biomeReader);

            int size = 513;
            int smallSize = 257;
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.75F;
            float maxMoistCoef = 0.5F;
            float maxTemperCoef = 0.55F;

            int[,] map = new int[size, size];
            MapPart[,] complexMap = new MapPart[size, size];
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;
                }
            }

            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleOther(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HTMBSSeparate-H1.png");
            heightMapGenerator.MakeMapParts(map, complexMap);

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            MoistureMapGenerator moistureMapGenerator = new MoistureMapGenerator(size, moistureReader, temperMoistPartialGenerator, minMapVal,
                heightReader, map, minVal, range, maxMoistCoef);
            Bitmap moistImage = moistureMapGenerator.RunGeneration();
            moistImage.Save(folderPath + "\\HTMBSSeparate-M1.png");
            moistureMapGenerator.MakeMapParts(complexMap);

            TemperatureMapGenerator temperMapGenerator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator,
                minMapVal);
            temperMapGenerator.MakeMapPartsBasic(complexMap);
            Bitmap temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSSeparate-TB1.png");
            temperMapGenerator.commonGenerator.RunGenerWithHeightsFromPrior(heightReader, map, minVal, range, maxTemperCoef, 0);
            temperMapGenerator.MakeMapPartsFinal(complexMap);
            temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSSeparate-TF1.png");

            BiomeMapGenerator biomeMapGenerator = new BiomeMapGenerator(biomeReader, size, complexMap);
            biomeMapGenerator.ChangeItersNum(7);
            biomeMapGenerator.RunGeneration();
            Bitmap biomeImage = biomeMapGenerator.GetPicture();
            biomeImage.Save(folderPath + "\\HTMBSSeparate-B1.png");

            SoilMapGenerator soilMapGenerator = new SoilMapGenerator(size, complexMap);
            soilMapGenerator.ChangeItersNum(5);
            soilMapGenerator.RunGeneration();
            Bitmap soilImage = soilMapGenerator.GetPicture();
            soilImage.Save(folderPath + "\\HTMBSSeparate-S1.png");
        }
    }

    [TestClass]
    public class BalancingClass
    {
        string folderPath = "C:\\Users\\Marina Nik\\Documents\\Visual Studio projects\\Diploma Parts - D.E.W\\Data Files\\Map Generator";

        [TestMethod]
        public void BWBalanceTest()
        {
            int minVal = 0;
            int maxVal = 255;

            int[,] map = new int[513, 513];
            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(257, 257, 513);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationBWNoWeightsScaleBalance(map, minVal, maxVal);
            image.Save(folderPath + "\\HeightNoiseBalanceBW.png");

            Assert.IsNotNull(image);
        }

        [TestMethod]
        public void ColorBalanceTest()
        {
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();

            int size = 513;
            int smallSize = 257;
            int minVal = -15000;
            int maxVal = 10000;

            int[,] map = new int[size, size];
            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleBalance(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HeightNoiseBalanceColor.png");

            Assert.IsNotNull(image);
        }

        [TestMethod]
        public void FullGenerationWithBalance()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);
            biomeReader.readBiomeHeightsXlsx(temperatureReader);
            biomeReader.readMoistTempersXlsx(temperatureReader, moistureReader);
            SoilReader soilReader = new SoilReader(folderPath);
            soilReader.readSoilsXlsx(biomeReader);

            int size = 129;
            int smallSize = 65;
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.75F;
            float maxMoistCoef = 0.5F;
            float maxTemperCoef = 0.55F;

            int[,] map = new int[size, size];
            MapPart[,] complexMap = new MapPart[size, size];
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;
                }
            }

            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleBalance(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HTMBSSeparate-H1.png");
            heightMapGenerator.MakeMapParts(map, complexMap);

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            MoistureMapGenerator moistureMapGenerator = new MoistureMapGenerator(size, moistureReader, temperMoistPartialGenerator, minMapVal,
                heightReader, map, minVal, range, maxMoistCoef);
            Bitmap moistImage = moistureMapGenerator.RunGeneration();
            moistImage.Save(folderPath + "\\HTMBSSeparateBalance-M1.png");
            moistureMapGenerator.MakeMapParts(complexMap);

            TemperatureMapGenerator temperMapGenerator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator,
                minMapVal);
            temperMapGenerator.MakeMapPartsBasic(complexMap);
            Bitmap temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSSeparateBalance-TB1.png");
            temperMapGenerator.commonGenerator.RunGenerWithHeightsFromPrior(heightReader, map, minVal, range, maxTemperCoef, 0);
            temperMapGenerator.MakeMapPartsFinal(complexMap);
            temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSSeparateBalance-TF1.png");

            BiomeMapGenerator biomeMapGenerator = new BiomeMapGenerator(biomeReader, size, complexMap);
            biomeMapGenerator.ChangeItersNum(7);
            biomeMapGenerator.RunGeneration();
            Bitmap biomeImage = biomeMapGenerator.GetPicture();
            biomeImage.Save(folderPath + "\\HTMBSSeparateBalance-B1.png");

            SoilMapGenerator soilMapGenerator = new SoilMapGenerator(size, complexMap);
            soilMapGenerator.RunGeneration();
            Bitmap soilImage = soilMapGenerator.GetPicture();
            soilImage.Save(folderPath + "\\HTMBSSeparateBalance-S1.png");

            string outputPath = folderPath + "\\AllMapPartOutput.txt";
            using StreamWriter streamWriter = new StreamWriter(outputPath);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    string strJson = JsonConvert.SerializeObject(complexMap[i, j]);
                    streamWriter.WriteLine(strJson);
                }
            }
            Assert.IsNotNull(soilImage);
        }
    }

    [TestClass]
    public class ResourceGenClass
    {
        string folderPath = "C:\\Users\\Marina Nik\\Documents\\Visual Studio projects\\Diploma Parts - D.E.W\\Data Files\\Map Generator";

        [TestMethod]
        public void GlobalOnGroundGenTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);
            biomeReader.readBiomeHeightsXlsx(temperatureReader);
            biomeReader.readMoistTempersXlsx(temperatureReader, moistureReader);

            int size = 513;
            int smallSize = 257;
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.75F;
            float maxMoistCoef = 0.5F;
            float maxTemperCoef = 0.55F;

            int[,] map = new int[size, size];
            MapPart[,] complexMap = new MapPart[size, size];
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;
                }
            }

            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleBalance(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HTMBSR-H1.png");
            heightMapGenerator.MakeMapParts(map, complexMap);

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            MoistureMapGenerator moistureMapGenerator = new MoistureMapGenerator(size, moistureReader, temperMoistPartialGenerator, minMapVal,
                heightReader, map, minVal, range, maxMoistCoef);
            Bitmap moistImage = moistureMapGenerator.RunGeneration();
            moistImage.Save(folderPath + "\\HTMBSR-M1.png");
            moistureMapGenerator.MakeMapParts(complexMap);

            TemperatureMapGenerator temperMapGenerator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator,
                minMapVal);
            temperMapGenerator.MakeMapPartsBasic(complexMap);
            Bitmap temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSR-TB1.png");
            temperMapGenerator.commonGenerator.RunGenerWithHeightsFromPrior(heightReader, map, minVal, range, maxTemperCoef, 0);
            temperMapGenerator.MakeMapPartsFinal(complexMap);
            temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSR-TF1.png");

            BiomeMapGenerator biomeMapGenerator = new BiomeMapGenerator(biomeReader, size, complexMap);
            biomeMapGenerator.ChangeItersNum(7);
            biomeMapGenerator.RunGeneration();
            Bitmap biomeImage = biomeMapGenerator.GetPicture();
            biomeImage.Save(folderPath + "\\HTMBSR-B1.png");


            ResourceMapsGenerator resourceMapsGenerator = new ResourceMapsGenerator(complexMap, size, resourceReader);
            resourceMapsGenerator.GenerateGlobalMap();
            resourceMapsGenerator.GenerateOnGroundMap();
            Bitmap globalRes = resourceMapsGenerator.MakePicture("G");
            globalRes.Save(folderPath + "\\HTMBSR-RG1.png");
            Bitmap onGroundRes = resourceMapsGenerator.MakePicture("O");
            onGroundRes.Save(folderPath + "\\HTMBSR-RO1.png");
        }

        [TestMethod]
        public void OneUnderGroundGenTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);
            biomeReader.readBiomeHeightsXlsx(temperatureReader);
            biomeReader.readMoistTempersXlsx(temperatureReader, moistureReader);

            int size = 257;
            int smallSize = 129;
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.75F;
            float maxMoistCoef = 0.5F;
            float maxTemperCoef = 0.55F;

            int[,] map = new int[size, size];
            MapPart[,] complexMap = new MapPart[size, size];
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;
                }
            }

            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleBalance(map, heightReader, minVal, maxVal);
            //Bitmap image = heightMapGenerator.runGenerationColorNoWeightsScaleOther(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HTMBSR-H1.png");
            heightMapGenerator.MakeMapParts(map, complexMap);

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            MoistureMapGenerator moistureMapGenerator = new MoistureMapGenerator(size, moistureReader, temperMoistPartialGenerator, minMapVal,
                heightReader, map, minVal, range, maxMoistCoef);
            Bitmap moistImage = moistureMapGenerator.RunGeneration();
            moistImage.Save(folderPath + "\\HTMBSR-M1.png");
            moistureMapGenerator.MakeMapParts(complexMap);

            TemperatureMapGenerator temperMapGenerator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator,
                minMapVal);
            temperMapGenerator.MakeMapPartsBasic(complexMap);
            Bitmap temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSR-TB1.png");
            temperMapGenerator.commonGenerator.RunGenerWithHeightsFromPrior(heightReader, map, minVal, range, maxTemperCoef, 0);
            temperMapGenerator.MakeMapPartsFinal(complexMap);
            temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSR-TF1.png");

            BiomeMapGenerator biomeMapGenerator = new BiomeMapGenerator(biomeReader, size, complexMap);
            biomeMapGenerator.ChangeItersNum(5);
            biomeMapGenerator.RunGeneration();
            Bitmap biomeImage = biomeMapGenerator.GetPicture();
            biomeImage.Save(folderPath + "\\HTMBSR-B1.png");

            List<float> coefficients = new List<float>
            {
                0.35F,
                0.50F,
                0.65F
            };
            ResourceMapsGenerator resourceMapsGenerator = new ResourceMapsGenerator(complexMap, size, resourceReader);
            resourceMapsGenerator.GenerateConcreteUnderGroundMap("U3", coefficients);
            Bitmap globalRes = resourceMapsGenerator.MakePicture("U3");
            globalRes.Save(folderPath + "\\HTMBSR-RU3-1.png");
        }

        [TestMethod]
        public void FullUnderGroundGenTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);
            biomeReader.readBiomeHeightsXlsx(temperatureReader);
            biomeReader.readMoistTempersXlsx(temperatureReader, moistureReader);

            int size = 129;
            int smallSize = 65;
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.75F;
            float maxMoistCoef = 0.5F;
            float maxTemperCoef = 0.55F;

            int[,] map = new int[size, size];
            MapPart[,] complexMap = new MapPart[size, size];
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;
                }
            }

            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            //Bitmap image = heightMapGenerator.runGenerationColorNoWeightsScaleBalance(map, heightReader, minVal, maxVal);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleOther(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HTMBSR-H1.png");
            heightMapGenerator.MakeMapParts(map, complexMap);

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            MoistureMapGenerator moistureMapGenerator = new MoistureMapGenerator(size, moistureReader, temperMoistPartialGenerator, minMapVal,
                heightReader, map, minVal, range, maxMoistCoef);
            Bitmap moistImage = moistureMapGenerator.RunGeneration();
            moistImage.Save(folderPath + "\\HTMBSR-M1.png");
            moistureMapGenerator.MakeMapParts(complexMap);

            TemperatureMapGenerator temperMapGenerator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator,
                minMapVal);
            temperMapGenerator.MakeMapPartsBasic(complexMap);
            Bitmap temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSR-TB1.png");
            temperMapGenerator.commonGenerator.RunGenerWithHeightsFromPrior(heightReader, map, minVal, range, maxTemperCoef, 0);
            temperMapGenerator.MakeMapPartsFinal(complexMap);
            temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSR-TF1.png");

            BiomeMapGenerator biomeMapGenerator = new BiomeMapGenerator(biomeReader, size, complexMap);
            biomeMapGenerator.ChangeItersNum(7);
            biomeMapGenerator.RunGeneration();
            Bitmap biomeImage = biomeMapGenerator.GetPicture();
            biomeImage.Save(folderPath + "\\HTMBSR-B1.png");

            List<float> coefficients = new List<float>
            {
                0.0F,
                0.35F,
                0.3F,
                0.3F,
                0.35F,
                0.5F,
                0.65F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.7F,
                0.35F,
                0.45F,
                0.5F,
                0.5F,
                0.45F,
                0.45F
            };
            ResourceMapsGenerator resourceMapsGenerator = new ResourceMapsGenerator(complexMap, size, resourceReader);
            List<string> categories = resourceReader.getResources().DistinctBy(r => r.Category).Select(r => r.Category).ToList();
            resourceMapsGenerator.GenerateUnderGroundMap(coefficients);
            for (int i = 0; i < categories.Count - 2; i++)
            {
                Bitmap globalRes = resourceMapsGenerator.MakePicture(categories.ElementAt(i));
                globalRes.Save(folderPath + "\\HTMBSR-R" + categories.ElementAt(i) + "-1.png");
            }
        }

        [TestMethod]
        public void FullResourceGenTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);
            biomeReader.readBiomeHeightsXlsx(temperatureReader);
            biomeReader.readMoistTempersXlsx(temperatureReader, moistureReader);
            SoilReader soilReader = new SoilReader(folderPath);
            soilReader.readSoilsXlsx(biomeReader);

            int size = 513;
            int smallSize = 257;
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.75F;
            float maxMoistCoef = 0.5F;
            float maxTemperCoef = 0.55F;

            int[,] map = new int[size, size];
            MapPart[,] complexMap = new MapPart[size, size];
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;
                }
            }

            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleBalance(map, heightReader, minVal, maxVal);
            //Bitmap image = heightMapGenerator.runGenerationColorNoWeightsScaleOther(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HTMBSR-H1.png");
            heightMapGenerator.MakeMapParts(map, complexMap);

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            MoistureMapGenerator moistureMapGenerator = new MoistureMapGenerator(size, moistureReader, temperMoistPartialGenerator, minMapVal,
                heightReader, map, minVal, range, maxMoistCoef);
            Bitmap moistImage = moistureMapGenerator.RunGeneration();
            moistImage.Save(folderPath + "\\HTMBSR-M1.png");
            moistureMapGenerator.MakeMapParts(complexMap);

            TemperatureMapGenerator temperMapGenerator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator,
                minMapVal);
            temperMapGenerator.MakeMapPartsBasic(complexMap);
            Bitmap temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSR-TB1.png");
            temperMapGenerator.commonGenerator.RunGenerWithHeightsFromPrior(heightReader, map, minVal, range, maxTemperCoef, 0);
            temperMapGenerator.MakeMapPartsFinal(complexMap);
            temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSR-TF1.png");

            BiomeMapGenerator biomeMapGenerator = new BiomeMapGenerator(biomeReader, size, complexMap);
            biomeMapGenerator.ChangeItersNum(7);
            biomeMapGenerator.RunGeneration();
            Bitmap biomeImage = biomeMapGenerator.GetPicture();
            biomeImage.Save(folderPath + "\\HTMBSR-B1.png");

            SoilMapGenerator soilMapGenerator = new SoilMapGenerator(size, complexMap);
            soilMapGenerator.ChangeItersNum(5);
            soilMapGenerator.RunGeneration();
            Bitmap soilImage = soilMapGenerator.GetPicture();
            soilImage.Save(folderPath + "\\HTMBSR-S1.png");

            List<float> coefficients = new List<float>
            {
                0.0F,
                0.35F,
                0.3F,
                0.3F,
                0.35F,
                0.5F,
                0.65F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.7F,
                0.35F,
                0.45F,
                0.5F,
                0.5F,
                0.45F,
                0.45F
            };
            ResourceMapsGenerator resourceMapsGenerator = new ResourceMapsGenerator(complexMap, size, resourceReader);
            List<string> categories = resourceReader.getResources().DistinctBy(r => r.Category).Select(r => r.Category).ToList();
            resourceMapsGenerator.GenerateGlobalMap();
            resourceMapsGenerator.GenerateOnGroundMap();
            resourceMapsGenerator.GenerateUnderGroundMap(coefficients);
            for (int i = 0; i < categories.Count; i++)
            {
                Bitmap globalRes = resourceMapsGenerator.MakePicture(categories.ElementAt(i));
                globalRes.Save(folderPath + "\\HTMBSR-R" + categories.ElementAt(i) + "-1.png");
            }
        }
    }

    [TestClass]
    public class PoliticalMapGenClass
    {
        string folderPath = "C:\\Users\\Marina Nik\\Documents\\Visual Studio projects\\Diploma Parts - D.E.W\\Data Files\\Map Generator";

        [TestMethod]
        public void CountriesReadTest() {
            CountryReader countryReader = new CountryReader(folderPath);
            countryReader.readCountriesXlsx();
            List<CountryBase> countries = countryReader.GetCountries();
            Assert.IsNotNull(countries);
        }

        [TestMethod]
        public void CountrySpreadTest()
        {
            CountryReader countryReader = new CountryReader(folderPath);
            countryReader.readCountriesXlsx();
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();

            int size = 1025;
            int smallSize = 257;
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;

            int[,] map = new int[size, size];
            MapPart[,] complexMap = new MapPart[size, size];
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;
                }
            }

            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleBalance(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HP-H1.png");
            heightMapGenerator.MakeMapParts(map, complexMap);

            //Neighbors by 4 directions!
            for (int i = 0; i < size; i++)  //Y coord
            {
                for (int j = 0; j < size; j++)  //X coord
                {
                    if (i != 0)
                    {
                        complexMap[i, j].neighbors.Add(complexMap[i - 1, j]);
                        complexMap[i, j].neighborIDs.Add(complexMap[i - 1, j].ID);
                    }
                    if (j != 0)
                    {
                        complexMap[i, j].neighbors.Add(complexMap[i, j - 1]);
                        complexMap[i, j].neighborIDs.Add(complexMap[i, j - 1].ID);
                    }
                    if (i != size - 1)
                    {
                        complexMap[i, j].neighbors.Add(complexMap[i + 1, j]);
                        complexMap[i, j].neighborIDs.Add(complexMap[i + 1, j].ID);
                    }
                    if (j != size - 1)
                    {
                        complexMap[i, j].neighbors.Add(complexMap[i, j + 1]);
                        complexMap[i, j].neighborIDs.Add(complexMap[i, j + 1].ID);
                    }
                }
            }

            PoliticalMapGenerator politicalMapGenerator = new PoliticalMapGenerator(countryReader, complexMap, size);
            politicalMapGenerator.RunGeneration();
            Bitmap politicImage = politicalMapGenerator.MakePicture();
            politicImage.Save(folderPath + "\\HP-P1.png");
        }

        [TestMethod]
        public void CompleteGenerationTest()
        {
            ResourceReader resourceReader = new ResourceReader(folderPath);
            resourceReader.readResourcesXlsx();
            TemperatureReader temperatureReader = new TemperatureReader(folderPath);
            temperatureReader.readTemperaturesXlsx();
            MoistureReader moistureReader = new MoistureReader(folderPath);
            moistureReader.readMoisturesXlsx();
            HeightReader heightReader = new HeightReader(folderPath);
            heightReader.readHeightsXlsx();
            BiomeReader biomeReader = new BiomeReader(folderPath);
            biomeReader.readBiomesXlsx(resourceReader);
            biomeReader.readBiomeHeightsXlsx(temperatureReader);
            biomeReader.readMoistTempersXlsx(temperatureReader, moistureReader);
            SoilReader soilReader = new SoilReader(folderPath);
            soilReader.readSoilsXlsx(biomeReader);
            CountryReader countryReader = new CountryReader(folderPath);
            countryReader.readCountriesXlsx();

            int size = 513;
            int smallSize = 257;
            int minVal = -15000;
            int maxVal = 10000;
            int range = maxVal - minVal;
            float minMapVal = 0.75F;
            float maxMoistCoef = 0.5F;
            float maxTemperCoef = 0.55F;

            int[,] map = new int[size, size];
            MapPart[,] complexMap = new MapPart[size, size];
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;
                }
            }

            HeightMapNoiseGenerator heightMapGenerator = new HeightMapNoiseGenerator(smallSize, smallSize, size);
            heightMapGenerator.ChangeOctaves(5);
            Bitmap image = heightMapGenerator.RunGenerationColorNoWeightsScaleBalance(map, heightReader, minVal, maxVal);
            image.Save(folderPath + "\\HTMBSRP-H1.png");
            heightMapGenerator.MakeMapParts(map, complexMap);

            TemperMoistPartialGenerator temperMoistPartialGenerator = new TemperMoistPartialGenerator(size);
            MoistureMapGenerator moistureMapGenerator = new MoistureMapGenerator(size, moistureReader, temperMoistPartialGenerator, minMapVal,
                heightReader, map, minVal, range, maxMoistCoef);
            Bitmap moistImage = moistureMapGenerator.RunGeneration();
            moistImage.Save(folderPath + "\\HTMBSRP-M1.png");
            moistureMapGenerator.MakeMapParts(complexMap);

            TemperatureMapGenerator temperMapGenerator = new TemperatureMapGenerator(size, temperatureReader, temperMoistPartialGenerator,
                minMapVal);
            temperMapGenerator.MakeMapPartsBasic(complexMap);
            Bitmap temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSRP-TB1.png");
            temperMapGenerator.commonGenerator.RunGenerWithHeightsFromPrior(heightReader, map, minVal, range, maxTemperCoef, 0);
            temperMapGenerator.MakeMapPartsFinal(complexMap);
            temperImage = temperMapGenerator.RunGeneration();
            temperImage.Save(folderPath + "\\HTMBSRP-TF1.png");

            BiomeMapGenerator biomeMapGenerator = new BiomeMapGenerator(biomeReader, size, complexMap);
            biomeMapGenerator.ChangeItersNum(7);
            biomeMapGenerator.RunGeneration();
            Bitmap biomeImage = biomeMapGenerator.GetPicture();
            biomeImage.Save(folderPath + "\\HTMBSRP-B1.png");

            SoilMapGenerator soilMapGenerator = new SoilMapGenerator(size, complexMap);
            soilMapGenerator.ChangeItersNum(5);
            soilMapGenerator.RunGeneration();
            Bitmap soilImage = soilMapGenerator.GetPicture();
            soilImage.Save(folderPath + "\\HTMBSRP-S1.png");

            List<float> coefficients = new List<float>
            {
                0.0F,
                0.35F,
                0.3F,
                0.3F,
                0.35F,
                0.5F,
                0.65F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.5F,
                0.7F,
                0.35F,
                0.45F,
                0.5F,
                0.5F,
                0.45F,
                0.45F
            };
            ResourceMapsGenerator resourceMapsGenerator = new ResourceMapsGenerator(complexMap, size, resourceReader);
            List<string> categories = resourceReader.getResources().DistinctBy(r => r.Category).Select(r => r.Category).ToList();
            resourceMapsGenerator.GenerateGlobalMap();
            resourceMapsGenerator.GenerateOnGroundMap();
            resourceMapsGenerator.GenerateUnderGroundMap(coefficients);
            for (int i = 0; i < categories.Count; i++)
            {
                Bitmap globalRes = resourceMapsGenerator.MakePicture(categories.ElementAt(i));
                globalRes.Save(folderPath + "\\HTMBSRP-R" + categories.ElementAt(i) + "-1.png");
            }

            for (int i = 0; i < size; i++)  //Y coord
            {
                for (int j = 0; j < size; j++)  //X coord
                {
                    if (i != 0)
                    {
                        complexMap[i, j].neighbors.Add(complexMap[i - 1, j]);
                        complexMap[i, j].neighborIDs.Add(complexMap[i - 1, j].ID);
                    }
                    if (j != 0)
                    {
                        complexMap[i, j].neighbors.Add(complexMap[i, j - 1]);
                        complexMap[i, j].neighborIDs.Add(complexMap[i, j - 1].ID);
                    }
                    if (i != size - 1)
                    {
                        complexMap[i, j].neighbors.Add(complexMap[i + 1, j]);
                        complexMap[i, j].neighborIDs.Add(complexMap[i + 1, j].ID);
                    }
                    if (j != size - 1)
                    {
                        complexMap[i, j].neighbors.Add(complexMap[i, j + 1]);
                        complexMap[i, j].neighborIDs.Add(complexMap[i, j + 1].ID);
                    }
                }
            }

            PoliticalMapGenerator politicalMapGenerator = new PoliticalMapGenerator(countryReader, complexMap, size);
            politicalMapGenerator.RunGeneration();
            Bitmap politicImage = politicalMapGenerator.MakePicture();
            politicImage.Save(folderPath + "\\HTMBSRP-P1.png");

            string outputPath = folderPath + "\\HTMBSRP-MapParts513Out.txt";
            using StreamWriter streamWriter = new StreamWriter(outputPath);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    string strJson = JsonConvert.SerializeObject(complexMap[i, j]);
                    streamWriter.WriteLine(strJson);
                }
            }
            Assert.IsNotNull(complexMap);
        }
    }

    [TestClass]
    public class JSONConvertAndOutputClass
    {
        string folderPath = "C:\\Users\\Marina Nik\\Documents\\Visual Studio projects\\Diploma Parts - D.E.W\\Data Files\\Map Generator";

        [TestMethod]
        public void JSONConvertOneMapPartTest()
        {
            MapPart mapPart = new MapPart(3);
            mapPart.X = 1;
            mapPart.Y = 2;
            mapPart.Height = 530;
            mapPart.TemperatureFinal = new TemperatureObject("Neutral", Color.Green);
            mapPart.TemperatureBasic = new TemperatureObject("Cold", Color.LightBlue);
            mapPart.Moisture = new MoistureObject("Neutral", Color.Yellow);
            mapPart.Biome = new Biome("Forest", Color.DarkGreen);
            mapPart.NoHeightSpecific = false;
            mapPart.Soil = new Soil("BlackSoil", Color.Black);
            mapPart.Resources = new List<IResource>
            {
                new BasicResource(0, "Res1", Color.Red, "Q1"),
                new BasicResource(1, "Res2", Color.Orange, "Q1")
            };
            CountryBase country = new CountryBase("CountryA", Color.Red);
            mapPart.Country = country;
            country.Capital = new System.Numerics.Vector2(32, 32);

            string strJson = JsonConvert.SerializeObject(mapPart);

            string outputPath = folderPath + "\\SingleMapPartOut.txt";
            File.WriteAllText(outputPath, strJson);
            Console.WriteLine(strJson);
            Assert.IsNotNull(strJson);
        }

        [TestMethod]
        public void JSONConvertMultipleMapPartsTest()
        {
            MapPart[,] complexMap = new MapPart[3, 3];
            CountryBase country = new CountryBase("CountryA", Color.Red);
            int counter = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    complexMap[i, j] = new MapPart(counter);
                    complexMap[i, j].X = j;
                    complexMap[i, j].Y = i;
                    counter++;

                    complexMap[i, j].Country = country;
                }
            }
            country.Capital = new System.Numerics.Vector2(1, 1);

            for (int i = 0; i < 3; i++)  //Y coord
            {
                for (int j = 0; j < 3; j++)  //X coord
                {
                    if (i != 0)
                    {
                        complexMap[i, j].neighbors.Add(complexMap[i - 1, j]);
                        complexMap[i, j].neighborIDs.Add(complexMap[i - 1, j].ID);
                    }
                    if (j != 0)
                    {
                        complexMap[i, j].neighbors.Add(complexMap[i, j - 1]);
                        complexMap[i, j].neighborIDs.Add(complexMap[i, j - 1].ID);
                    }
                    if (i != 2)
                    {
                        complexMap[i, j].neighbors.Add(complexMap[i + 1, j]);
                        complexMap[i, j].neighborIDs.Add(complexMap[i + 1, j].ID);
                    }
                    if (j != 2)
                    {
                        complexMap[i, j].neighbors.Add(complexMap[i, j + 1]);
                        complexMap[i, j].neighborIDs.Add(complexMap[i, j + 1].ID);
                    }
                }
            }

            string outputPath = folderPath + "\\MultMapPartOut.txt";
            using StreamWriter streamWriter = new StreamWriter(outputPath);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    //Self-referencing loop in here  FIXED
                    string strJson = JsonConvert.SerializeObject(complexMap[i, j]);
                    streamWriter.WriteLine(strJson);
                }
            }
            Assert.IsNotNull(complexMap);
        }

        [TestMethod]
        public void CountriesOutputTest()
        {
            CountryBase countryA = new CountryBase("CountryA", Color.Red);
            CountryBase countryB = new CountryBase("CountryB", Color.Orange);
            
            countryA.Capital = new System.Numerics.Vector2(0, 1);
            countryB.Capital = new System.Numerics.Vector2(2, 2);

            string outputPath = folderPath + "\\CountriesOut.txt";
            using StreamWriter streamWriter = new StreamWriter(outputPath);

            streamWriter.WriteLine(countryA.Output());
            streamWriter.WriteLine(countryB.Output());
            Assert.IsNotNull(countryA);
        }
    }
}