using ArBreakout.Game;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Misc
{
    public static class GamePlayUtils
    {
        public static void AnchorBallToPaddle(BallBehaviour ballBehaviour, PaddleBehaviour paddle)
        {
            CenterAboveObject(ballBehaviour.transform, paddle.gameObject.transform);
            ballBehaviour.transform.SetParent(paddle.transform.parent);
            ballBehaviour.ResetToDefaults();
            ballBehaviour.ResetPowerUpToDefaults();
            paddle.SetBall(ballBehaviour);
        }

        public static void ApplyMagnet(BallBehaviour ballBehaviour, PaddleBehaviour paddle)
        {
            var ballTrans = ballBehaviour.transform;
            var paddleTrans = paddle.transform;

            ballTrans.position = paddleTrans.position + ballTrans.TransformVector(Vector3.forward);
            ballTrans.SetParent(paddleTrans.parent);
            ballBehaviour.ResetToDefaults();
            paddle.SetBall(ballBehaviour);
        }

        public static void CenterAboveObject(Transform target, Transform anchorObject)
        {
            var offset = anchorObject.transform.position - target.position + target.TransformVector(Vector3.forward);
            target.Translate(offset, Space.World);
        }

        public static string FormatTime(float timeInSeconds)
        {
            return $"{(int) (timeInSeconds / 60):D2}:{(int) (timeInSeconds % 60):D2}";
        }
    }
}