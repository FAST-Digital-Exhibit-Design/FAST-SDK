//=============================================================================
// FAST SDK
// A software development kit for creating FAST digital exhibit experiences
// in Unity.
//
// Copyright (C) 2024 Museum of Science, Boston
// <https://www.mos.org/>
//
// This software was developed through a grant to the Museum of Science, Boston
// from the Institute of Museum and Library Services under
// Award #MG-249646-OMS-21. For more information about this grant, see
// <https://www.imls.gov/grants/awarded/mg-249646-oms-21>.
//
// This software is open source: you can redistribute it and/or modify
// it under the terms of the MIT License.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// MIT License for more details.
//
// You should have received a copy of the MIT License along with this software.
// If not, see <https://opensource.org/license/MIT>.
//=============================================================================

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace FAST
{
    /// <summary>
    /// Implements the <c>AlignmentTool</c> window functionality. Not meant to be used directly.
    /// </summary>
    public class AlignmentWindow : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        protected AlignmentVisuals alignmentVisuals;

        protected bool isSendNavigationEvents;
        protected bool isCursorVisible;

        protected Vector2 beginWindowPosition;
        protected Vector2 dragBeginPosition;
        protected Vector2 dragPosition;

        protected string alignmentMode = "window";
        protected bool isIsolate = false;
        protected bool isIgnoreKeyPresses = false;

        [Header("Alignment Objects")]
        [SerializeField]
        protected AlignmentTransform[] alignmentTransforms;
        [SerializeField]
        protected AlignmentTransform selectedTransform;
        [SerializeField]
        protected int selectedIndex;

        [Header("UI Elements")]
        [SerializeField]
        protected Canvas windowCanvas;
        [SerializeField]
        protected CanvasGroup windowCanvasGroup;
        [SerializeField]
        protected TMP_Text selectedObjectNameText;
        [SerializeField]
        protected TMP_Text sensitivityText;
        [SerializeField]
        protected TMP_Dropdown displayDropdown;
        [SerializeField]
        protected RectTransform windowTransform;
        [SerializeField]
        protected Toggle isolationToggle;
        [SerializeField]
        protected Slider opacitySlider;
        [SerializeField]
        protected TMP_InputField xPositionInput;
        [SerializeField]
        protected TMP_InputField yPositionInput;
        [SerializeField]
        protected TMP_InputField rotationInput;
        [SerializeField]
        protected TMP_InputField scaleInput;
        [SerializeField]
        protected TMP_InputField xSizeInput;
        [SerializeField]
        protected TMP_InputField ySizeInput;
        
        // Scales the increment amount used to change transform or opacity values using 
        // the keyboard or mouse. Scale options are [0.01, 0.1, 1, 10, 100].
        public static float sensitivity = 1f;
        private static int sensitivityExponent = 0;
        private const int kSensistivityExponentMax = 2; 

        protected void Awake()
        {
            selectedObjectNameText.text = "";
            selectedIndex = -1;
            selectedTransform = null;

            // If there anren't any AlignmentTransforms, the Alignment Tools will be disabled in Start()
            alignmentTransforms = FindObjectsByType<AlignmentTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None);            
        }
        protected void Start()
        {
            windowCanvas.gameObject.SetActive(false);
            windowCanvasGroup.alpha = 0f;

            sensitivity = Mathf.Pow(10f, sensitivityExponent);
            sensitivityText.text = sensitivity.ToString();

            if (alignmentTransforms.Length > 0) {
                selectedIndex = 0;
                SelectObject();
                OnReset();
            }
            else {
                windowCanvas.transform.parent.gameObject.SetActive(false);
            }
        }

        protected void OnEnable()
        {
            isCursorVisible = Cursor.visible;
            Cursor.visible = true;

            isSendNavigationEvents = EventSystem.current.sendNavigationEvents;
            EventSystem.current.sendNavigationEvents = false;

            Rect windowRect = new Rect(windowTransform.anchoredPosition, windowTransform.sizeDelta);
            bool isOverlapping = windowCanvas.pixelRect.Overlaps(windowRect);
            if (!isOverlapping) {
                windowTransform.anchoredPosition = (windowCanvas.renderingDisplaySize - windowTransform.sizeDelta) / 2.0f;
            }
        }
        protected void OnDisable ()
        {
            Cursor.visible = isCursorVisible;

            if (EventSystem.current != null) {
                EventSystem.current.sendNavigationEvents = isSendNavigationEvents;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            dragBeginPosition = eventData.position;

            RectTransform rectTransform = transform as RectTransform;
            beginWindowPosition = rectTransform.position;
        }
        public void OnDrag(PointerEventData eventData)
        {
            dragPosition = eventData.position;

            Vector2 direction = dragPosition - dragBeginPosition;
            RectTransform rectTransform = transform as RectTransform;
            rectTransform.position = beginWindowPosition + direction;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            dragPosition = eventData.position;

            Vector2 direction = dragPosition - dragBeginPosition;
            RectTransform rectTransform = transform as RectTransform;
            Vector2 endPosition = beginWindowPosition + direction;
            endPosition.x = Mathf.Round(endPosition.x);
            endPosition.y = Mathf.Round(endPosition.y);
            rectTransform.position = endPosition;
        }
        
        public void OnDisplayChanged(int displayIndex)
        {
            windowCanvas.targetDisplay = displayIndex;
            alignmentVisuals.OnDisplayChanged(displayIndex);
        }
        public void OnXPositionChanged(string input)
        {
            bool isValid = float.TryParse(input, out float value);
            if (!isValid) {
                return;
            }
            selectedTransform.offsetPosition.x = value;
        }
        public void OnXPositionEndEdit(string input)
        {
            xPositionInput.text = selectedTransform.offsetPosition.x.ToString();
        }
        public void OnYPositionChanged(string input)
        {
            bool isValid = float.TryParse(input, out float value);
            if (!isValid) {
                return;
            }
            selectedTransform.offsetPosition.y = value;
        }
        public void OnYPositionEndEdit(string input)
        {
            yPositionInput.text = selectedTransform.offsetPosition.y.ToString();
        }
        public void OnRotationChanged(string input)
        {
            bool isValid = float.TryParse(input, out float value);
            if (!isValid) {
                return;
            }
            selectedTransform.offsetRotation = value;
            selectedTransform.offsetRotation = selectedTransform.offsetRotation % 360f;
        }
        public void OnRotationEndEdit(string input)
        {
            rotationInput.text = selectedTransform.offsetRotation.ToString();
        }
        public void OnScaleChanged(string input)
        {
            bool isValid = float.TryParse(input, out float value);
            if (!isValid) {
                return;
            }
            selectedTransform.offsetScale = value;
            selectedTransform.offsetScale = Mathf.Max(selectedTransform.offsetScale, -1f);
        }
        public void OnScaleEndEdit(string input)
        {
            scaleInput.text = selectedTransform.offsetScale.ToString();
        }
        public void OnXSizeChanged(string input)
        {
            bool isValid = float.TryParse(input, out float value);
            if (!isValid) {
                return;
            }

            if (typeof(AlignmentRectTransform) != selectedTransform.GetType()) {
                return;
            }
            var selectedRectTransform = selectedTransform as AlignmentRectTransform;
            selectedRectTransform.offsetSize.x = value;
        }
        public void OnXSizeEndEdit(string input)
        {
            if (typeof(AlignmentRectTransform) != selectedTransform.GetType()) {
                return;
            }
            var selectedRectTransform = selectedTransform as AlignmentRectTransform;
            xSizeInput.text = selectedRectTransform.offsetSize.x.ToString();
        }
        public void OnYSizeChanged(string input)
        {
            bool isValid = float.TryParse(input, out float value);
            if (!isValid) {
                return;
            }

            if (typeof(AlignmentRectTransform) != selectedTransform.GetType()) {
                return;
            }
            var selectedRectTransform = selectedTransform as AlignmentRectTransform;
            selectedRectTransform.offsetSize.y = value;
        }
        public void OnYSizeEndEdit(string input)
        {
            if (typeof(AlignmentRectTransform) != selectedTransform.GetType()) {
                return;
            }
            var selectedRectTransform = selectedTransform as AlignmentRectTransform;
            ySizeInput.text = selectedRectTransform.offsetSize.y.ToString();
        }

        public void OnChangeMode(string modeName)
        {
            modeName = modeName.ToLower();
            alignmentMode = modeName;
        }
        public void OnIsolateObject(bool isChecked)
        {
            isIsolate = isChecked;

            foreach (var alignmentTransform in alignmentTransforms) {
                if (!alignmentTransform.gameObject.GetComponent<Camera>()) {
                    alignmentTransform.gameObject.SetActive(!isIsolate);
                }
            }

            if (isIsolate) {
                selectedTransform.gameObject.SetActive(true);
            }
        }
        public void IgnoreKeyPresses()
        {
            isIgnoreKeyPresses = true;
        }
        public void EnableKeyPresses()
        {
            isIgnoreKeyPresses = false;
        }

        public void OnToggleAlignmentWindow()
        {
            windowCanvas.gameObject.SetActive(!windowCanvas.gameObject.activeSelf);
            if (windowCanvas.gameObject.activeSelf) {
                KeyBinding.SetState("Alignment");
            }
            else {
                KeyBinding.SetPreviousState();
            }
            windowCanvasGroup.alpha = 1f - windowCanvasGroup.alpha;
            alignmentVisuals.OnToggleAlignmentVisuals();
        }
        public void OnLeft()
        {
            if (isIgnoreKeyPresses) {
                return;
            }

            if (alignmentMode.Equals("display")) {
                displayDropdown.value--;
            }
            else if (alignmentMode.Equals("window")) {
                Vector2 windowPosition = windowTransform.localPosition;
                windowPosition.x -= 1 * sensitivity;
                windowTransform.localPosition = windowPosition;
            }
            else if (alignmentMode.Equals("object")) {
                selectedIndex = Mod(selectedIndex - 1, alignmentTransforms.Length);
                SelectObject();
            }
            else if (alignmentMode.Equals("isolation")) {
                isolationToggle.isOn = !isolationToggle.isOn;
            }
            else if (alignmentMode.Equals("opacity")) {
                opacitySlider.value -= 0.01f * sensitivity;
            }
            else if (alignmentMode.Equals("position")) {
                selectedTransform.offsetPosition.x -= 1f * sensitivity;
                xPositionInput.text = selectedTransform.offsetPosition.x.ToString();
            }
            else if (alignmentMode.Equals("rotation")) {
                selectedTransform.offsetRotation -= 1 * sensitivity;
                selectedTransform.offsetRotation = selectedTransform.offsetRotation % 360f;
                rotationInput.text = selectedTransform.offsetRotation.ToString();
            }
            else if (alignmentMode.Equals("scale")) {
                selectedTransform.offsetScale -= 0.1f * sensitivity;
                selectedTransform.offsetScale = Mathf.Max(selectedTransform.offsetScale, -1f);
                scaleInput.text = selectedTransform.offsetScale.ToString();
            }
            else if (alignmentMode.Equals("size")) {
                if (typeof(AlignmentRectTransform) == selectedTransform.GetType()) {
                    var selectedRectTransform = selectedTransform as AlignmentRectTransform;
                    selectedRectTransform.offsetSize.x -= 1 * sensitivity;
                    xSizeInput.text = selectedRectTransform.offsetSize.x.ToString();
                }
            }
        }
        public void OnRight()
        {
            if (isIgnoreKeyPresses) {
                return;
            }

            if (alignmentMode.Equals("display")) {
                displayDropdown.value++;
            }
            else if (alignmentMode.Equals("window")) {
                Vector2 windowPosition = windowTransform.localPosition;
                windowPosition.x += 1 * sensitivity;
                windowTransform.localPosition = windowPosition;
            }
            else if (alignmentMode.Equals("object")) {
                selectedIndex = Mod(selectedIndex + 1, alignmentTransforms.Length);
                SelectObject();
            }
            else if (alignmentMode.Equals("isolation")) {
                isolationToggle.isOn = !isolationToggle.isOn;
            }
            else if (alignmentMode.Equals("opacity")) {
                opacitySlider.value += 0.01f * sensitivity;
            }
            else if (alignmentMode.Equals("position")) {
                selectedTransform.offsetPosition.x += 1 * sensitivity;
                xPositionInput.text = selectedTransform.offsetPosition.x.ToString();
            }
            else if (alignmentMode.Equals("rotation")) {
                selectedTransform.offsetRotation += 1 * sensitivity;
                selectedTransform.offsetRotation = selectedTransform.offsetRotation % 360f;
                rotationInput.text = selectedTransform.offsetRotation.ToString();
            }
            else if (alignmentMode.Equals("scale")) {
                selectedTransform.offsetScale += 0.1f * sensitivity;
                selectedTransform.offsetScale = Mathf.Max(selectedTransform.offsetScale, -1f);
                scaleInput.text = selectedTransform.offsetScale.ToString();
            }
            else if (alignmentMode.Equals("size")) {
                if (typeof(AlignmentRectTransform) == selectedTransform.GetType()) {
                    var selectedRectTransform = selectedTransform as AlignmentRectTransform;
                    selectedRectTransform.offsetSize.x += 1 * sensitivity;
                    xSizeInput.text = selectedRectTransform.offsetSize.x.ToString();
                }
            }
        }

        public void OnDown()
        {
            if (isIgnoreKeyPresses) {
                return;
            }

            if (alignmentMode.Equals("display")) {
                displayDropdown.value--;
            }
            else if (alignmentMode.Equals("window")) {
                Vector2 windowPosition = windowTransform.localPosition;
                windowPosition.y -= 1 * sensitivity;
                windowTransform.localPosition = windowPosition;
            }
            else if (alignmentMode.Equals("object")) {

                selectedIndex = Mod(selectedIndex - 1, alignmentTransforms.Length);
                SelectObject();
            }
            else if (alignmentMode.Equals("isolation")) {
                isolationToggle.isOn = !isolationToggle.isOn;
            }
            else if (alignmentMode.Equals("opacity")) {
                opacitySlider.value -= 0.01f * sensitivity;
            }
            else if (alignmentMode.Equals("position")) {
                selectedTransform.offsetPosition.y -= 1 * sensitivity;
                yPositionInput.text = selectedTransform.offsetPosition.y.ToString();
            }
            else if (alignmentMode.Equals("rotation")) {
                selectedTransform.offsetRotation -= 1 * sensitivity;
                selectedTransform.offsetRotation = selectedTransform.offsetRotation % 360f;
                rotationInput.text = selectedTransform.offsetRotation.ToString();
            }
            else if (alignmentMode.Equals("scale")) {
                selectedTransform.offsetScale -= 0.1f * sensitivity;
                selectedTransform.offsetScale = Mathf.Max(selectedTransform.offsetScale, -1f);
                scaleInput.text = selectedTransform.offsetScale.ToString();
            }
            else if (alignmentMode.Equals("size")) {
                if (typeof(AlignmentRectTransform) == selectedTransform.GetType()) {
                    var selectedRectTransform = selectedTransform as AlignmentRectTransform;
                    selectedRectTransform.offsetSize.y -= 1 * sensitivity;
                    ySizeInput.text = selectedRectTransform.offsetSize.y.ToString();
                }
            }
        }
        public void OnUp()
        {
            if (isIgnoreKeyPresses) {
                return;
            }

            if (alignmentMode.Equals("display")) {
                displayDropdown.value++;
            }
            else if (alignmentMode.Equals("window")) {
                Vector2 windowPosition = windowTransform.localPosition;
                windowPosition.y += 1 * sensitivity;
                windowTransform.localPosition = windowPosition;
            }
            else if (alignmentMode.Equals("object")) {
                selectedIndex = Mod(selectedIndex + 1, alignmentTransforms.Length);
                SelectObject();
            }
            else if (alignmentMode.Equals("isolation")) {
                isolationToggle.isOn = !isolationToggle.isOn;
            }
            else if (alignmentMode.Equals("opacity")) {
                opacitySlider.value += 0.01f * sensitivity;
            }
            else if (alignmentMode.Equals("position")) {
                selectedTransform.offsetPosition.y += 1 * sensitivity;
                yPositionInput.text = selectedTransform.offsetPosition.y.ToString();
            }
            else if (alignmentMode.Equals("rotation")) {
                selectedTransform.offsetRotation += 1 * sensitivity;
                selectedTransform.offsetRotation = selectedTransform.offsetRotation % 360f;
                rotationInput.text = selectedTransform.offsetRotation.ToString();
            }
            else if (alignmentMode.Equals("scale")) {
                selectedTransform.offsetScale += 0.1f * sensitivity;
                selectedTransform.offsetScale = Mathf.Max(selectedTransform.offsetScale, -1f);
                scaleInput.text = selectedTransform.offsetScale.ToString();
            }
            else if (alignmentMode.Equals("size")) {
                if (typeof(AlignmentRectTransform) == selectedTransform.GetType()) {
                    var selectedRectTransform = selectedTransform as AlignmentRectTransform;
                    selectedRectTransform.offsetSize.y += 1 * sensitivity;
                    ySizeInput.text = selectedRectTransform.offsetSize.y.ToString();
                }
            }
        }

        public void OnIncreaseSensitivity()
        {
            sensitivityExponent = Mathf.Clamp(++sensitivityExponent, -kSensistivityExponentMax, kSensistivityExponentMax);
            sensitivity = Mathf.Pow(10f, sensitivityExponent);
            sensitivityText.text = sensitivity.ToString();
        }
        public void OnDecreaseSensitivity()
        {
            sensitivityExponent = Mathf.Clamp(--sensitivityExponent, -kSensistivityExponentMax, kSensistivityExponentMax);
            sensitivity = Mathf.Pow(10f, sensitivityExponent);
            sensitivityText.text = sensitivity.ToString();
        }

        public void OnSubmit()
        {
            if (isIgnoreKeyPresses) {
                return;
            }

            if (alignmentMode.Equals("isolation")) {
                isolationToggle.isOn = !isolationToggle.isOn;
            }
        }
        public void OnSave()
        {
            List<AlignmentTransformSettings> settings = Application.settings.alignmentSettings;
            settings.Clear();

            foreach (var alignmentTransform in alignmentTransforms) {
                AlignmentTransformSettings alignmentTransformSettings = new();

                if (alignmentTransform.GetType().Equals(typeof(AlignmentRectTransform))) {
                    AlignmentRectTransformSettings alignmentRectTransformSettings = new();
                    alignmentTransformSettings = alignmentRectTransformSettings;

                    alignmentRectTransformSettings.size.x = (alignmentTransform as AlignmentRectTransform).offsetSize.x;
                    alignmentRectTransformSettings.size.y = (alignmentTransform as AlignmentRectTransform).offsetSize.y;
                }

                alignmentTransformSettings.name = alignmentTransform.name;
                alignmentTransformSettings.position.x = alignmentTransform.offsetPosition.x;
                alignmentTransformSettings.position.y = alignmentTransform.offsetPosition.y;
                alignmentTransformSettings.position.z = alignmentTransform.offsetPosition.z;
                alignmentTransformSettings.rotation.z = alignmentTransform.offsetRotation;
                alignmentTransformSettings.scale.xyz = alignmentTransform.offsetScale;

                settings.Add(alignmentTransformSettings);
            }

            Application.WriteSettings();
        }
        public void OnReset()
        {
            List<AlignmentTransformSettings> settings = Application.settings.alignmentSettings;

            foreach (var alignmentSettings in settings) {
                AlignmentTransform alignmentTransform = alignmentTransforms.Single(x => x.name.Equals(alignmentSettings.name));
                if (alignmentTransform == null) {
                    continue;
                }
                alignmentTransform.offsetPosition.x = alignmentSettings.position.x;
                alignmentTransform.offsetPosition.y = alignmentSettings.position.y;
                alignmentTransform.offsetPosition.z = alignmentSettings.position.z;
                alignmentTransform.offsetRotation = alignmentSettings.rotation.z;
                alignmentTransform.offsetScale = alignmentSettings.scale.xyz;

                if (alignmentTransform.GetType().Equals(typeof(AlignmentRectTransform)) &&
                    alignmentSettings.GetType().Equals(typeof(AlignmentRectTransformSettings))) {
                    (alignmentTransform as AlignmentRectTransform).offsetSize.x = (alignmentSettings as AlignmentRectTransformSettings).size.x;
                    (alignmentTransform as AlignmentRectTransform).offsetSize.y = (alignmentSettings as AlignmentRectTransformSettings).size.y;
                }
            }
        }

        protected void SelectObject()
        {
            selectedTransform = alignmentTransforms[selectedIndex];
            selectedObjectNameText.text = selectedTransform.gameObject.name;
            alignmentVisuals.SelectObject(selectedTransform);

            xPositionInput.text = selectedTransform.offsetPosition.x.ToString();
            yPositionInput.text = selectedTransform.offsetPosition.y.ToString();
            rotationInput.text = selectedTransform.offsetRotation.ToString();
            scaleInput.text = selectedTransform.offsetScale.ToString();

            if (typeof(AlignmentRectTransform) == selectedTransform.GetType()) {
                var selectedRectTransform = selectedTransform as AlignmentRectTransform;
                xSizeInput.interactable = true;
                xSizeInput.text = selectedRectTransform.offsetSize.x.ToString();
                ySizeInput.interactable = true;
                ySizeInput.text = selectedRectTransform.offsetSize.y.ToString();
            }
            else {
                xSizeInput.interactable = false;
                ySizeInput.interactable = false;
            }

            if (isIsolate) {
                foreach (var alignmentTransform in alignmentTransforms) {
                    if (!alignmentTransform.gameObject.GetComponent<Camera>()) {
                        alignmentTransform.gameObject.SetActive(false);
                    }
                }
                selectedTransform.gameObject.SetActive(true);
            }
        }

        private int Mod(int value, int modulus)
        {
            int remainder = value % modulus;
            int result = remainder < 0 ? remainder + modulus : remainder;
            return result;
        }
        private float Mod(float value, float modulus)
        {
            float remainder = value % modulus;
            float result = remainder < 0 ? remainder + modulus : remainder;
            return result;
        }
    }
}