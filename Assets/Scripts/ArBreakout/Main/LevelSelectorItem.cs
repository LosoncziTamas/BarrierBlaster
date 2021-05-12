using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Main
{
    public class LevelSelectorItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _button;
        
        private Main.LevelSelector.ItemData _data;
                
        public void Init(Main.LevelSelector.ItemData data)
        {
            _data = data;
            _text.text = data.levelText;
            _button.interactable = data.unlocked;
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            _data?.onClick.Invoke(_data);
        }
    }
}