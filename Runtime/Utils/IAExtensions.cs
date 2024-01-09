using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

namespace IA.Utils
{
    public static class IAExtensions
    {
        public static bool AreThisStringEqualTo(this string thisString, string otherString) =>
         string.Equals(thisString, otherString, StringComparison.OrdinalIgnoreCase);

        public static bool IsNullOrWhiteSpace(this String value)
        {
            if (value == null || value.Length == 0) return true;

            foreach (char targetChar in value)
            {
                if (!Char.IsWhiteSpace(targetChar)) return false;
            }

            return true;
        }

        public static void DestroyList<T>(this List<T> list) where T : UnityEngine.Object
        {
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    GameObject.Destroy(item);
                }

                list.Clear();
            }
        }

        public static double CalculateMedian(this int[] values)
        {
            int n = values.Length;
            Array.Sort(values);
            if (n % 2 == 0)
            {
                // if the array has an even number of elements, take the average of the middle two values
                return (double)(values[n / 2 - 1] + values[n / 2]) / 2.0;
            }
            else
            {
                // if the array has an odd number of elements, return the middle value
                return (double)values[n / 2];
            }
        }

        public static bool IsNull<T>(this T source) where T : struct
        {
            return source.Equals(default(T));
        }

        public static Vector3 GetRandomValue(this Vector3 value, float range)
        {
            return new Vector3(Random.Range(value.x, range), Random.Range(value.y, range), Random.Range(value.z, range));
        }

        public static Vector3 GetRandomValue(this Vector3 value, float min, float max)
        {
            return new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
        }

        public static Vector3 GetRandomValue(this Vector3 min, Vector3 max)
        {
            return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
        }

        public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
        {
            return new Vector3(
                Mathf.Clamp(value.x, min.x, max.x),
                Mathf.Clamp(value.y, min.y, max.y),
                Mathf.Clamp(value.z, min.z, max.z));
        }

        public static Vector3 Abs(this Vector3 value)
        {
            return new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));
        }

        public static T GetRandomItem<T>(this T[] currentArray) =>
        currentArray[0.RandomInt(currentArray.Length)];

        public static T GetRandomItem<T>(this List<T> currentList) =>
        currentList[0.RandomInt(currentList.Count)];

        public static T GetRandomItem<T>(this List<T> currentList, Func<T, bool> characteristic)
        {
            if (currentList.Count == 0) return default;

            // create current item list
            T[] currentItemList = new T[currentList.Count];

            currentList.CopyTo(currentItemList);

            T currentItem = default;
            // get random item in list
            T randomItem = GetRandomItem(currentItemList.ToList());

            // get item if item is correct
            bool hasValue = characteristic.Invoke(randomItem);

            if (hasValue)
            {
                return randomItem;
            }
            else
            {
                List<T> newItems = currentItemList.ToList();
                newItems.Remove(randomItem);
                currentItem = GetRandomItem(newItems, characteristic);
            }

            return currentItem;
        }

        /// <summary>
        /// Get Random Int Value
        /// </summary>
        /// <param name="minValue">Selected int is Min value</param>
        /// <param name="maxValue">Max value</param>
        /// <returns></returns>
        public static int RandomInt(this int minValue, int maxValue) => UnityEngine.Random.Range(minValue, maxValue);

        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }

        public static T RandomEnumValue<T>()
        {
            Array value = Enum.GetValues(typeof(T));

            int randomValueIndex = 0.RandomInt(value.Length);

            return (T)value.GetValue(randomValueIndex);
        }

        /// <summary>
        /// Get Random Enum Value
        /// </summary>
        /// <typeparam name="T">Out Value</typeparam>
        /// <param name="luckyType">Choose lucky type of enum for define return chance</param>
        /// <param name="chanceFactor">Choose value between 0.0f - 1.0f, default is 0.5f</param>
        /// <returns></returns>
        public static T RandomEnumValue<T>(T luckyType, float chanceFactor = 0.5f)
        {
            Array value = Enum.GetValues(typeof(T));

            int randomValueIndex = Random.Range(0, value.Length);

            T outputValue = Random.value <= chanceFactor ? luckyType : (T)value.GetValue(randomValueIndex);

            return outputValue;
        }

        /// <summary>
        /// Create a List and Fill it with item
        /// </summary>
        /// <typeparam name="T">List item Type</typeparam>
        /// <param name="count">How much fill ?</param>
        /// <param name="item">What item add ?</param>
        /// <returns>New List</returns>
        public static List<T> CreateList<T>(int count, System.Func<T> item)
        {
            List<T> currentList = new List<T>();

            for (int i = 0; i < count; i++)
            {
                currentList.Add(item.Invoke());
            }

            return currentList;
        }

        public static void ClearEmptyItems<T>(this List<T> _currentList, out List<T> _result)
        {
            List<T> tempList = new List<T>();

            if (_currentList.Count > 0)
            {
                _currentList.ForEach(x =>
                {
                    if (!x.Equals(null) && !x.Equals(default))
                    {
                        tempList.Add(x);
                    }
                });
            }

            _result = tempList;
        }

        public static void SetupToggle(this Toggle _targetToggle, UnityAction<bool> e, bool defaultValue = true)
        {
            _targetToggle.SetIsOnWithoutNotify(defaultValue);
            _targetToggle.onValueChanged.RemoveAllListeners();
            _targetToggle.onValueChanged.AddListener((isOn) =>
            {
                e.Invoke(isOn);
            });
        }

        public static int GetRandomWithoutThisValue(this int withoutThisValue, int min, int max)
        {
            List<int> values = new List<int>();

            for (int i = min; i < max; i++)
            {
                if (i != withoutThisValue)
                {
                    values.Add(i);
                }
            }

            return values.GetRandomItem();
        }

        /// <summary>
        /// Create Raycast on Ray Origin and Direction
        /// </summary>
        /// <param name="ray">Defined Ray</param>
        /// <param name="hitColor">Ray Color on DrawRay</param>
        /// <param name="distance">Ray Distance</param>
        /// <param name="hitMask">Ray Hit Object Layer</param>
        /// <returns>Ray Hit Point and Reflect Direction</returns>
        public static Ray CreateReflection(this Ray ray, RayReflectionProperty reflectionProperty, UnityAction<RaycastHit> hitInfoCallback = null)
        {
            Vector3 hitPoint = Vector3.zero;
            Vector3 reflectDirection = Vector3.zero;

            if (Physics.Raycast(ray, out RaycastHit hitInfo, reflectionProperty.RayDistance, reflectionProperty.HitMask))
            {
                hitPoint = hitInfo.point;

                hitInfoCallback?.Invoke(hitInfo);

                /// Ray: Reflect from Hitted Object
                reflectDirection = Vector3.Reflect(hitInfo.point - ray.origin, hitInfo.normal);

                if (reflectionProperty.ReflectDirectionReturnHandler != null)
                {
                    reflectDirection = reflectionProperty.ReflectDirectionReturnHandler.Invoke(reflectDirection);
                }

                if (reflectionProperty.HitColor != default)
                {
                    /// Ray: Ball to Hitted Object
                    Debug.DrawRay(ray.origin, (hitInfo.point - ray.origin), reflectionProperty.HitColor);
                }
            }

            return new Ray(hitPoint, reflectDirection);
        }

        public static Ray CreateSphereCastReflection(this Ray ray, SphereCastReflectionProperty _property,
            UnityAction<RaycastHit, Vector3> onHit = null,
            UnityAction<Vector3> dontHit = null,
            Func<RaycastHit, Vector3, Vector3> reflect = null)
        {
            Vector3 hitPoint = Vector3.zero;
            Vector3 reflectDirection = Vector3.zero;
            Vector3 sphereCastMidpoint;

            if (Physics.SphereCast(ray, _property.Radius,
                out RaycastHit _hit, _property.Distance, _property.HitMask))
            {
                hitPoint = _hit.point;

                sphereCastMidpoint = ray.origin + (ray.direction * _hit.distance);
                onHit?.Invoke(_hit, sphereCastMidpoint);

                reflectDirection = Vector3.Reflect(_hit.point - ray.origin, _hit.normal);
            }
            else
            {
                sphereCastMidpoint = ray.origin + (ray.direction * (_property.Distance - _property.Radius));
                dontHit?.Invoke(sphereCastMidpoint);
            }

            return new Ray(hitPoint, reflectDirection);
        }

        /// <summary>
        /// Start Coroutine on Temporary Monobehaviour
        /// </summary>
        /// <param name="ienum">Current IEnumerator</param>
        /// <returns>Coroutine</returns>
        public static Coroutine StartCoroutine(this IEnumerator ienum)
        {
            GameObject coroutineStarterGO = new GameObject("Coroutine Starter");

            CoroutineStarter coroutineStarter = coroutineStarterGO.AddComponent<CoroutineStarter>();

            Coroutine coroutine = coroutineStarter.Setup(ienum).StartCoroutine(coroutineStarter);

            return coroutine;
        }

        /// <summary>
        /// Start Coroutine on Target MonoBehaviour
        /// </summary>
        /// <param name="ienum">Current IEnumerator</param>
        /// <param name="coroutineStarter">Target Monobehaviour</param>
        /// <returns>Coroutine</returns>
        public static Coroutine StartCoroutine(this IEnumerator ienum, MonoBehaviour coroutineStarter)
        {
            if (coroutineStarter.gameObject.activeInHierarchy)
            {
                return coroutineStarter.StartCoroutine(ienum);
            }
            else return null;
        }

        /// <summary>
        /// WaitForSeconds, WaitForEndOfFrame or other YieldInstruction Classes
        /// </summary>
        /// <param name="yield"></param>
        /// <param name="e">Do Something</param>
        /// <returns>IEnumerator</returns>
        public static IEnumerator EventRoutine(this YieldInstruction yield, UnityAction e)
        {
            yield return yield;
            e.Invoke();
        }

        /// <summary>
        /// WaitUntil or other CustomYieldInstruction Classes
        /// </summary>
        /// <param name="yield"></param>
        /// <param name="e">Do Something</param>
        /// <returns>IEnumerator</returns>
        public static IEnumerator EventRoutine(this CustomYieldInstruction yield, UnityAction e)
        {
            yield return yield;
            e.Invoke();
        }

        /// <summary>
        /// This method cut the long values of Float
        /// Ex: 1,0512884
        /// Return: 1,05
        /// </summary>
        /// <param name="value">In</param>
        /// <returns></returns>
        public static float CutLongValues(this float value, int length = 2)
        {
            string floatAsString = value.ToString($"F{length}");

            float result = float.Parse(floatAsString);

            return result;
        }

        public static Vector3 CutLongValues(this Vector3 value, int length = 2)
        {
            value.x = value.x.CutLongValues(length);
            value.y = value.y.CutLongValues(length);
            value.z = value.z.CutLongValues(length);

            return value;
        }

        public static bool IsDublicatePressed(this Event e)
        {
            if (e != null)
            {
                if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.D)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsUndoPressed(this Event e)
        {
            if (e != null)
            {
                if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Z)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class CoroutineStarter : MonoBehaviour
    {
        public IEnumerator Setup(IEnumerator enumerator)
        {
            yield return enumerator;

            DestroyMe();
        }

        private void DestroyMe()
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}