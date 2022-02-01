﻿using System;
using UnityEngine;

namespace Array2DEditor
{
    public class ExampleScript2 : MonoBehaviour
    {
        [SerializeField]
        private Array2DInt arrayInt;

        [SerializeField]
        private Array2DExampleEnum arrayEnum;

        [SerializeField]
        private Array2DBool arrayBool;

        [SerializeField]
        private Array2DFloat arrayFloat;

        [SerializeField]
        private Array2DString arrayString;
        
        [SerializeField]
        private Array2DString arrayString2;
        
        [SerializeField]
        private Array2DChar arrayChar;

        private void Awake()
        {
        }

        private void OnGUI()
        {
            if (GUILayout.Button("print"))
            {
                Debug.Log(arrayChar.GetCell(0, 0));
            }
        }
    }
}