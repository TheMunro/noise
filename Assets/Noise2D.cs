using UnityEngine;

namespace Assets
{
    public interface INoise2D
    {
        float Noise(float x, float y);
    }

    public abstract class Noise2D : MonoBehaviour, INoise2D
    {
        public abstract float Noise(float x, float y);
    }
}