﻿using System;
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
            DrawDefaultInspector();

            var element = (ProceduralTerrainSettings)target;
            EditorGUILayout.LabelField("AlphaMapResolution", element.AlphaMapResolution.ToString());
            EditorGUILayout.LabelField("HeightMapResolution", element.HeightMapResolution.ToString());
        }

        //private static float GetMinimum(MemberInfo type)
        //{
        //    return ((RangeAttribute)Attribute.GetCustomAttribute(type, typeof(RangeAttribute))).min;
        //}

        //private static float GetMaximum(MemberInfo type)
        //{
        //    return ((RangeAttribute)Attribute.GetCustomAttribute(type, typeof(RangeAttribute))).max;
        //}
    }

    class ProceduralTerrainSettings : MonoBehaviour
    {
        [Range(1, 1000)]
        public int Length = 250;

        [Range(1, 25)]
        public int Height = 5;

        [Range(1f, 16f)]
        public float Density = 8f;

        [Range(4, 10)]
        public int HeightMapPower = 8;
        
        public int HeightMapResolution { get { return (1 << HeightMapPower) + 1; } }

        [Range(4, 10)]
        public int AlphaMapPower = 8;

        public int AlphaMapResolution { get { return (1 << AlphaMapPower) + 1; } }

        public Texture2D FlatTexture;
        public Texture2D SteepTexture;
    }
}