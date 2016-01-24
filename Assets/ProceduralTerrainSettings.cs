using UnityEngine;

namespace Assets
{
    class ProceduralTerrainSettings : MonoBehaviour
    {
        [Range(1, 100)]
        public int Length = 64;

        [Range(1, 100)]
        public int Height = 5;

        [Range(1f, 32f)]
        public float Density = 8f;

        [Range(1, 32)]
        public int HeightMapResolution = 8;

        [Range(1, 32)]
        public int AlphaMapResolution = 8;

    }
}
