# GameMapGenerator-CSharp
A map generator with the usage of noise functions

Thank you Auburn for your FastNoiseLite library! Link here: https://github.com/Auburn/FastNoiseLite

Thank you JamesNK for your Newtonsoft.Json library! Link here: https://github.com/JamesNK/Newtonsoft.Json

Thank you ahmedwalid05 for your FastExcel library! Link here: https://github.com/ahmedwalid05/FastExcel

No program to run, sorry. I've put my Unit tests here for you to see how the module works. I've planned an interface but didn't get to make it. The most fresh test classes are: BalancingClass, ResourceGenClass and PoliticalMapGenClass.

# Parts of the generation
1. Height map
2. Temperature map (depends on Height)
3. Moisture map (depends on Height)
4. Biome map (depends on Height, Temperature and Moisture)
5. Soil map (depends on Biome)
6. 9 Resource maps - Global (Water, Forest and Soil), OnGround (Wildlife and Crops) and Underground, split by custom categories. Global and OnGround depend on Biome, Underground isn't dependent on anything
7. Political map (depends on Height)

# Where the noise is used
1. To generate the Height map
2. To make Temperature and Moisture maps more random
3. To generate Underground resources maps

# What else is used
Height map generator uses Gauss filter to make the shifts in heights less drastic.

Heights are mapped to different value interval which is set manually.

Political map generator uses a Flood fill technique to spread each country's territory.

Height map uses Diamond-Square algorithm for scaling. That's why the map MUST be square and the sides size MUST be 2 ^ power + 1 (65, 129, 257, 513 etc.)

Biome, Soil and Resource maps (Global and OnGround) generators use Multi-state cellular automata to balance their maps and give them a better look.

I use Newtonsoft.Json for output. I have huge plans on this program which I can't run inside one program module. So for the Economic part I'll need to make a separate module and transfer some data there. To do this, I use Json format for some MapPart class properties needed inside the Ecomonic module.

Most, if not all the input data is stored inside Excel sheets to be more compact.

# Output example
Inside output of the most complex test, there will be: 16 pictures (Height map, two Temperature maps (basic and final - the latter includes height impact), Moisture map, Biome map, Soil map, Global resources map, OnGround resources map, all UnderGround resource maps, Political map) and 1 txt file with MapPart objects.

The example of a full test (without txt file) follows. My advice is to read the xlsx files (DataFiles folder) first and to use the color picker (I use this one: https://colorpicker.me) to understand all color codes (I didn't make legends for all maps, only for some of them)

# Height map
![HTMBSRP-H1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/5eda12a2-b74d-4589-8a7b-26998491f4ad)
# Temperature maps (basic - final)
![HTMBSRP-TB1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/54fb14f6-e9a2-4953-9874-cfe99c47b74a)
![HTMBSRP-TF1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/7f89c10f-ce89-4428-a533-890a4dbe2e2b)
# Moisture map
![HTMBSRP-M1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/debf1a1b-cd20-44a1-95a8-98419a4e576b)
# Biome map
![HTMBSRP-B1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/73dc5d68-6105-402c-812f-7019b8cd0ed4)
# Soil map
![HTMBSRP-S1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/9559879e-702a-469e-bac4-2bde436a8e45)
# Global resources map
![HTMBSRP-RG-1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/fe5b2f4b-1170-4c30-9630-258f39c83a5f)
# OnGround resources map
![HTMBSRP-RO-1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/b7a8eac1-74c5-4b90-bd4a-a97eacd2be2b)
# UnderGround resources maps
![HTMBSRP-RU1-1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/9a925608-47bd-4e75-a474-6c308fc88f91)
![HTMBSRP-RU2-1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/115e8a95-27e4-4566-bf49-e26dfa1ee1a1)
![HTMBSRP-RU3-1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/650c80fb-db9a-4a94-b515-94064a720629)
![HTMBSRP-RU4-1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/5808c5f4-0d04-47aa-813c-09f3e76bd04b)
![HTMBSRP-RU5-1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/6332d836-2475-4be3-bdd1-753d8b63cca9)
![HTMBSRP-RU6-1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/8ee401d6-6a20-434b-86a9-d805bf123f79)
![HTMBSRP-RU7-1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/93dfb85f-dbe2-4c75-b7ea-799a465d808d)
# Political map
![HTMBSRP-P1](https://github.com/MarinaNikolaieva/GameMapGenerator-CSharp/assets/60624855/bcfe78db-3413-46ca-bda3-e0b23151dde1)
