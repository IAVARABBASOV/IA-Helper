using System.Security.Cryptography;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace IA.Utils
{
    public static class DataSaveLoad
    {
        private const string hashKey = "IA^_^Secret_KEY"; //recommend too many symbols.

        //Find current path by platform
        public static string GetPath(string fileName)
        {
            string filePath = "File Doesn't Exist";
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                filePath = Application.dataPath + $"/{fileName}.json";
            }
            else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                filePath = Application.persistentDataPath + $"/{fileName}.json";
            }

            return filePath;
        }

        public static void WriteTextIntoFile(string jsonString, string fileName, bool useEncryption = false)
        {
            FileStream fileStream = new FileStream(GetPath(fileName), FileMode.Create);

            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                if (useEncryption)
                {
                    jsonString = Encrypt(jsonString);
                }

                streamWriter.Write(jsonString);
            }
        }
        public static string ReadTextFromFile(string fileName = "CurrentData")
        {
            var text = string.Empty;
            var path = GetPath(fileName);
            if (File.Exists(path))
            {
                using (StreamReader streamReader = new StreamReader(path))
                {
                    text = streamReader.ReadToEnd();

                    text = TryDecrypt(text, out bool isDecrypted);
                }
            }

            return text;
        }


        //Save string to Json
        public static void SaveIntoJsonFile<T>(T classType, string fileName = "")
        {
            if (fileName.IsNullOrWhiteSpace()) return;

            var jsonString = ConvertToJsonText(classType);

            WriteTextIntoFile(jsonString, fileName);
        }

        public static void SaveIntoJsonFile<T>(T classType, string fileName = "", List<string> removedKeywords = null)
        {
            if (fileName.IsNullOrWhiteSpace()) return;

            string jsonString = ConvertToJsonText(classType, (dataObject) =>
            {
                if (removedKeywords != null && removedKeywords.Count > 0)
                {
                    foreach (string keyword in removedKeywords)
                    {
                        // Remove the "targetTier" property from the object, if it exists
                        JProperty targetKeywordProperty = dataObject.Property(keyword);

                        if (targetKeywordProperty != null)
                        {
                            targetKeywordProperty.Remove();
                        }
                    }
                }

                return dataObject;
            });

            WriteTextIntoFile(jsonString, fileName);
        }

        public static void SaveIntoJsonFile<T>(T classType, string fileName = "", Func<JObject, JObject> e = null)
        {
            if (fileName.IsNullOrWhiteSpace()) return;

            string jsonString = ConvertToJsonText(classType, e);

            WriteTextIntoFile(jsonString, fileName);
        }

        public static string ConvertToJsonText<T>(T classType, Func<JObject, JObject> e = null)
        {
            string jsonString = JsonUtility.ToJson(classType);

            if (!jsonString.IsNullOrWhiteSpace() && e != null)
            {
                // Deserialize the JSON data into a dictionary
                JObject dataObject = JObject.Parse(jsonString);

                dataObject = e.Invoke(dataObject);

                jsonString = dataObject.ToString();
            }

            return jsonString;
        }
        public static T ConvertToTargetType<T>(string jsonText)
        {
            var currentItem = JsonUtility.FromJson<T>(jsonText);

            return currentItem;
        }

        //Read data from Json
        public static T ReadFromJson<T>(string fileName = "CurrentData")
        {
            var dataAsJson = ReadTextFromFile(fileName);

            return ConvertToTargetType<T>(dataAsJson);
        }


        //Delete Saved Data on Device
        public static void DeleteFile(string fileName)
        {
            var path = GetPath(fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }


        #region Encode & Decode

        //Encrypting string ("hasKey" must be the same on both)
        public static string Encrypt(string inputData)
        {
            var bData = Encoding.UTF8.GetBytes(inputData);

            var md5 = new MD5CryptoServiceProvider();
            var tripodalDes = new TripleDESCryptoServiceProvider
            {
                Key = md5.ComputeHash(Encoding.UTF8.GetBytes($"$${hashKey}$$")), //Help to make better encrypt
                Mode = CipherMode.ECB // electronic code book --- Encrypting every block
            };
            var transform = tripodalDes.CreateEncryptor();
            var result = transform.TransformFinalBlock(bData, 0, bData.Length);

            return Convert.ToBase64String(result);
        }

        //Decrypting Encrypted string ("hasKey" must be the same on both)
        public static string TryDecrypt(string inputData, out bool isDecryptedData)
        {
            Span<byte> bytes = new Span<byte>(new byte[inputData.Length]);
            isDecryptedData = Convert.TryFromBase64String(inputData, bytes, out int bytesWritten);
            if (isDecryptedData)
            {
                var md5 = new MD5CryptoServiceProvider();
                var tripodalDes = new TripleDESCryptoServiceProvider
                {
                    Key = md5.ComputeHash(Encoding.UTF8.GetBytes($"$${hashKey}$$")), //Help to make better encrypt
                    Mode = CipherMode.ECB // electronic code book --- Encrypting every block
                };
                var transform = tripodalDes.CreateDecryptor();
                var result = transform.TransformFinalBlock(bytes.Slice(0, bytesWritten).ToArray(), 0, bytesWritten);
                return Encoding.UTF8.GetString(result);
            }
            else
            {
                // Handle the conversion failure
                // Return an appropriate value or throw an exception
                // For example:
                //throw new ArgumentException("Invalid input data. Conversion from base64 failed.");

                return inputData;
            }
        }

        #endregion
    }
}