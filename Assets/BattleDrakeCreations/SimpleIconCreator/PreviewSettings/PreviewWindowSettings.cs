using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleDrakeCreations.SimpleIconCreator
{
    public class PreviewSettings : ScriptableObject
    {
        public string iconSavePath;
        public string settingsSavePath;
        public Vector3 objectPosition;
        public Vector3 objectRotation;
        public float previewFOV;
        public Color lightOneColor;
        public float lightOneIntensity;
        public Color lightTwoColor;
        public float lightTwoIntensity;
        public bool isTransparent;
        public Color backgroundColor;
        public Texture2D backgroundTexture;
        public Texture2D foregroundTexture;
        public int iconResolution;
    }
}
