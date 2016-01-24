using UnityEngine;

namespace Assets
{
    public interface INoise3D
    {
        float Noise(float x, float y, float z);
    }

    public abstract class Noise3D : MonoBehaviour, INoise3D
    {
        public abstract float Noise(float x, float y, float z);
    }
}
