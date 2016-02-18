using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [CustomEditor(typeof(ProceduralTerrain))]
    public class ProceduralTerrainEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var element = (ProceduralTerrain)target;
            if (GUILayout.Button("Generate Texture"))
                CreateTexture(element);
        }

        private static void CreateTexture(ProceduralTerrain element)
        {
            var path = EditorUtility.SaveFilePanel("Save Texture", string.Empty, "texture.png", "png");
            if (string.IsNullOrEmpty(path))
                return;

            var resolution = element.Settings.HeightMapResolution;
            var texture = new Texture2D(resolution, resolution);

            for (var z = 0; z < resolution; z++)
            {
                for (var x = 0; x < resolution; x++)
                {
                    var xCoordinate = (element.Settings.Density * x) / resolution;
                    var zCoordinate = (element.Settings.Density * z) / resolution;

                    var noise = 0.5f * (element.NoiseGenerator.Noise(xCoordinate, zCoordinate) + 1);
                    texture.SetPixel(x, z, new Color(noise, noise, noise));
                }
            }

            // Encode texture into PNG
            var bytes = texture.EncodeToPNG();
            Destroy(texture);
            //File.WriteAllBytes(Application.dataPath + "/../Perlin.png", bytes);
            File.WriteAllBytes(path, bytes);
        }

        //private static float GetMinimum(MemberInfo type)
        //{
        //    return ((RangeAttribute)Attribute.GetCustomAttribute(type, typeof (RangeAttribute))).min;
        //}

        //private static float GetMaximum(MemberInfo type)
        //{
        //    return ((RangeAttribute)Attribute.GetCustomAttribute(type, typeof(RangeAttribute))).max;
        //}
    }

    [RequireComponent(typeof(Noise2D))]
    [RequireComponent(typeof(ProceduralTerrainSettings))]
    public class ProceduralTerrain : MonoBehaviour
    {
        private Terrain _terrain;

        public Noise2D NoiseGenerator;
        public ProceduralTerrainSettings Settings;

        void Start ()
        {
            CreateTerrain();
        }

        public void CreateTerrain()
        {
            var data = new TerrainData();
            data.alphamapResolution = Settings.AlphaMapResolution;
            data.heightmapResolution = Settings.HeightMapResolution;

            var heightmap = GetHeightmap();
            data.SetHeights(0, 0, heightmap);
            ApplyTextures(data);

            data.size = new Vector3(Settings.Length, Settings.Height, Settings.Length);

            var terrain = Terrain.CreateTerrainGameObject(data);
            terrain.transform.position = new Vector3(-0.5f * Settings.Length, 0, -0.5f * Settings.Length);

            _terrain = terrain.GetComponent<Terrain>();
            _terrain.heightmapPixelError = 8;
            _terrain.materialType = Terrain.MaterialType.Custom;
            _terrain.materialTemplate = Settings.TerrainMaterial;
            _terrain.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            _terrain.Flush();
        }

        private void ApplyTextures(TerrainData data)
        {
            var flatSplat = new SplatPrototype();
            var steepSplat = new SplatPrototype();

            flatSplat.texture = Settings.FlatTexture;
            steepSplat.texture = Settings.SteepTexture;

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

                    var steepness = data.GetSteepness(normalizedX, normalizedZ) * Settings.SteepnessTextureMultiplier;
                    var steepnessNormalized = Mathf.Clamp(steepness, 0, 1f);

                    splatMap[z, x, 0] = 1f - steepnessNormalized;
                    splatMap[z, x, 1] = steepnessNormalized;
                }
            }

            data.SetAlphamaps(0, 0, splatMap);
        }

        private float[,] GetHeightmap()
        {
            var resolution = Settings.HeightMapResolution;
            var heightmap = new float[resolution, resolution];

            for (var z = 0; z < resolution; z++)
            {
                for (var x = 0; x < resolution; x++)
                {
                    var xCoordinate = (Settings.Density * x) / resolution;
                    var zCoordinate = (Settings.Density * z) / resolution;

                    var noise = 0.5f * (NoiseGenerator.Noise(xCoordinate, zCoordinate) + 1);
                    heightmap[x, z] = noise;
                }
            }

            return heightmap;
        }
    }
}



