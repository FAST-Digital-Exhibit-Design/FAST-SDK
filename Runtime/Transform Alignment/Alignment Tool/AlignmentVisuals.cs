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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FAST
{
    /// <summary>
    /// Implements the <c>AlignmentTool</c> visualization functionality. Not meant to be used directly.
    /// </summary>
    public class AlignmentVisuals : MonoBehaviour
    {
        [Header("UI Elements")]
        protected Canvas visualsCanvas;
        [SerializeField]
        protected CanvasGroup visualsCanvasGroup;
        [SerializeField]
        protected RectTransform highlightRectTransform;
        [SerializeField]
        protected TMP_Text selectedObjectText;
        protected AlignmentTransform selectedAlignment;

        protected void Awake()
        {
            if (visualsCanvas == null) {
                visualsCanvas = GetComponentInParent<Canvas>(true);
            }
            visualsCanvas.gameObject.SetActive(false);
            visualsCanvasGroup.alpha = 0f;
        }
        protected void LateUpdate()
        {
            if (selectedAlignment == null) {
                return;
            }
            // 2D object
            else if (typeof(AlignmentRectTransform) == selectedAlignment.GetType()) {
                var selectedAlignmentRect = selectedAlignment as AlignmentRectTransform;
                var selectedRectTransform = selectedAlignmentRect.gameObject.transform as RectTransform;

                Vector3[] selectedWorldCorners = new Vector3[4];
                Vector3[] selectedScreenCorners = new Vector3[4];
                selectedRectTransform.GetWorldCorners(selectedWorldCorners);

                if (selectedAlignmentRect.drawingCamera == null) {
                    visualsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    visualsCanvas.targetDisplay = selectedAlignmentRect.parentCanvas.targetDisplay;
                    visualsCanvas.worldCamera = null;
                }
                else {
                    visualsCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                    visualsCanvas.targetDisplay = 0;
                    visualsCanvas.worldCamera = selectedAlignmentRect.drawingCamera;
                }


                Vector2 selectedScreenMinPosition;
                Vector2 selectedScreenMaxPosition;
                if (selectedAlignmentRect.drawingCamera == null) {

                    selectedScreenMinPosition = selectedWorldCorners[0];
                    selectedScreenMaxPosition = selectedWorldCorners[2];
                    for (int i = 0; i < selectedWorldCorners.Length; i++) {
                        selectedScreenMinPosition = Vector2.Min(selectedScreenMinPosition, selectedWorldCorners[i]);
                        selectedScreenMaxPosition = Vector2.Max(selectedScreenMaxPosition, selectedWorldCorners[i]);
                    }

                    highlightRectTransform.position = selectedScreenMinPosition;
                }
                else {

                    for (int i = 0; i < selectedWorldCorners.Length; i++) {
                        selectedScreenCorners[i] = selectedAlignment.drawingCamera.WorldToScreenPoint(selectedWorldCorners[i]);
                    }

                    selectedScreenMinPosition = selectedScreenCorners[0];
                    selectedScreenMaxPosition = selectedScreenCorners[2];
                    for (int i = 0; i < selectedScreenCorners.Length; i++) {
                        selectedScreenMinPosition = Vector2.Min(selectedScreenMinPosition, selectedScreenCorners[i]);
                        selectedScreenMaxPosition = Vector2.Max(selectedScreenMaxPosition, selectedScreenCorners[i]);
                    }

                    highlightRectTransform.anchoredPosition = selectedScreenMinPosition;
                }

                
                highlightRectTransform.sizeDelta = selectedScreenMaxPosition - selectedScreenMinPosition;
                highlightRectTransform.localRotation = Quaternion.identity;
                highlightRectTransform.localScale = Vector3.one;
            }
            // 3D object
            else {
                Camera selectedCamera = selectedAlignment.gameObject.GetComponent<Camera>();
                if (selectedCamera != null) {
                    visualsCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                    visualsCanvas.worldCamera = selectedCamera;

                    highlightRectTransform.anchoredPosition = Vector3.zero;
                    highlightRectTransform.localRotation = Quaternion.identity;
                    highlightRectTransform.localScale = Vector3.one;
                    highlightRectTransform.sizeDelta = visualsCanvas.renderingDisplaySize;
                }
                else {
                    visualsCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                    visualsCanvas.worldCamera = selectedAlignment.drawingCamera;

                    var selectedTransform = selectedAlignment.gameObject.transform;
                    Renderer selectedRenderer = selectedTransform.gameObject.GetComponent<Renderer>();
                    if (selectedRenderer == null) {
                        return;
                    }

                    Vector3 cen = selectedRenderer.bounds.center;
                    Vector3 ext = selectedRenderer.bounds.extents;
                    Vector2[] extentPoints = new Vector2[8]
                    {
                    selectedAlignment.drawingCamera.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
                    selectedAlignment.drawingCamera.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
                    selectedAlignment.drawingCamera.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
                    selectedAlignment.drawingCamera.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),

                    selectedAlignment.drawingCamera.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
                    selectedAlignment.drawingCamera.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
                    selectedAlignment.drawingCamera.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
                    selectedAlignment.drawingCamera.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
                    };

                    Vector2 min = extentPoints[0];
                    Vector2 max = extentPoints[0];

                    foreach (Vector2 v in extentPoints) {
                        min = Vector2.Min(min, v);
                        max = Vector2.Max(max, v);
                    }

                    highlightRectTransform.anchoredPosition = min;
                    highlightRectTransform.localRotation = Quaternion.identity;
                    highlightRectTransform.localScale = Vector3.one;
                    highlightRectTransform.sizeDelta = max - min;
                }
            }
        }

        public void OnToggleAlignmentVisuals()
        {
            if (visualsCanvas == null) {
                visualsCanvas = GetComponentInParent<Canvas>(true);
            }
            visualsCanvas.gameObject.SetActive(!visualsCanvas.gameObject.activeSelf);
            visualsCanvasGroup.alpha = 1f - visualsCanvasGroup.alpha;
        }
        public void OnDisplayChanged(int displayIndex)
        {
            visualsCanvas.targetDisplay = displayIndex;
        }
        public void SelectObject(AlignmentTransform alignmentTransform)
        {
            selectedAlignment = alignmentTransform;
            selectedObjectText.text = selectedAlignment.name;
        }
    }
}
