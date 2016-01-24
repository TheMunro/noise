using System.IO;
using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(ProceduralTerrainSettings))]
    public class ProceduralTerrain : MonoBehaviour
    {
        private Terrain _terrain;
        private ProceduralTerrainSettings _settings;

        public  Noise2D NoiseGenerator;


        void Start ()
        {
            _settings = GetComponent<ProceduralTerrainSettings>();
            //var texture = new Texture2D(Length, Length);

            //for (var z = 0; z < Length; z++)
            //{
            //    for (var x = 0; x < Length; x++)
            //    {
            //        var xCoordinate = (Density * x) / Length;
            //        var zCoordinate = (Density * z) / Length;

            //        var noise = 0.5f * (NoiseGenerator.Noise(xCoordinate, zCoordinate) + 1);
            //        //var noise = Mathf.PerlinNoise(xCoordinate, zCoordinate);
            //        texture.SetPixel(x, z, new Color(noise, noise, noise));
            //    }
            //}

            //// Encode texture into PNG
            //var bytes = texture.EncodeToPNG();
            //Object.Destroy(texture);

            //// For testing purposes, also write to a file in the project folder
            //File.WriteAllBytes(Application.dataPath + "/../Perlin.png", bytes);

            CreateTerrain();
        }
	
        public void CreateTerrain()
        {
            var terrainData = new TerrainData();

            var heightmap = GetHeightmap();
            terrainData.SetHeights(0, 0, heightmap);
            terrainData.size = new Vector3(_settings.Length, _settings.Height, _settings.Length);

            var terrain = Terrain.CreateTerrainGameObject(terrainData);
            _terrain = terrain.GetComponent<Terrain>();
            
            _terrain.Flush();
        }

        private float[,] GetHeightmap()
        {
            var resolution = _settings.HeightMapResolution;
            var heightmap = new float[resolution, resolution];

            for (var z = 0; z < resolution; z++)
            {
                for (var x = 0; x < resolution; x++)
                {
                    var xCoordinate = (8f * x) / resolution;
                    var zCoordinate = (8f * z) / resolution;

                    heightmap[x, z] = NoiseGenerator.Noise(xCoordinate, zCoordinate);
                }
            }

            return heightmap;
        }
    }
}
