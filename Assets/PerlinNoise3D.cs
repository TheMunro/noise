using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
    public class PerlinNoise3D : Noise3D
    {
        private readonly Vector3[] _gradients =
        {
            new Vector3(1, 1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0),
            new Vector3(1, 0, 1), new Vector3(-1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1),
            new Vector3(0, 1, 1), new Vector3(0, -1, 1), new Vector3(0, 1, -1), new Vector3(0, -1, -1)
        };

        private readonly int _resolution;
        private readonly int[] _permutations;

        public PerlinNoise3D(int power, int seed)
        {
            _resolution = 1 << power;
            _permutations = new int[2 * _resolution];

            var range = Enumerable.Range(0, _resolution).ToList();
            Shuffle(range, seed);

            //duplicate permutations for wrapped index lookup
            for (var i = 0; i < 2 * _resolution; i++)
                _permutations[i] = range[i % _resolution];
        }

        ////Fisher-Yates Shuffle
        ////http://www.dotnetperls.com/fisher-yates-shuffle
        private static void Shuffle(IList<int> array, int seed)
        {
            //make Random.value deterministic
            Random.seed = seed;
            var n = array.Count;
            for (var i = 0; i < n; i++)
            {
                var r = i + (int)(Random.value * (n - i));
                var t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        private static float SmoothStep(float x)
        {
            //zero 1st derivative at endpoints
            return x * (x * (3 - 2 * x));
        }

        private static float SmootherStep(float x)
        {
            //zero 2nd derivative at endpoints
            return x * (x * (x * (10 + (x * (6 * x - 15)))));
        }

        public override float Noise(float x, float y, float z)
        {
            var minimumX = Mathf.FloorToInt(x);
            var minimumY = Mathf.FloorToInt(y);
            var minimumZ = Mathf.FloorToInt(z);

            //get delta offset
            var deltaX = x - minimumX;
            var deltaY = y - minimumY;
            var deltaZ = z - minimumZ;

            //wrap integers to resolution limit
            minimumX = minimumX & _resolution - 1;
            minimumY = minimumY & _resolution - 1;
            minimumZ = minimumZ & _resolution - 1;

            //hashed gradient indices based on deterministic lookup in permutations array via vertex coordinates (this is the reason for the double length array to avoid wrapping issues)
            var gradientIndex000 = _permutations[minimumX + _permutations[minimumY + _permutations[minimumZ]]] % 12;
            var gradientIndex001 = _permutations[minimumX + _permutations[minimumY + _permutations[minimumZ + 1]]] % 12;
            var gradientIndex010 = _permutations[minimumX + _permutations[minimumY + 1 + _permutations[minimumZ]]] % 12;
            var gradientIndex011 = _permutations[minimumX + _permutations[minimumY + 1 + _permutations[minimumZ + 1]]] % 12;
            var gradientIndex100 = _permutations[minimumX + 1 + _permutations[minimumY + _permutations[minimumZ]]] % 12;
            var gradientIndex101 = _permutations[minimumX + 1 + _permutations[minimumY + _permutations[minimumZ + 1]]] % 12;
            var gradientIndex110 = _permutations[minimumX + 1 + _permutations[minimumY + 1 + _permutations[minimumZ]]] % 12;
            var gradientIndex111 = _permutations[minimumX + 1 + _permutations[minimumY + 1 + _permutations[minimumZ + 1]]] % 12;

            //contribution from each vertex gradient
            var n000 = Vector3.Dot(_gradients[gradientIndex000], new Vector3(deltaX, deltaY, deltaZ));
            var n001 = Vector3.Dot(_gradients[gradientIndex001], new Vector3(deltaX, deltaY, deltaZ - 1));
            var n010 = Vector3.Dot(_gradients[gradientIndex010], new Vector3(deltaX, deltaY - 1, deltaZ));
            var n011 = Vector3.Dot(_gradients[gradientIndex011], new Vector3(deltaX, deltaY - 1, deltaZ - 1));

            var n100 = Vector3.Dot(_gradients[gradientIndex100], new Vector3(deltaX - 1, deltaY, deltaZ));
            var n101 = Vector3.Dot(_gradients[gradientIndex101], new Vector3(deltaX - 1, deltaY, deltaZ - 1));
            var n110 = Vector3.Dot(_gradients[gradientIndex110], new Vector3(deltaX - 1, deltaY - 1, deltaZ));
            var n111 = Vector3.Dot(_gradients[gradientIndex111], new Vector3(deltaX - 1, deltaY - 1, deltaZ - 1));
        
            var u = SmootherStep(deltaX);
            var v = SmootherStep(deltaY);
            var w = SmootherStep(deltaZ);

            var nx00 = Mathf.Lerp(n000, n100, u);
            var nx01 = Mathf.Lerp(n001, n101, u);
            var nx10 = Mathf.Lerp(n010, n110, u);
            var nx11 = Mathf.Lerp(n011, n111, u);

            var nxy0 = Mathf.Lerp(nx00, nx10, v);
            var nxy1 = Mathf.Lerp(nx01, nx11, v);

            var nxyz = Mathf.Lerp(nxy0, nxy1, w);

            return nxyz;
        }
    }
}