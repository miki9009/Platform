using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine
{
    public class DataProperty<T>
    {
        string key;
        object val;
        bool changed = false;

        Action<object> Setter;

        DataProperty(string key, int val)
        {
            this.key = key;
            if(PlayerPrefs.HasKey(key))
            {
                this.val = PlayerPrefs.GetInt(key);
            }
            else
            {
                PlayerPrefs.SetInt(key, val);
                this.val = val;
            }
            Setter = SetInt;
        }

        DataProperty(string key, float val)
        {
            this.key = key;
            if (PlayerPrefs.HasKey(key))
            {
                this.val = PlayerPrefs.GetFloat(key);
            }
            else
            {
                PlayerPrefs.SetFloat(key, val);
                this.val = val;
            }
            Setter = SetFloat;
        }

        DataProperty(string key, string val)
        {
            this.key = key;
            if (PlayerPrefs.HasKey(key))
            {
                this.val = PlayerPrefs.GetString(key);
            }
            else
            {
                PlayerPrefs.SetString(key, val);
                this.val = val;
            }
            Setter = SetString;
        }

        DataProperty(string key, bool val)
        {
            this.key = key;
            if (PlayerPrefs.HasKey(key))
            {
                val = PlayerPrefs.GetInt(key) > 0;
            }
            else
            {
                int a = val ? 1 : 0; 
                PlayerPrefs.SetInt(key, a);
            }
            this.val = val;
            Setter = SetBool;
        }

        DataProperty(string key, T val)
        {
            this.key = key;
            string temp;
            if (PlayerPrefs.HasKey(key))
            {
                temp = PlayerPrefs.GetString(key);
            }
            else
            {
                temp = Data.SerializeJSON(val);
                PlayerPrefs.SetString(key, temp);
            }
            Console.WriteLine(temp, Console.LogColor.Red);
            this.val = Data.DeserializeJSON<T>(temp);
            Setter = SetObject;
        }

        DataProperty(string key, T[] val)
        {
            this.key = key;
            string temp;
            if (PlayerPrefs.HasKey(key))
            {
                temp = PlayerPrefs.GetString(key);
            }
            else
            {
                temp = Data.SerializeJSON(val);
                PlayerPrefs.SetString(key, temp);
            }
            Console.WriteLine(temp, Console.LogColor.Red);
            this.val = Data.DeserializeJSON<T>(temp);
            Setter = SetObject;
        }

        public T Value
        {
            get
            {
                return (T)val;
            }
            set
            {
                val = value;
                Setter(value);
            }
        }

        void SetObject(object val)
        {
            var temp = Data.SerializeJSON(val);
            PlayerPrefs.SetString(key, temp);
        }

        void SetString(object val)
        {
            PlayerPrefs.SetString(key, (string)val);
        }

        void SetInt(object val)
        {
            PlayerPrefs.SetInt(key, (int)val);
        }

        void SetFloat(object val)
        {
            PlayerPrefs.SetFloat(key, (float)val);
        }

        void SetBool(object val)
        {
            PlayerPrefs.SetInt(key, (bool)val ? 1 : 0);
        }

        public static DataProperty<int> Get(string key, int defaultValue)
        {
            if (string.IsNullOrEmpty(key)) Debug.LogError("Key is empty");
            return new DataProperty<int>(key, defaultValue);
        }

        public static DataProperty<float> Get(string key, float defaultValue)
        {
            if (string.IsNullOrEmpty(key)) Debug.LogError("Key is empty");
            return new DataProperty<float>(key, defaultValue);
        }

        public static DataProperty<string> Get(string key, string defaultValue)
        {
            if (string.IsNullOrEmpty(key)) Debug.LogError("Key is empty");
            return new DataProperty<string>(key, defaultValue);
        }

        public static DataProperty<bool> Get(string key, bool defaultValue)
        {
            if (string.IsNullOrEmpty(key)) Debug.LogError("Key is empty");
            return new DataProperty<bool>(key, defaultValue);
        }

        public static DataProperty<T> Get(string key, T defaultValue) 
        {
            if (string.IsNullOrEmpty(key)) Debug.LogError("Key is empty");
            return new DataProperty<T>(key, defaultValue);
        }

        public static DataProperty<T[]> Get(string key, T[] defaultValue)
        {
            if (string.IsNullOrEmpty(key)) Debug.LogError("Key is empty");
            return new DataProperty<T[]>(key, defaultValue);
        }
    }
}