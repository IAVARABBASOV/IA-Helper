using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IA.Utils
{
    public class IAScreenShot : MonoBehaviour
    {
        public static Camera ScreenShotCamera;

        /*    public static void TakeScreenShot(string path, int width, int height,string screenShotName = "", int scale = 1, bool isTransparent = false)
            {
                if (ScreenShotCamera == null) ScreenShotCamera = Camera.main;

                int resWidth = width * scale;
                int resHeight = height * scale;

                RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
                ScreenShotCamera.targetTexture = rt;

                TextureFormat tFormat = isTransparent ? TextureFormat.ARGB32 : TextureFormat.RGB24;

                Texture2D screenShot = new Texture2D(resWidth, resHeight, tFormat, false);
                ScreenShotCamera.Render();

                RenderTexture.active = rt;
                screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
                ScreenShotCamera.targetTexture = null;
                RenderTexture.active = null;
                byte[] bytes = screenShot.EncodeToPNG();
                string filename = ScreenShotName(resWidth, resHeight, path);

                Debug.LogError("filename: " + filename);

                System.IO.File.WriteAllBytes(filename, bytes);
                Debug.Log(string.Format("Took screenshot to: {0}", filename));
                Application.OpenURL(filename);
            }*/

#if UNITY_EDITOR

        public static IEnumerator TakeScreenShot(string screenShotName, int width, int height, int scale = 1, bool isTransparent = false, TextureImporterType textureImporterType = TextureImporterType.Default, bool isShowImage = true)
        {
            if (ScreenShotCamera == null) ScreenShotCamera = Camera.main;

            int resWidth = width * scale;
            int resHeight = height * scale;

            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            ScreenShotCamera.targetTexture = rt;

            TextureFormat tFormat = isTransparent ? TextureFormat.ARGB32 : TextureFormat.RGB24;

            Texture2D screenShot = new Texture2D(resWidth, resHeight, tFormat, false);
            ScreenShotCamera.Render();

            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            ScreenShotCamera.targetTexture = null;
            RenderTexture.active = null;
            byte[] bytes = screenShot.EncodeToPNG();
            string ssPath = ScreenShotPath(screenShotName);

            Debug.LogError("filename: " + ssPath);

            System.IO.File.WriteAllBytes(ssPath, bytes);

            if (isShowImage)
            {
                AssetDatabase.Refresh();
                yield return new WaitForSecondsRealtime(0.5f);

                string texturePath = GetTexturePath(screenShotName);

                Debug.LogError("texturePath: " + texturePath);

                TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;

                if (importer == null)
                {
                    Debug.LogError($"{texturePath} is not correct. File Not Found");
                }
                else
                {
                    importer.textureType = textureImporterType;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.SaveAndReimport();
                }
                Debug.Log(string.Format("Took screenshot to: {0}", ssPath));
                Application.OpenURL(ssPath);
            }
        }

        private static string GetTexturePath(string name)
        {
            string strPath = "";

            strPath = $"Assets{name}";

            return strPath;
        }

        public static string ScreenShotPath(string name)
        {
            string strPath = "";

            strPath = Application.dataPath + name;

            return strPath;
        }

#endif
    }
}