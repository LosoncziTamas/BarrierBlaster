using ArBreakout.Game;
using DG.Tweening;
using UnityEngine;

namespace ArBreakout.Misc
{
    public static class GamePlayUtils
    {
        public static void AnchorBallToPaddle(BallBehaviour ballBehaviour, PaddleBehaviour paddle)
        {
            CenterAboveObject(ballBehaviour.gameObject, paddle.gameObject.transform);
            ballBehaviour.transform.SetParent(paddle.transform.parent);
            ballBehaviour.ResetToDefaults();
            ballBehaviour.ResetPowerUpToDefaults();
            paddle.AnchoredBallBehaviour = ballBehaviour;
        }

        public static void ApplyMagnet(BallBehaviour ballBehaviour, PaddleBehaviour paddle)
        {
            var ballTrans = ballBehaviour.transform;
            var paddleTrans = paddle.transform;

            ballTrans.position = paddleTrans.position + ballTrans.TransformVector(Vector3.forward);
            ballTrans.SetParent(paddleTrans.parent);
            ballBehaviour.ResetToDefaults();
            paddle.AnchoredBallBehaviour = ballBehaviour;
#if false
            DOTween.Sequence().Append(ballTrans.DOMove(paddleTrans.position + ballTrans.TransformVector(Vector3.forward), 0.3f)).AppendCallback(() =>
            {
                ballTrans.SetParent(paddle.transform);
                ballBehaviour.ResetToDefaults();
                paddle.AnchoredBallBehaviour = ballBehaviour;
            });            
#endif
        }
        
        public static void CenterAboveObject(GameObject ballInstance, Transform anchorObject)
        {
            var offset = anchorObject.transform.position - ballInstance.transform.position + ballInstance.transform.TransformVector(Vector3.forward);
            ballInstance.transform.Translate(offset, Space.World);
        }
        
        public static string FormatTime(float timeInSeconds)
        {
            return $"{(int) (timeInSeconds / 60):D2}:{(int) (timeInSeconds % 60):D2}";
        }
    }
}