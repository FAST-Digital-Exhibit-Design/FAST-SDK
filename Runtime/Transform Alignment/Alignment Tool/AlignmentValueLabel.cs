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

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace FAST
{
    /// <summary>
    /// Implements the <c>AlignmentTool</c> window functionality. Not meant to be used directly.
    /// </summary>
    public class AlignmentValueLabel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private float sensitivity = 1f;
        private TMP_InputField inputField;
        private float startValue;
        private float offset;

        private static bool isHovering = false;
        private static bool isDragging = false;

        private static Texture2D cursorTexture;
        private static CursorMode cursorMode = CursorMode.Auto;
        private static Vector2 hotSpot = Vector2.zero;

        private void Awake()
        {
            inputField = transform.parent.GetComponentInChildren<TMP_InputField>(true);
            cursorTexture = Resources.Load<Texture2D>("Cursors/east-west_arrow");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovering = true;
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

            startValue = float.Parse(inputField.text);
            offset = eventData.position.x;
        }
        public void OnDrag(PointerEventData eventData)
        {
            inputField.text = (startValue + (eventData.position.x - offset) * sensitivity * AlignmentWindow.sensitivity).ToString();
            inputField.onEndEdit.Invoke(inputField.text);
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            if (!isHovering) {
                Cursor.SetCursor(null, Vector2.zero, cursorMode);
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
            if (!isDragging) {
                Cursor.SetCursor(null, Vector2.zero, cursorMode);
            }
        }
    }
}