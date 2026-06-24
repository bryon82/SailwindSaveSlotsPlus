using UnityEngine;
using static SaveSlotsPlus.SlotPath;
using static SaveSlotsPlus.SSP_Plugin;
using static SaveSlotsPlus.Configs;

namespace SaveSlotsPlus
{
    internal class SaveNameInput : MonoBehaviour
    {
        private const int MAX_LENGTH = 13;
        private const float CURSOR_BLINK_RATE = 0.5f;

        internal TextMesh displayMesh;
        private string _currentText = "";
        private int _cursorPos = 0;
        private float _cursorTimer = 0f;
        private bool _showCursor = true;
        private bool _isActive = false;

        private void Update()
        {
            if (!_isActive) 
                return;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _cursorPos = Mathf.Max(0, _cursorPos - 1);
                ResetCursorBlink();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _cursorPos = Mathf.Min(_currentText.Length, _cursorPos + 1);
                ResetCursorBlink();
            }

            foreach (char c in Input.inputString)
            {
                if (c == '\b')
                {                    
                    if (_cursorPos > 0)
                    {
                        _currentText = _currentText.Remove(_cursorPos - 1, 1);
                        _cursorPos--;
                        ResetCursorBlink();
                    }
                }
                else if (_currentText.Length < saveNameMaxLength.Value)
                {
                    _currentText = _currentText.Insert(_cursorPos, c.ToString());
                    _cursorPos++;
                    ResetCursorBlink();
                }
            }

            _cursorTimer += Time.deltaTime;
            if (_cursorTimer >= CURSOR_BLINK_RATE)
            {
                _showCursor = !_showCursor;
                _cursorTimer = 0f;
            }

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (displayMesh == null || !_isActive)
                return;
            var before = _currentText.Substring(0, _cursorPos);
            var after = _currentText.Substring(_cursorPos);
            displayMesh.text = before + (_showCursor ? "|" : " ") + after;
        }

        private void ResetCursorBlink()
        {
            _showCursor = true;
            _cursorTimer = 0f;
            UpdateDisplay();
        }

        internal void Activate()
        {
            _currentText = "";
            _isActive = true;
            _showCursor = true;
            _cursorTimer = 0f;
            _cursorPos = 0;
            if (displayMesh != null)
                displayMesh.text = _currentText;
        }

        internal void Deactivate()
        {
            _isActive = false;
            _showCursor = false;
        }

        internal void SaveName(int slot)
        {
            var name = _currentText.Trim();
            if (string.IsNullOrWhiteSpace(name))
                return;
            System.IO.File.WriteAllText(SlotMetaPath(slot), name.Trim());
            LogInfo($"Saved name '{name}' for slot {slot}");
        }

        internal static string LoadName(int slot)
        {
            var path = SlotMetaPath(slot);
            if (System.IO.File.Exists(path))
                return System.IO.File.ReadAllText(path).Trim();
            return null;
        }
    }
}