namespace Assets
{
    public class FractalNoise2D : Noise2D
    {
        public Noise2D NoiseGenerator;

        public int Octaves;
        public int Lacunarity;

        public override float Noise(float x, float y)
        {
            var value = 0f;
            //for(var i = 0; i < Octaves; i++)
            value += NoiseGenerator.Noise(x, y);

            return value;
        }
    }
}