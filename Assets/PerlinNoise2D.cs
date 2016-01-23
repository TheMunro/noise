using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
    public interface INoise2D
    {
        float Noise(float x, float y);
    }

    public class PerlinNoise2D : INoise2D
    {
        private readonly Vector2[] _gradients =
        {
            new Vector2(0, 1), new Vector2(0, -1),
            new Vector2(1, 0), new Vector2(-1, 0)
        };

        private readonly int _resolution;
        private readonly int[] _permutations;

        public PerlinNoise2D(int power, int seed)
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

        public float Noise(float x, float y)
        {
            var minimumX = Mathf.FloorToInt(x);
            var minimumY = Mathf.FloorToInt(y);

            //get delta offset
            var deltaX = x - minimumX;
            var deltaY = y - minimumY;

            //wrap integers to resolution limit
            minimumX = minimumX & _resolution - 1;
            minimumY = minimumY & _resolution - 1;

            //hashed gradient indices based on deterministic lookup in permutations array via vertex coordinates (this is the reason for the double length array to avoid wrapping issues)
            var gradientIndex00 = _permutations[minimumX + _permutations[minimumY]] % 4;
            var gradientIndex01 = _permutations[minimumX + _permutations[minimumY + 1]] % 4;
            var gradientIndex10 = _permutations[minimumX + 1 + _permutations[minimumY]] % 4;
            var gradientIndex11 = _permutations[minimumX + 1 + _permutations[minimumY + 1]] % 4;

            //contribution from each vertex gradient
            var n00 = Vector2.Dot(_gradients[gradientIndex00], new Vector2(deltaX, deltaY));
            var n01 = Vector2.Dot(_gradients[gradientIndex01], new Vector2(deltaX, deltaY - 1));
            var n10 = Vector2.Dot(_gradients[gradientIndex10], new Vector2(deltaX - 1, deltaY));
            var n11 = Vector2.Dot(_gradients[gradientIndex11], new Vector2(deltaX - 1, deltaY - 1));

            var u = SmootherStep(deltaX);
            var v = SmootherStep(deltaY);

            var nx0 = Mathf.Lerp(n00, n10, u);
            var nx1 = Mathf.Lerp(n01, n11, u);

            var nxy = Mathf.Lerp(nx0, nx1, v);

            return nxy;
        }
    }
}