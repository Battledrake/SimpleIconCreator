using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using BattleDrakeCreations.Utilities;

namespace BattleDrakeCreations.SimpleIconCreator
{

    public enum ImageFilterMode
    {
        Nearest = 0,
        Bilinear = 1,
        Average = 2
    }

    public class IconCreatorWindow : EditorWindow
    {
        private CustomPreviewEditor _previewWindow;
        private GameObject _targetObject;

        private Material _targetMaterial;
        private PreviewSettings _previewSettings;
        private string _previewSettingsFileName = "PreviewSettings";

        private int _xPadding = 5;
        private int _yPadding = 5;
        private Rect _previewRect;

        private GUIStyle _customStyle;
        private GUIStyle _titleStyle;

        private Rect _iconOptionsArea;
        private Rect _previewOptionsArea;
        private Rect _saveOptionsArea;

        private bool _isTransparent;

        private Texture2D _bgTexture;
        private Texture2D _fgTexture;

        private Color _transparencyColor;
        private Color _iconBGColor = new Color(0.192f, 0.192f, 0.192f);
        private Color _previewLightOneColor = Color.white;
        private float _previewLightOneIntensity = 1.0f;
        private Color _previewLightTwoColor = Color.white;
        private float _previewLightTwoIntensity = 1.0f;

        private string _iconName = "NewIcon";
        private string _savePath = "BattleDrakeCreations/SimpleIconCreator/Icons/";

        private string _settingsSavePath = "BattleDrakeCreations/SimpleIconCreator/PreviewSettings/";

        private int _iconResIndex = 3;
        private int[] _iconResolutions = { 32, 64, 128, 256, 512, 1024, 2048, 4096 };

        private ImageFilterMode _imageFilterMode = ImageFilterMode.Average;

        public GameObject TargetObject { get { return _targetObject; } set { _targetObject = value; } }
        public CustomPreviewEditor PreviewEditor => _previewWindow;

        public event Action<CustomPreviewEditor> OnPreviewCreated;

        [MenuItem("BattleDrakeCreations/SimpleIconCreator")]
        public static void ShowWindow()
        {
            IconCreatorWindow editorWindow = EditorWindow.GetWindow<IconCreatorWindow>("Simple Icon Creator");

            editorWindow.minSize = new Vector2(512, 512);
            editorWindow.Show();
        }

        private void OnEnable()
        {
            _previewRect = new Rect(_xPadding, _yPadding, 256, 256);
            _iconOptionsArea = new Rect(_xPadding + _previewRect.width + _xPadding, _yPadding, 241, 200);
            _previewOptionsArea = new Rect(_xPadding, _yPadding + _previewRect.height + _yPadding, _previewRect.width, 241);
            _saveOptionsArea = new Rect(_xPadding + _previewRect.width + _xPadding, _yPadding + _iconOptionsArea.height + _yPadding / 2, 241, 300);

            _transparencyColor = Color.magenta;
        }

        private void UpdateOptionsToPreviewSettings()
        {
            if (!_isTransparent)
            {
                _previewWindow.SetBackgroundColor(_iconBGColor);
            }
            else
            {
                _previewWindow.SetBackgroundColor(_transparencyColor, true);
            }
            _previewWindow.LightOneColor = _previewLightOneColor;
            _previewWindow.LightOneIntensity = _previewLightOneIntensity;

            _previewWindow.LightTwoColor = _previewLightTwoColor;
            _previewWindow.LightTwoIntensity = _previewLightTwoIntensity;

            if (_bgTexture != null)
                _previewWindow.BGTexture = _bgTexture;
            if (_fgTexture != null)
                _previewWindow.FGTExture = _fgTexture;
        }

