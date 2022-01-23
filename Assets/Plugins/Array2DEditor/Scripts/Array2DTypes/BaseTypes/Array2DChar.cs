using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DChar : Array2D<char>
    {
        [SerializeField]
        CellRowChar[] cells = new CellRowChar[Consts.defaultGridSize];

        protected override CellRow<char> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }
}