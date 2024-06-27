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

namespace FAST
{
    /// <summary>
    /// Manages the GUI for the startup loading screen progress and errors. Not meant to be used directly in code.
    /// </summary>
    /// <remarks>
    /// Default functionality is included in the <c>Startup Prefab</c>.
    /// </remarks>
    /// @warning If you find yourself trying to use @ref FAST.LoadingScreenManager in code, you may 
	/// want to review the @ref Startup documentation to make sure you are following FAST best practices.
    public class LoadingScreenManager : MonoBehaviour
    {
        /// <summary>
        /// List of progress bars to update when an update function is called.
        /// </summary>
        /// <remarks>
        /// Usually there is one <see cref="LoadingProgress"/> per display.
        /// </remarks>
        [SerializeField]
        private LoadingProgress[] loadingProgresses;

        void Awake()
        {
            loadingProgresses = GetComponentsInChildren<LoadingProgress>(true);
        }

        /// <summary>
        /// Updates the progress bar percentage on all <see cref="LoadingProgress"/>es.
        /// </summary>
        /// <remarks>
        /// Called by <see cref="FAST.StartupManager"/> every time a <see cref="FAST.StartupLoader"/> 
        /// is loaded during startup.
        /// </remarks>
        /// <param name="percent">The progress bar percentage in the range [0, 100].</param>
        public void UpdateProgressPercent(int percent)
        {
            foreach (LoadingProgress loadingProgress in loadingProgresses) {
                loadingProgress.UpdateProgressPercent(percent);
            }
        }

        /// <summary>
        /// Updates the title and message on all <see cref="LoadingProgress"/>es.
        /// </summary>
        /// <remarks>
        /// Called by a <see cref="FAST.StartupLoader"/> when loading status and feedback is needed.
        /// </remarks>
        /// <param name="heading">The title of the message.</param>
        /// <param name="details">The message and any additional details.</param>
        public void UpdateProgressMessage(string heading, string details)
        {
            foreach (LoadingProgress loadingProgress in loadingProgresses) {
                loadingProgress.UpdateProgressMessage(heading, details);
            }
        }

        /// <summary>
        /// Updates the title and message on all <see cref="LoadingProgress"/>es when an error occurs.
        /// </summary>
        /// <remarks>
        /// Called by a <see cref="FAST.StartupLoader"/> when there is an error.
        /// </remarks>
        /// <param name="heading">The title of the error message.</param>
        /// <param name="message">The error message and any additional details.</param>
        public void UpdateErrorMessage(string heading, string message)
        {
            message = message.Replace("\\n", "\n").Replace("\\t", "\t");
            foreach (LoadingProgress progressBar in loadingProgresses) {
                progressBar.UpdateErrorMessage(heading, message);
            }
        }
    }
}