        private void OnGUI()
        {
            _customStyle = new GUIStyle(GUI.skin.button);
            _customStyle.padding = new RectOffset(10, 10, 10, 10);

            _titleStyle = new GUIStyle(GUI.skin.button);
            _titleStyle.fontStyle = FontStyle.Bold;
            _titleStyle.alignment = TextAnchor.MiddleCenter;

            if (_targetObject != null)
            {
                if (_previewWindow == null)
                {
                    _previewWindow = Editor.CreateEditor(_targetObject, typeof(CustomPreviewEditor)) as CustomPreviewEditor;
                    _previewWindow.TargetAsset = _targetObject;
                    OnPreviewCreated?.Invoke(_previewWindow);
                    if (_previewSettings != null)
                        LoadPreviewSettings();
                    UpdateOptionsToPreviewSettings();
                }
                if (_previewWindow.HasPreviewGUI())
                {
                    _previewWindow.OnInteractivePreviewGUI(_previewRect, null);
                    UpdateOptionsToPreviewSettings();
                }
            }
            else
            {
                GUILayout.BeginArea(_previewRect);
                GUI.Box(new Rect(0, 0, _previewRect.width, _previewRect.height), "Image Not Available", _customStyle);
                GUILayout.EndArea();
            }

            /*
             * Icon Options Area
             */
            GUILayout.BeginArea(_iconOptionsArea, _customStyle);

            GUILayout.BeginVertical();

            GUILayout.Label("Icon Options", _titleStyle);

            if (_targetObject != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Transparency");
                EditorGUI.BeginChangeCheck();
                _isTransparent = GUILayout.Toggle(_isTransparent, _isTransparent.ToString()/*, GUI.skin.button*/);
                if (EditorGUI.EndChangeCheck())
                {
                    if (_isTransparent)
                    {
                        _previewWindow.SetBackgroundColor(_transparencyColor, true);
                    }
                    else
                    {
                        _previewWindow.SetBackgroundColor(_iconBGColor);
                    }
                }
                GUILayout.EndHorizontal();

                if (!_isTransparent)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Background Color");
                    EditorGUI.BeginChangeCheck();
                    _iconBGColor = EditorGUILayout.ColorField(_iconBGColor);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _previewWindow.SetBackgroundColor(_iconBGColor);
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Background Texture");
                EditorGUI.BeginChangeCheck();
                _bgTexture = EditorGUILayout.ObjectField(_bgTexture, typeof(Texture2D), false) as Texture2D;
                if (EditorGUI.EndChangeCheck())
                {
                    if (_bgTexture != null)
                        _previewWindow.BGTexture = _bgTexture;
                    else
                    {
                        _previewWindow.BGTexture = null;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Foreground Texture");
                EditorGUI.BeginChangeCheck();
                _fgTexture = EditorGUILayout.ObjectField(_fgTexture, typeof(Texture2D), false) as Texture2D;
                if (EditorGUI.EndChangeCheck())
                {
                    if (_fgTexture != null)
                        _previewWindow.FGTExture = _fgTexture;
                    else
                    {
                        _previewWindow.FGTExture = null;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(50);
                if (GUILayout.Button("Reset Options", GUILayout.Width(120)))
                {
                    ResetIconOptions();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();

            /*
             * Below Preview Options
             */
            GUILayout.BeginArea(_previewOptionsArea, _customStyle);

            GUILayout.BeginVertical();

            GUILayout.Label("Preview Window Options", _titleStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Icon Object");
            EditorGUI.BeginChangeCheck();
            _targetObject = EditorGUILayout.ObjectField(_targetObject, typeof(GameObject), false) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                DestroyImmediate(_previewWindow);
                if (_targetObject != null)
                {
                    _iconName = _targetObject.name;
                    _targetMaterial = _targetObject.GetComponentInChildren<Renderer>().sharedMaterial;
                }
            }
            GUILayout.EndHorizontal();

            if (_targetObject != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Material");
                EditorGUI.BeginChangeCheck();
                _targetMaterial = EditorGUILayout.ObjectField(_targetMaterial, typeof(Material), false) as Material;
                if (EditorGUI.EndChangeCheck())
                {
                    _previewWindow.TargetObject.GetComponentInChildren<Renderer>().sharedMaterial = _targetMaterial;
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Preview Settings:");
                EditorGUI.BeginChangeCheck();
                _previewSettings = EditorGUILayout.ObjectField(_previewSettings, typeof(PreviewSettings), false) as PreviewSettings;
                if (EditorGUI.EndChangeCheck())
                {
                    if (_previewSettings != null)
                    {
                        LoadPreviewSettings();
                        _previewSettingsFileName = _previewSettings.name;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(40);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Light One Color");
                EditorGUI.BeginChangeCheck();
                _previewLightOneColor = EditorGUILayout.ColorField(_previewLightOneColor);
                if (EditorGUI.EndChangeCheck())
                {
                    _previewWindow.LightOneColor = _previewLightOneColor;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Light One Intensity");
                EditorGUI.BeginChangeCheck();
                _previewLightOneIntensity = EditorGUILayout.Slider(_previewLightOneIntensity, 0, 2.0f);
                if (EditorGUI.EndChangeCheck())
                {
                    _previewWindow.LightOneIntensity = _previewLightOneIntensity;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Light Two Color");
                EditorGUI.BeginChangeCheck();
                _previewLightTwoColor = EditorGUILayout.ColorField(_previewLightTwoColor);
                if (EditorGUI.EndChangeCheck())
                {
                    _previewWindow.LightTwoColor = _previewLightTwoColor;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Light Two Intensity");
                EditorGUI.BeginChangeCheck();
                _previewLightTwoIntensity = EditorGUILayout.Slider(_previewLightTwoIntensity, 0, 2.0f);
                if (EditorGUI.EndChangeCheck())
                {
                    _previewWindow.LightTwoIntensity = _previewLightTwoIntensity;
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Reset Lights"))
                {
                    ResetWindowOptions();
                }

                if (GUILayout.Button("Reset Transform"))
                {
                    ResetTarget();
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();

            /*
             * Save Icon Options
             */

            GUILayout.BeginArea(_saveOptionsArea, _customStyle);

            GUILayout.BeginVertical();

            GUILayout.Label("Save Options", _titleStyle);

            if (_targetObject != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Icon Resolution");

                EditorGUI.BeginChangeCheck();
                _iconResIndex = EditorGUILayout.Popup(_iconResIndex, Array.ConvertAll<int, string>(_iconResolutions, x => x.ToString()));
                if (EditorGUI.EndChangeCheck())
                {
                    GUI.changed = true;
                }
                GUILayout.Label("x");
                GUILayout.Label(_iconResolutions[_iconResIndex].ToString());
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Path:");
                _savePath = GUILayout.TextArea(_savePath, GUILayout.Width(175), GUILayout.Height(50));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:");
                _iconName = GUILayout.TextField(_iconName, GUILayout.Width(175));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(50);
                if (GUILayout.Button("Create Icon", GUILayout.Width(120)))
                {
                    if (_previewWindow.PreviewTexture != null)
                    {
                        CreatePngFromTexture();
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(35);

                GUILayout.BeginHorizontal();
                GUILayout.Label("New or Overwrite Preview Settings:");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Path:");
                _settingsSavePath = GUILayout.TextArea(_settingsSavePath, GUILayout.Width(175), GUILayout.Height(50));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Name:");
                _previewSettingsFileName = GUILayout.TextField(_previewSettingsFileName, GUILayout.Width(175));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(50);
                if (GUILayout.Button("Save Settings", GUILayout.Width(120)))
                {
                    CreatePreviewSettings();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void LoadPreviewSettings()
        {
            _savePath = _previewSettings.iconSavePath;
            _previewWindow.TargetObject.transform.position = _previewSettings.objectPosition;
            _previewWindow.TargetObject.transform.rotation = Quaternion.Euler(_previewSettings.objectRotation);
            _previewWindow.PreviewFOV = _previewSettings.previewFOV;
            _previewLightOneColor = _previewSettings.lightOneColor;
            _previewLightOneIntensity = _previewSettings.lightOneIntensity;
            _previewLightTwoColor = _previewSettings.lightTwoColor;
            _previewLightTwoIntensity = _previewSettings.lightTwoIntensity;
            _isTransparent = _previewSettings.isTransparent;
            if (_isTransparent)
            {
                return;
            }
            _iconBGColor = _previewSettings.backgroundColor;
            _bgTexture = _previewSettings.backgroundTexture;
            _fgTexture = _previewSettings.foregroundTexture;
            _iconResIndex = Array.FindIndex<int>(_iconResolutions, (i) => i == _previewSettings.iconResolution);
        }

        private void CreatePreviewSettings()
        {
            PreviewSettings previewSettings = ScriptableObject.CreateInstance<PreviewSettings>();
            previewSettings.iconSavePath = _savePath;
            previewSettings.settingsSavePath = _settingsSavePath;
            previewSettings.objectPosition = _previewWindow.TargetObject.transform.position;
            previewSettings.objectRotation = _previewWindow.TargetObject.transform.rotation.eulerAngles;
            previewSettings.previewFOV = _previewWindow.PreviewFOV;
            previewSettings.lightOneColor = _previewLightOneColor;
            previewSettings.lightOneIntensity = _previewLightTwoIntensity;
            previewSettings.lightTwoColor = _previewLightTwoColor;
            previewSettings.lightTwoIntensity = _previewLightTwoIntensity;
            previewSettings.isTransparent = _isTransparent;
            previewSettings.backgroundColor = _iconBGColor;
            previewSettings.backgroundTexture = _bgTexture;
            previewSettings.foregroundTexture = _fgTexture;
            previewSettings.iconResolution = _iconResolutions[_iconResIndex];

            AssetDatabase.CreateAsset(previewSettings, "Assets/" + _settingsSavePath + _previewSettingsFileName + ".asset");
            _previewSettings = previewSettings;
        }

        private void ResetIconOptions()
        {
            _iconBGColor = new Color(0.192f, 0.192f, 0.192f);
            _previewWindow.SetBackgroundColor(_iconBGColor);

            _isTransparent = false;

            _previewWindow.BGTexture = null;
            _bgTexture = null;

            _previewWindow.FGTExture = null;
            _fgTexture = null;
        }

        private void ResetWindowOptions()
        {
            _previewLightOneColor = Color.white;
            _previewWindow.LightOneColor = Color.white;
            _previewLightOneIntensity = 1.0f;
            _previewWindow.LightOneIntensity = 1.0f;

            _previewLightTwoColor = Color.white;
            _previewWindow.LightTwoColor = Color.white;
            _previewLightTwoIntensity = 1.0f;
            _previewWindow.LightTwoIntensity = 1.0f;
        }

        private void ResetTarget()
        {
            _previewWindow.ResetTargetObject();
        }

        private void CreatePngFromTexture()
        {
            Texture2D textureToConvert = _previewWindow.PreviewTexture;

            if (_isTransparent)
            {
                textureToConvert = CreateTextureWithTransparency(textureToConvert);
            }

            textureToConvert = ResizeTexture(textureToConvert, _imageFilterMode, _iconResolutions[_iconResIndex], _iconResolutions[_iconResIndex]);

            byte[] byteTexture = textureToConvert.EncodeToPNG();
            string folderPath = Application.dataPath + "/" + _savePath;
            File.WriteAllBytes(Application.dataPath + "/" + _savePath + _iconName + ".png", byteTexture);

            AssetDatabase.Refresh();
        }

        private Texture2D CreateTextureWithTransparency(Texture2D texture)
        {
            Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

            Color[] pixels = texture.GetPixels(0, 0, texture.width, texture.height);

            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i] == _transparencyColor)
                {
                    pixels[i] = Color.clear;
                }
            }
            newTexture.SetPixels(0, 0, texture.width, texture.height, pixels);
            newTexture.Apply();

            return newTexture;
        }

        public Texture2D ResizeTexture(Texture2D originalTexture, ImageFilterMode filterMode, int newWidth, int newHeight)
        {

            //*** Get All the source pixels
            Color[] sourceColor = originalTexture.GetPixels(0);
            Vector2 sourceSize = new Vector2(originalTexture.width, originalTexture.height);

            //*** Calculate New Size
            float textureWidth = newWidth;
            float textureHeight = newHeight;

            //*** Make New
            Texture2D newTexture = new Texture2D((int)textureWidth, (int)textureHeight, TextureFormat.RGBA32, false);

            //*** Make destination array
            Color[] aColor = new Color[(int)textureWidth * (int)textureHeight];

            Vector2 pixelSize = new Vector2(sourceSize.x / textureWidth, sourceSize.y / textureHeight);

            //*** Loop through destination pixels and process
            Vector2 center = new Vector2();
            for (int i = 0; i < aColor.Length; i++)
            {

                //*** Figure out x&y
                float x = (float)i % textureWidth;
                float y = Mathf.Floor((float)i / textureWidth);

                //*** Calculate Center
                center.x = (x / textureWidth) * sourceSize.x;
                center.y = (y / textureHeight) * sourceSize.y;

                //*** Do Based on mode
                //*** Nearest neighbour (testing)
                if (filterMode == ImageFilterMode.Nearest)
                {

                    //*** Nearest neighbour (testing)
                    center.x = Mathf.Round(center.x);
                    center.y = Mathf.Round(center.y);

                    //*** Calculate source index
                    int sourceIndex = (int)((center.y * sourceSize.x) + center.x);

                    //*** Copy Pixel
                    aColor[i] = sourceColor[sourceIndex];
                }

                //*** Bilinear
                else if (filterMode == ImageFilterMode.Bilinear)
                {

                    //*** Get Ratios
                    float ratioX = center.x - Mathf.Floor(center.x);
                    float ratioY = center.y - Mathf.Floor(center.y);

                    //*** Get Pixel index's
                    int indexTL = (int)((Mathf.Floor(center.y) * sourceSize.x) + Mathf.Floor(center.x));
                    int indexTR = (int)((Mathf.Floor(center.y) * sourceSize.x) + Mathf.Ceil(center.x));
                    int indexBL = (int)((Mathf.Ceil(center.y) * sourceSize.x) + Mathf.Floor(center.x));
                    int indexBR = (int)((Mathf.Ceil(center.y) * sourceSize.x) + Mathf.Ceil(center.x));

                    //*** Calculate Color
                    aColor[i] = Color.Lerp(
                        Color.Lerp(sourceColor[indexTL], sourceColor[indexTR], ratioX),
                        Color.Lerp(sourceColor[indexBL], sourceColor[indexBR], ratioX),
                        ratioY
                    );
                }

                //*** Average
                else if (filterMode == ImageFilterMode.Average)
                {

                    //*** Calculate grid around point
                    int xFrom = (int)Mathf.Max(Mathf.Floor(center.x - (pixelSize.x * 0.5f)), 0);
                    int xTo = (int)Mathf.Min(Mathf.Ceil(center.x + (pixelSize.x * 0.5f)), sourceSize.x);
                    int yFrom = (int)Mathf.Max(Mathf.Floor(center.y - (pixelSize.y * 0.5f)), 0);
                    int yTo = (int)Mathf.Min(Mathf.Ceil(center.y + (pixelSize.y * 0.5f)), sourceSize.y);

                    //*** Loop and accumulate
                    Color tempColor = new Color();
                    float xGridCount = 0;
                    for (int iy = yFrom; iy < yTo; iy++)
                    {
                        for (int ix = xFrom; ix < xTo; ix++)
                        {

                            //*** Get Color
                            tempColor += sourceColor[(int)(((float)iy * sourceSize.x) + ix)];

                            //*** Sum
                            xGridCount++;
                        }
                    }

                    //*** Average Color
                    aColor[i] = tempColor / (float)xGridCount;
                }
            }

            //*** Set Pixels
            newTexture.SetPixels(aColor);
            newTexture.Apply();

            //*** Return
            return newTexture;
        }
    }
}

