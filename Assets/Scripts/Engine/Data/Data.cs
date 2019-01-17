
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


namespace Engine
{
    [Serializable]
    public class Data
    {
        public static event Action Loaded;
        public static event Action Saved;
        private static Dictionary<string, Data> datas = new Dictionary<string, Data>();
        
        public string id;

        /// <summary>
        /// Saving serializable object to data
        /// </summary>
        /// <param name="fileName">Name of the file to save</param>
        /// <param name="data">Object to be serialized must be Serializable</param>
        /// <returns></returns>
        public static bool Save(string fileName, object data, bool useFullPath = false)
        {
            bool saved = false;
            FileStream file = null;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                string path = "";
                if (useFullPath)
                    path = fileName;
                else
                    path = Application.persistentDataPath + string.Format("/{0}", fileName);
                file = File.Create(path);
                bf.Serialize(file, data);
            }
            catch (Exception ex)
            {
                saved = false;
                Debug.LogError(ex.Message);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    saved = true;
                }
                else
                {
                    saved = false;
                }
            }
            Saved?.Invoke();
            if (saved)
            {
                Debug.Log("Save successful");
            }
            else
            {
                Debug.LogError("Didn't save");
            }
            return saved;
        }

        public static bool SaveByte(string fileName, object data, bool useFullPath = false)
        {
            bool saved = false;
            FileStream file = null;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                string path = useFullPath ? fileName : Application.persistentDataPath + string.Format("/{0}", fileName);
                file = File.Create(path);
                bf.Serialize(file, data);
            }
            catch (Exception ex)
            {
                saved = false;
                Debug.LogError(ex.Message);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    saved = true;
                }
                else
                {
                    saved = false;
                }
            }
            Saved?.Invoke();
            return saved;
        }

        /// <summary>
        /// Loading object from file, remember that it has to be the same type that was saved to a file
        /// </summary>
        /// <typeparam name="T">Object's class</typeparam>
        /// <param name="fileName">file to load from</param>
        /// <returns>Returns an object loaded from file</returns>
        public static object Load(string fileName, bool useFullPath = false)
        {
            object data = null;
            FileStream file = null;
            try
            {
                string path = "";
                if (useFullPath)
                    path = fileName;
                else
                    path = Application.persistentDataPath + string.Format("/{0}", fileName);

                if (File.Exists(path))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    file = File.Open(path, FileMode.Open);
                    data = bf.Deserialize(file);
                    file.Close();
                }
                else
                {
                    Debug.Log("Path doesn't exist: " + path);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
            Loaded?.Invoke();
            return data;
        }

        public static object DeserializeFile(string file)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(GenerateStreamFromString(file));
        }

        public static object DesirializeFile(byte[] bytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return bf.Deserialize(new MemoryStream(bytes));
        }

        public static string SerializeJSON<T>(T obj)
        {
            //return JsonUtility.ToJson(obj);
            return JsonHelper.ToJson<T>(obj);
        }


        public static T DeserializeJSON<T>(string str)
        {
            //return JsonUtility.FromJson<T>(str);
            return JsonHelper.FromJson<T>(str);
        }


        static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }



    [Serializable]
    public class STransform
    {
        public Vector LocalPosition { get; set; }
        public Vector LocalScale { get; set; }
        public Vector LocalRotation { get; set; }

        public static implicit operator STransform(Transform trans)
        {
            return new STransform()
            {
                LocalPosition = trans.localPosition,
                LocalScale = trans.localScale,
                LocalRotation = trans.localRotation
            };
        }
    }

    public interface IData
    {
        string ID { get; }
    }

    public static class JsonHelper
    {
        public static T FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T Items;
        }
    }

}