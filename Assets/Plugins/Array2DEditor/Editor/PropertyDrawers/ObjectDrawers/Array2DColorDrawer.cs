using System;
using UnityEditor;
using UnityEngine;

namespace Array2DEditor
{
    [CustomPropertyDrawer(typeof(Array2DColor))]
    public class Array2DColorDrawer : Array2DDrawer
    {
        private static readonly Color DefaultColor = new Color(0, 0, 0, 0);
        
        protected override object GetDefaultCellValue() => DefaultColor;

        protected override object GetCellValue(SerializedProperty cell) => cell.colorValue;

        protected override void SetValue(SerializedProperty cell, object obj)
        {
            cell.colorValue = (Color) obj;
        }
    }
}