using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public struct DebugData
    {
        public string Label;
        public string Data;
    }
    
    public class DebugUI : MonoBehaviour
    {
        public DebugUIElement Element;

        private static DebugUIElement _prefab;
        private static Transform _elementPeretnt;

        private List<DebugData> _debugsData = new List<DebugData>();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _prefab = Element;
            _elementPeretnt = transform.GetChild(0);
        }

        public static void OnDataChange(string lable, ref Action<string> onChange)
        {
            var elem = Instantiate(_prefab, _elementPeretnt);
            elem.Lable.text = lable;
            onChange += elem.OnDataChange;
        }
    }
}