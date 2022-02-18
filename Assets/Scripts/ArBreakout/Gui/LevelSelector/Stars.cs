using System.Collections.Generic;
using UnityEngine;

namespace ArBreakout.Gui.LevelSelector
{
    public class Stars : MonoBehaviour
    {
        [SerializeField] private List<Star> _stars;

        private void Start()
        {
            _stars[0].SetFilled(true);
            _stars[1].SetFilled(false);
            _stars[2].SetFilled(false);
        }
    }
}