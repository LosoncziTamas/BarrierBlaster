using System.Collections.Generic;
using UnityEngine;

namespace ArBreakout.Gui.LevelSelector
{
    public class Stars : MonoBehaviour
    {
        [SerializeField] private List<Star> _stars;
        
        public void SetFilledCount(int filledCount)
        {
            for (var i = 0; i < _stars.Count; i++)
            {
                _stars[i].SetFilled(i < filledCount);
            }
        }
    }
}