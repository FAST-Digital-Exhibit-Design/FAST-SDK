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
    /// Loading screen progress and error GUI. Not meant to be used directly in code.
    /// </summary>
    /// <remarks>
    /// Default functionality is included in the <c>Startup Prefab</c>.
    /// </remarks>
    /// @warning If you find yourself trying to use @ref FAST.LoadingProgress in code, you may 
	/// want to review the @ref Startup documentation to make sure you are following FAST best practices.
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/GameObject.html">
    /// UnityEngine.GameObject</a>
    public class LoadingProgress : MonoBehaviour
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The parent <c style="color:DarkRed;"><see cref="GameObject"/></c> all progress GUI 
        /// elements are grouped under.
        /// </summary>
        [Header("Progress")]
        [SerializeField]
        private GameObject progressGroup;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The progress message title.
        /// </summary>
        [SerializeField]
        private TMP_Text progressTitle;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The progress message and any additional details.
        /// </summary>
        [SerializeField]
        private TMP_Text progressMessage;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The progress bar percentage in the range [0, 100].
        /// </summary>
        [SerializeField]
        private int progressBarValue;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The progress bar fill image.
        /// </summary>
        [SerializeField]
        private Image progressBarFill;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The parent <c style="color:DarkRed;"><see cref="GameObject"/></c> all error GUI 
        /// elements are grouped under.
        /// </summary>
        [Header("Error")]
        [SerializeField]
        private GameObject errorGroup;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The error message title.
        /// </summary>
        [SerializeField]
        private TMP_Text errorTitle;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The error message and any additional details.
        /// </summary>
        [SerializeField]
        private TMP_Text errorMessage;

        void Awake()
        {
            progressGroup.SetActive(true);
            UpdateProgress(0, "Exhibit is loading, please wait . . .", "");
        }

        /// <summary>
        /// Updates the progress title, message, and progress bar percentage.
        /// </summary>
        /// <param name="value">The progress bar percentage in the range [0, 100].</param>
        /// <param name="title">The title of the message.</param>
        /// <param name="message">The message and any additional details.</param>
        public void UpdateProgress(int value, string title, string message)
        {
            progressTitle.text = title;
            progressMessage.text = message;

            progressBarValue = value;
            progressBarFill.transform.localScale = new Vector3(progressBarValue * 0.01f, 1f, 1f);

        }

        /// <summary>
        /// Updates the progress bar percentage.
        /// </summary>
        /// <param name="value">The progress bar percentage in the range [0, 100].</param>
        public void UpdateProgressPercent(int value)
        {
            progressBarValue = value;
            progressBarFill.transform.localScale = new Vector3(progressBarValue * 0.01f, 1f, 1f);
        }

        /// <summary>
        /// Updates the progress title and message.
        /// </summary>
        /// <param name="title">The title of the message.</param>
        /// <param name="message">The message and any additional details.</param>
        public void UpdateProgressMessage(string title, string message)
        {
            progressTitle.text = title;
            progressMessage.text = message;
        }

        /// <summary>
        /// Updates the error title and message.
        /// </summary>
        /// <param name="title">The title of the error message.</param>
        /// <param name="message">The error message and any additional details.</param>
        public void UpdateErrorMessage(string title, string message)
        {
            progressGroup.SetActive(false);
            errorGroup.SetActive(true);

            errorTitle.text = title;
            errorMessage.text = message;
        }
    }
}