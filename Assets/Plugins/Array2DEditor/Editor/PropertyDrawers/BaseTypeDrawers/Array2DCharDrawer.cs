using System;
using UnityEditor;

namespace Array2DEditor
{
    [CustomPropertyDrawer(typeof(Array2DChar))]
    public class Array2DCharDrawer : Array2DDrawer
    {
        protected override object GetDefaultCellValue() => 0;

        protected override object GetCellValue(SerializedProperty cell) => cell.stringValue;

        protected override void SetValue(SerializedProperty cell, object obj)
        {
            cell.stringValue = obj.ToString();
        }
    }
}