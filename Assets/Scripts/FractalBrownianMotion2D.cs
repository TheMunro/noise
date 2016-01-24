using UnityEngine;

namespace Assets
{
    public class FractalBrownianMotion2D : Noise2D
    {
        public Noise2D NoiseGenerator;

        [Range(1, 16)]
        public int Octaves;

        [Range(1f, 4f)]
        public float Lacunarity = 2.1042f;

        [Range(0.05f, 0.75f)]
        public float Persistence = 0.5f;

        public override float Noise(float x, float y)
        {
            var sum = 0f;
            var frequency = 1f;
            var amplitude = Persistence;

            for(var i = 0; i < Octaves; i++)
            {
                sum += NoiseGenerator.Noise(x * frequency, y * frequency) * amplitude;
                frequency *= Lacunarity;
                amplitude *= Persistence;
            }
            return sum;
        }
    }
}