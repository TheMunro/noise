using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Assets
{
    [CustomEditor(typeof(ProceduralTerrainSettings))]
    public class ProceduralTerrainSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var element = (ProceduralTerrainSettings)target;

            var lengthType = element.GetType().GetField("Length");
            element.Length = EditorGUILayout.IntSlider("Length", element.Length, (int)GetMinimum(lengthType), (int)GetMaximum(lengthType));

            var heightType = element.GetType().GetField("Height");
            element.Height = EditorGUILayout.IntSlider("Height", element.Height, (int)GetMinimum(heightType), (int)GetMaximum(heightType));

            var densityType = element.GetType().GetField("Density");
            element.Density = EditorGUILayout.Slider("Density", element.Density, GetMinimum(densityType), GetMaximum(densityType));

            var alphaMapPowerType = element.GetType().GetField("AlphaMapPower");
            element.AlphaMapPower = EditorGUILayout.IntSlider("AlphaMapPower", element.AlphaMapPower, (int)GetMinimum(alphaMapPowerType), (int)GetMaximum(alphaMapPowerType));
            EditorGUILayout.LabelField("AlphaMapResolution", element.AlphaMapResolution.ToString());

            var heightMapPowerType = element.GetType().GetField("HeightMapPower");
            element.HeightMapPower = EditorGUILayout.IntSlider("HeightMapPower", element.HeightMapPower, (int)GetMinimum(heightMapPowerType), (int)GetMaximum(heightMapPowerType));
            EditorGUILayout.LabelField("HeightMapResolution", element.HeightMapResolution.ToString());
        }

        private static float GetMinimum(MemberInfo type)
        {
            return ((RangeAttribute)Attribute.GetCustomAttribute(type, typeof(RangeAttribute))).min;
        }

        private static float GetMaximum(MemberInfo type)
        {
            return ((RangeAttribute)Attribute.GetCustomAttribute(type, typeof(RangeAttribute))).max;
        }
    }

    class ProceduralTerrainSettings : MonoBehaviour
    {
        [Range(1, 1000)]
        public int Length = 250;

        [Range(1, 100)]
        public int Height = 5;

        [Range(1f, 32f)]
        public float Density = 8f;

        [Range(4, 16)]
        public int HeightMapPower = 8;
        
        public int HeightMapResolution { get { return 1 << HeightMapPower + 1; } }

        [Range(4, 16)]
        public int AlphaMapPower = 8;

        public int AlphaMapResolution { get { return 1 << AlphaMapPower + 1; } }
    }
}
