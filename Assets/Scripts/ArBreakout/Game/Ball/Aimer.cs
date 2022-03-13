using UnityEngine;

namespace ArBreakout.Game.Ball
{
    public class Aimer : MonoBehaviour
    {
        private Transform _aimerObject;
        
        private void FixedUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var info))
                {
                    var offset = info.point - transform.position;
                    Debug.DrawLine(transform.position, info.point);
                }
            }
        }
    }
}