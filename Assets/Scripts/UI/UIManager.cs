using System.Text;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textOverlay;
        [SerializeField] private int _maxScore;

        [Header("Score Game Events")]
        [SerializeField] GameEvent PointScored;

        private int _score;
        private static string _mCurrentText;
        private static VehicleStatus _mCurrentStatus;

        public enum VehicleStatus
        {
            Idle,
            Accelerate,
            Reverse,
            Pickup
        }

        private void Start()
        {
            if (_textOverlay == null)
            {
                _textOverlay = GetComponent<TextMeshProUGUI>();
            }

            _mCurrentStatus = VehicleStatus.Idle;

            UpdateScore(0);
        }

        private void Update()
        {
            if (_textOverlay)
            {
                _mCurrentText = _textOverlay.text;
            }
        }

        public void UpdateTextDisplay(string text, bool enableAnim = true)
        {
            if (!_textOverlay) return;

            var stringBuilder = new StringBuilder(500);

            stringBuilder.AppendLine(text);

            _textOverlay.text = stringBuilder.ToString();
            Animator animator = _textOverlay.gameObject.GetComponent<Animator>();
            animator.enabled = enableAnim;
        }

        public void ResetText(string text)
        {
            if (!_textOverlay) return;

            if (_textOverlay.text != text) return;

            _textOverlay.text = " ";

            Animator animator = _textOverlay.gameObject.GetComponent<Animator>();
            animator.enabled = false;
        }

        public static string GetCurrentText()
        {
            return _mCurrentText;
        }

        public static VehicleStatus GetCurrentStatus()
        {
            return _mCurrentStatus;
        }

        public static void SetCurrentStatus(VehicleStatus status)
        {
            _mCurrentStatus = status;
        }

        public void UpdateScore(int newScore)
        {
            if (!gameObject.CompareTag("Score")) return;

            _score += newScore;
            _textOverlay.text = $" {_score} / {_maxScore}";

            if (_score > 0)
            {
                PointScored?.Invoke();
            }
        }

        public int GetScore()
        {
            return _score;
        }
    }
}
