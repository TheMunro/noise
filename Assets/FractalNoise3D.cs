namespace Assets
{
    public class FractalNoise3D : Noise3D
    {
        public Noise3D NoiseGenerator;

        public int Octaves;
        public int Lacunarity;

        public override float Noise(float x, float y, float z)
        {
            var value = 0f;
            //for(var i = 0; i < Octaves; i++)
            value += NoiseGenerator.Noise(x, y, z);

            return value;
        }
    }
}