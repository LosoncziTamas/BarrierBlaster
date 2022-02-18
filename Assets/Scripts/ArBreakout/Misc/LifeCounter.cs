using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace ArBreakout.Misc
{
    public class LifeCounter : MonoBehaviour
    {
        [SerializeField] private Image[] _hearts;
        [SerializeField] private Sprite _filledHeart;
        [SerializeField] private Sprite _emptyHeart;

        public void UpdateLives(int lifeCount)
        {
            Assert.IsTrue(lifeCount > -1 && lifeCount <= _hearts.Length,
                $"Invalid life count: {lifeCount}. Available: {_hearts.Length}");

            HideAll();
            for (var i = 0; i < lifeCount; i++)
            {
                _hearts[i].sprite = _filledHeart;
            }
        }

        private void HideAll()
        {
            foreach (var heart in _hearts)
            {
                heart.sprite = _emptyHeart;
            }
        }
    }
}