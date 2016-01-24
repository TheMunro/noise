using UnityEngine;

namespace Assets.Scripts
{
    public class FractalBrownianMotion3D : Noise3D
    {
        public Noise3D NoiseGenerator;

        [Range(1, 16)]
        public int Octaves;

        [Range(1f, 4f)]
        public float Lacunarity = 2.1042f;

        [Range(0.05f, 0.75f)]
        public float Persistence = 0.5f;

        public override float Noise(float x, float y, float z)
        {
            var value = 0f;
            //for(var i = 0; i < Octaves; i++)
            value += NoiseGenerator.Noise(x, y, z);

            return value;
        }
    }
}