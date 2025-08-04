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

using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using UnityEngine;

namespace FAST
{
    /// <summary>
    /// A base class for activity settings which should be used to derive custom activity settings.
    /// </summary>
    /// @note Derive from this class to create the activity settings for your Unity application.
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Application-productName.html">
    /// UnityEngine.Application.productName</a>
    [System.Serializable]
    public abstract class BaseSettings
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// A unique name for the activity.
        /// </summary>
        /// <remarks>
        /// Consider enumerating the name if there are multiple instances of the same activity. 
        /// For example, <c>"Activity1"</c> or <c>"Activity2"</c>.
        /// </remarks>
        public string activityId = "";

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The <see cref="FAST.LogSettings"/> for the activity.
        /// </summary>
        public LogSettings logSettings = new();

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The <see cref="FAST.AssetSettings"/> for the activity.
        /// </summary>
        public AssetSettings assetSettings = new();

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The <see cref="FAST.DisplaySettings"/> for the activity.
        /// </summary>
        public DisplaySettings displaySettings = new();

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The <see cref="FAST.WhiteScreenSettings"/> for the activity.
        /// </summary>
        public WhiteScreenSettings whiteScreenSettings = new();

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The <see cref="FAST.WebRequestSettings"/> for each <see cref="FAST.WebRequest"/> 
        /// in the activity.
        /// </summary>
        [XmlElement]
        public List<WebRequestSettings> webRequestSettings = new();

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The <see cref="FAST.UdpConnectionSettings"/> for each <see cref="FAST.UdpConnection"/> 
        /// in the activity.
        /// </summary>
        [XmlElement]
        public List<UdpConnectionSettings> udpConnectionSettings = new();

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The <see cref="FAST.SerialConnectionSettings"/> for each <see cref="FAST.SerialConnection"/> 
        /// in the activity.
        /// </summary>
        [XmlElement]
        public List<SerialConnectionSettings> serialConnectionSettings = new();

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The <see cref="FAST.AlignmentTransformSettings"/> and <see cref="FAST.AlignmentRectTransformSettings"/> 
        /// for each <see cref="FAST.AlignmentTransform"/> and <see cref="FAST.AlignmentRectTransform"/>
        /// in the activity.
        /// </summary>
        [XmlArrayItem(Type = typeof(AlignmentTransformSettings)),
         XmlArrayItem(Type = typeof(AlignmentRectTransformSettings))]
        public List<AlignmentTransformSettings> alignmentSettings = new();

        /// <summary>
        /// Initializes the <see cref="FAST.BaseSettings.activityId"/> and <see cref="FAST.AssetSettings.languages"/> 
        /// when creating an activity settings instance in code.
        /// </summary>
        /// <remarks>
        /// Called from <see cref="FAST.SettingsLoader.ExecuteLoad()"/> when an activity settings file is not found. 
        /// Sets the <see cref="FAST.BaseSettings.activityId"/> to the 
        /// <c style="color:DarkRed;"><see cref="UnityEngine.Application.productName"/></c> and 
        /// the <see cref="FAST.AssetSettings.languages"/> to the current operating system language.
        /// </remarks>
        public virtual void Initialize()
        {
            activityId = UnityEngine.Application.productName;
            assetSettings.languages = new string[] { CultureInfo.InstalledUICulture.IetfLanguageTag };
        }

        /// <summary>
        /// Clears and reinitializes the <see cref="FAST.BaseSettings.webRequestSettings"/>,
        /// <see cref="FAST.BaseSettings.udpConnectionSettings"/>, and <see cref="FAST.BaseSettings.serialConnectionSettings"/> 
        /// from all the <see cref="FAST.WebRequest"/>s, <see cref="FAST.UdpConnection"/>s, and 
        /// <see cref="FAST.SerialConnection"/>s in the activty.
        /// </summary>
        /// <remarks>
        /// Called from <see cref="Application.WriteSettings"/> to make sure the settings are up-to-date before 
        /// writing the XML file.
        /// </remarks>
        public virtual void Update()
        {
            // TODO: All settings get cleared, but some settings don't get reassigned here because 
            // they aren't fields on WebRequest, UdpConnection, or SerialConnection
            webRequestSettings.Clear();
            foreach (var (webRequest, webRequestLoader) in Application.webRequests.Zip(StartupManager.webRequestLoaders, Tuple.Create)) {
                WebRequestSettings webRequestSettings = new() {
                    id = webRequest.id,
                    uri = webRequest.uri.OriginalString,

                    prefixErrorMessage = webRequestLoader.prefixErrorMessage,
                    replacementErrorMessage = webRequestLoader.replacementErrorMessage,
                    suffixErrorMessage = webRequestLoader.suffixErrorMessage
                };

                this.webRequestSettings.Add(webRequestSettings);
            }

            udpConnectionSettings.Clear();
            foreach (var (udpConnection, udpConnectionLoader) in Application.udpConnections.Zip(StartupManager.udpConnectionLoaders, Tuple.Create)) {
                UdpConnectionSettings udpConnectionSettings = new() {
                    id = udpConnection.id,
                    localReceivePort = udpConnection.localReceivePort,
                    localSendPort = udpConnection.localSendPort,
                    remoteIpAddress = udpConnection.remoteIpAddress,
                    remotePort = udpConnection.remotePort,

                    prefixErrorMessage = udpConnectionLoader.prefixErrorMessage,
                    replacementErrorMessage = udpConnectionLoader.replacementErrorMessage,
                    suffixErrorMessage = udpConnectionLoader.suffixErrorMessage
                };

                this.udpConnectionSettings.Add(udpConnectionSettings);
            }

            serialConnectionSettings.Clear();
            foreach (var (serialConnection, serialConnectionLoader) in Application.serialConnections.Zip(StartupManager.serialConnectionLoaders, Tuple.Create)) {
#if FAST_WIN
                if (!serialConnection.port.Contains("COM")) {
                    serialConnection.port = "COM" + serialConnection.port;
                }
#endif
                SerialConnectionSettings serialConnectionSettings = new() {
                    id = serialConnection.id,
                    port = serialConnection.port,
                    baudRate = (int)serialConnection.baudRate,

                    startupDelaySeconds = serialConnectionLoader.startupDelaySeconds,
                    prefixErrorMessage = serialConnectionLoader.prefixErrorMessage,
                    replacementErrorMessage = serialConnectionLoader.replacementErrorMessage,
                    suffixErrorMessage = serialConnectionLoader.suffixErrorMessage
                };

                this.serialConnectionSettings.Add(serialConnectionSettings);
            }
        }
    }
}