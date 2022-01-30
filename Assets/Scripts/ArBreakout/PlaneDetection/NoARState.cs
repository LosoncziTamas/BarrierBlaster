using ArBreakout.SinglePlayer;
using Possible.AppController;
using UnityEngine;

namespace ArBreakout.PlaneDetection
{
    public class NoARState : AppState
    {
        [SerializeField] private Transform _levelParent;

        public override void OnEnter(AppState fromState)
        {
            base.OnEnter(fromState);
            if (!_levelParent)
            {
                var levelParent = new GameObject("LevelParent");
            }
            Controller.TransitionTo(typeof(GameAppState));
        }
        
    }
}