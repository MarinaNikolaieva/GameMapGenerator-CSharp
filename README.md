# GameMapGenerator-CSharp
A map generator with the usage of noise functions

Thank you Auburn for your FastNoiseLite library! Link here: https://github.com/Auburn/FastNoiseLite

# Parts of the generation
1. Height map
2. Temperature map (depends on Height)
3. Moisture map (depends on Height)
4. Biome map (depends on Height, Temperature and Moisture)
5. Soil map (depends on Biome)
6. 9 Resource maps - Global (Water, Forest and Soil), OnGround (Wildlife and Crops) and Underground, split by custom categories. Global and OnGround depend on Biome, Underground isn't dependent on anything
7. Political map (depends on Height)

# Where the noise is used?
1. To generate the Height map
2. To make Temperature and Moisture maps more random
3. To generate Underground resources maps

# What else is used?
Height map generator uses Gauss filter to make the shifts in heights less drastic.

Heights are mapped to different value interval which is set manually.

Political map generator uses a Flood fill technique to spread each country's territory.

Biome, Soil and Resource maps (Global and OnGround) generators use Multi-state cellular automata to balance their maps and give them a better look.
