using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Noise2D))]
    [RequireComponent(typeof(ProceduralTerrainSettings))]
    public class ProceduralTerrain : MonoBehaviour
    {
        private Terrain _terrain;
        private ProceduralTerrainSettings _settings;

        public  Noise2D NoiseGenerator;

        void Start ()
        {
            _settings = GetComponent<ProceduralTerrainSettings>();
            //CreateTexture();
            CreateTerrain();
        }

        //private void CreateTexture()
        //{ 
        //    var resolution = _settings.HeightMapResolution;
        //    var texture = new Texture2D(resolution, resolution);

        //    for (var z = 0; z < resolution; z++)
        //    {
        //        for (var x = 0; x < resolution; x++)
        //        {
        //            var xCoordinate = (_settings.Density * x) / resolution;
        //            var zCoordinate = (_settings.Density * z) / resolution;
                    
        //            var noise = 0.5f * (NoiseGenerator.Noise(xCoordinate, zCoordinate) + 1);
        //            texture.SetPixel(x, z, new Color(noise, noise, noise));
        //        }
        //    }

        //    // Encode texture into PNG
        //    var bytes = texture.EncodeToPNG();
        //    Destroy(texture);

        //    // For testing purposes, also write to a file in the project folder
        //    File.WriteAllBytes(Application.dataPath + "/../Perlin.png", bytes);
        //}

        public void CreateTerrain()
        {
            var data = new TerrainData();
            data.alphamapResolution = _settings.AlphaMapResolution;
            data.heightmapResolution = _settings.HeightMapResolution;

            var heightmap = GetHeightmap();
            data.SetHeights(0, 0, heightmap);
            ApplyTextures(data);

            data.size = new Vector3(_settings.Length, _settings.Height, _settings.Length);

            var terrain = Terrain.CreateTerrainGameObject(data);
            terrain.transform.position = new Vector3(-0.5f * _settings.Length, 0, -0.5f * _settings.Length);

            _terrain = terrain.GetComponent<Terrain>();
            _terrain.heightmapPixelError = 8;
            _terrain.materialType = Terrain.MaterialType.Custom;
            _terrain.materialTemplate = _settings.TerrainMaterial;
            _terrain.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            _terrain.Flush();
        }

        private void ApplyTextures(TerrainData data)
        {
            var flatSplat = new SplatPrototype();
            var steepSplat = new SplatPrototype();

            flatSplat.texture = _settings.FlatTexture;
            steepSplat.texture = _settings.SteepTexture;

            data.splatPrototypes = new []
            {
                flatSplat,
                steepSplat
            };

            data.RefreshPrototypes();

            var splatMap = new float[data.alphamapResolution, data.alphamapResolution, 2];

            for (var z = 0; z < data.alphamapHeight; z++)
            {
                for (var x = 0; x < data.alphamapWidth; x++)
                {
                    var normalizedX = (float)x / (data.alphamapWidth - 1);
                    var normalizedZ = (float)z / (data.alphamapHeight - 1);

                    var steepness = data.GetSteepness(normalizedX, normalizedZ);
                    var steepnessNormalized = Mathf.Clamp(steepness , 0, 1f);

                    splatMap[z, x, 0] = 1f - steepnessNormalized;
                    splatMap[z, x, 1] = steepnessNormalized;
                }
            }

            data.SetAlphamaps(0, 0, splatMap);
        }

        private float[,] GetHeightmap()
        {
            var resolution = _settings.HeightMapResolution;
            var heightmap = new float[resolution, resolution];

            for (var z = 0; z < resolution; z++)
            {
                for (var x = 0; x < resolution; x++)
                {
                    var xCoordinate = (_settings.Density * x) / resolution;
                    var zCoordinate = (_settings.Density * z) / resolution;

                    var noise = 0.5f * (NoiseGenerator.Noise(xCoordinate, zCoordinate) + 1);
                    heightmap[x, z] = noise;
                }
            }

            return heightmap;
        }
    }
}



