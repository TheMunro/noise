using System.IO;
using UnityEngine;

namespace Assets
{
    public class ProceduralTerrain : MonoBehaviour
    {
        [Range(4, 16)]
        public int Power = 8;

        [Range(1, 100)]
        public int Seed = 50;

        public int Width = 64;
        public int Height = 5;
        public int Depth = 64;

        private INoise2D _perlin2D;
        private Terrain _terrain;

        void Start ()
        {
            _perlin2D = new PerlinNoise2D(Power, Seed);

            var texture = new Texture2D(Width, Depth);

            
            for (var z = 0; z < Depth; z++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var xCoordinate = (8f * x) / Width;
                    var zCoordinate = (8f * z) / Depth;

                    var noise = 0.5f * (_perlin2D.Noise(xCoordinate, zCoordinate) + 1);
                    //var noise = Mathf.PerlinNoise(xCoordinate, zCoordinate);
                    texture.SetPixel(x, z, new Color(noise, noise, noise));
                }
            }

            // Encode texture into PNG
            var bytes = texture.EncodeToPNG();
            Object.Destroy(texture);

            // For testing purposes, also write to a file in the project folder
            File.WriteAllBytes(Application.dataPath + "/../Perlin.png", bytes);

            CreateTerrain();
        }
	
        public void CreateTerrain()
        {
            var terrainData = new TerrainData();
            terrainData.heightmapResolution = Height +1;
            terrainData.alphamapResolution = Height + 1;

            var heightmap = GetHeightmap();
            terrainData.SetHeights(0, 0, heightmap);
            terrainData.size = new Vector3(Width, Height, Depth);

            var terrain = Terrain.CreateTerrainGameObject(terrainData);
            _terrain = terrain.GetComponent<Terrain>();
            _terrain.Flush();
        }

        private float[,] GetHeightmap()
        {
            var heightmap = new float[Height, Height];

            for (var z = 0; z < Height; z++)
            {
                for (var x = 0; x < Height; x++)
                {
                    var xCoordinate = (8f * x) / Width;
                    var zCoordinate = (8f * z) / Depth;

                    heightmap[x, z] = _perlin2D.Noise(xCoordinate, zCoordinate);
                }
            }

            return heightmap;
        }
    }
}
