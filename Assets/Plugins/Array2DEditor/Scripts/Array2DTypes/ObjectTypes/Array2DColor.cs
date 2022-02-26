using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DColor : Array2D<Color>
    {
        [SerializeField]
        CellRowColor[] cells = new CellRowColor[Consts.defaultGridSize];

        protected override CellRow<Color> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }
}