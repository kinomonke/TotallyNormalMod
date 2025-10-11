using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TotallyNormalMod.Tools
{
    public class Tools 
    {

        private static readonly Dictionary<string, GameObject> objectPool = new Dictionary<string, GameObject>();
        public static GameObject GetObject(string find)
        {
            if (objectPool.TryGetValue(find, out GameObject go))
                return go;

            GameObject tgo = GameObject.Find(find);
            if (tgo != null)
                objectPool.Add(find, tgo);

            return tgo;
        }
    }
}
