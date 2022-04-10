using System.Collections.Generic;
using DG.Tweening;
using Possible;
using Possible.Scheduling;
using Possible.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArBreakout.Misc
{
    [RequireComponent(typeof(RectTransform))]
    public class UIMessageController : DisposableManager<UIMessageController>
    {
        private struct Message
        {
            public readonly string text;
            public readonly float duration;
            public readonly float yPosition;

            public Message(string text, float duration, float yPosition)
            {
                this.text = text;
                this.duration = duration;
                this.yPosition = yPosition;
            }
        }

        [SerializeField] private TextMeshProUGUI _messagePrefab;
        private RectTransform _rectTransform;

        // Queue for scheduled messages
        private readonly Queue<Message> _messageQueue = new();

        // Reference to the currently displayed message
        private TextMeshProUGUI _currentMessageInstance;

        // State variable to indicate whether the screen is occupied for a definite amount of time
        private bool _isScreenOccupied;

        protected override void Awake()
        {
            base.Awake();
            _rectTransform = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Displays a message on the screen. If a message is already present, schedules the specified one to be displayed later.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="stayForSeconds">The message will stay on the screen for this amount of seconds. If less than zero, the message stays indefinitely.</param>
        /// <param name="yPosition">The y position of the text on screen to animate to.</param>
        public void DisplayMessage(string message, float stayForSeconds = -1, float yPosition = 100.0f)
        {
            if (_isScreenOccupied)
            {
                _messageQueue.Enqueue(new Message(message, stayForSeconds, yPosition));
                return;
            }

            if (_currentMessageInstance != null)
            {
                FadeOutAndDestroy(_currentMessageInstance);
            }

            _currentMessageInstance = Instantiate(_messagePrefab, parent: _rectTransform);
            if (_currentMessageInstance == null)
            {
                UnityEngine.Debug.LogError("[UIMessageController] Could not instantiate message prefab.");
                return;
            }

            var msg = new Message(message, stayForSeconds, yPosition);

            // Flag the screen as occupied if the message will stay on the screen for a definite amount of time
            _isScreenOccupied = msg.duration > 0.0f;

            if (!_isScreenOccupied)
            {
                while (_messageQueue.Count > 0 && !_isScreenOccupied)
                {
                    msg = _messageQueue.Dequeue();
                    _isScreenOccupied = msg.duration > 0.0f;
                }
            }

            // Initialize message
            _currentMessageInstance.text = msg.text;

            // Animate the message in
            FadeIn(_currentMessageInstance, msg.yPosition);

            if (_isScreenOccupied)
            {
                var messageInstance = _currentMessageInstance;
                Wait.For(seconds: msg.duration).ThenDo(() =>
                {
                    // Animate the message out and destroy)
                    FadeOutAndDestroy(messageInstance);

                    // Reset the flag
                    _isScreenOccupied = false;

                    // Displayed the next message in the queue, if there is any
                    if (_messageQueue.Count > 0)
                    {
                        var nextMessage = _messageQueue.Dequeue();
                        DisplayMessage(nextMessage.text, nextMessage.duration, nextMessage.yPosition);
                    }
                }).StartOn(this);
            }
        }

        /// <summary>
        /// Hides the currently displayed message and removes the previously scheduled ones from the queue.
        /// </summary>
        public void ClearMessages(bool animate = true)
        {
            if (_currentMessageInstance != null)
            {
                if (animate)
                {
                    FadeOutAndDestroy(_currentMessageInstance);
                }
                else
                {
                    Destroy(_currentMessageInstance);
                }

                _currentMessageInstance = null;
            }

            _messageQueue.Clear();
            _isScreenOccupied = false;
        }

        private static void FadeIn(Graphic text, float yPosition)
        {
            var rectTransform = text.rectTransform;
            rectTransform.Reset();
            rectTransform.anchoredPosition = Vector3.down * 100.0f;
            rectTransform.DOAnchorPosY(endValue: yPosition, duration: 1.0f).SetEase(Ease.OutCubic);
            DOTween.ToAlpha(() => text.color, color => text.color = color, 1.0f, duration: 0.5f);
        }

        private static void FadeOutAndDestroy(Graphic text)
        {
            if (text == null)
            {
                UnityEngine.Debug.LogWarning(
                    "[UIMessageController] Tried to fade out a message that no longer exists. Possibly because ClearMessages() was called before.");
                return;
            }

            var rectTransform = text.rectTransform;
            rectTransform.DOKill();
            rectTransform.DOAnchorPosY(endValue: -100.0f, duration: 1.0f)
                .SetEase(Ease.InCubic);
            DOTween.ToAlpha(() => text.color, (color) => text.color = color, 0.0f, duration: 1.0f);
            Destroy(text.gameObject, 1.1f);
        }
    }
}