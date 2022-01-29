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
            Controller.TransitionTo(typeof(GameAppState));
        }
    }
}