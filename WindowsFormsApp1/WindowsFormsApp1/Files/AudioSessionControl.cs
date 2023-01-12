/*
  LICENSE
  -------
  Copyright (C) 2007-2010 Ray Molenkamp

  This source code is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this source code or the software it produces.

  Permission is granted to anyone to use this source code for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this source code must not be misrepresented; you must not
     claim that you wrote the original source code.  If you use this source code
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original source code.
  3. This notice may not be removed or altered from any source distribution.
*/
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using CoreAudioApi.Interfaces;
using System.Runtime.InteropServices;

namespace CoreAudioApi
{
    public enum AudioSessionDisconnectReason
    {
        DisconnectReasonDeviceRemoval = 0,
        DisconnectReasonServerShutdown = (DisconnectReasonDeviceRemoval + 1),
        DisconnectReasonFormatChanged = (DisconnectReasonServerShutdown + 1),
        DisconnectReasonSessionLogoff = (DisconnectReasonFormatChanged + 1),
        DisconnectReasonSessionDisconnected = (DisconnectReasonSessionLogoff + 1),
        DisconnectReasonExclusiveModeOverride = (DisconnectReasonSessionDisconnected + 1)
    }

    public enum AudioSessionState
    {
        AudioSessionStateInactive = 0,
        AudioSessionStateActive = 1,
        AudioSessionStateExpired = 2
    }

    public class AudioVolumeNotificationData
    {
        private Guid _EventContext;
        private bool _Muted;
        private float _MasterVolume;
        private int _Channels;
        private float[] _ChannelVolume;

        public Guid EventContext
        {
            get
            {
                return _EventContext;
            }
        }

        public bool Muted
        {
            get
            {
                return _Muted;
            }
        }

        public float MasterVolume
        {
            get
            {
                return _MasterVolume;
            }
        }
        public int Channels
        {
            get
            {
                return _Channels;
            }
        }

        public float[] ChannelVolume
        {
            get
            {
                return _ChannelVolume;
            }
        }
        public AudioVolumeNotificationData(Guid eventContext, bool muted, float masterVolume, float[] channelVolume)
        {
            _EventContext = eventContext;
            _Muted = muted;
            _MasterVolume = masterVolume;
            _Channels = channelVolume.Length;
            _ChannelVolume = channelVolume;
        }
    }

    public class AudioSessionManager
    {
        private IAudioSessionManager2 _AudioSessionManager;
        private SessionCollection _Sessions;

        internal AudioSessionManager(IAudioSessionManager2 realAudioSessionManager)
        {
            _AudioSessionManager = realAudioSessionManager;
            IAudioSessionEnumerator _SessionEnum;
            Marshal.ThrowExceptionForHR(_AudioSessionManager.GetSessionEnumerator(out _SessionEnum));
            _Sessions = new SessionCollection(_SessionEnum);
        }

        public SessionCollection Sessions
        {
            get
            {
                return _Sessions;
            }
        }

    }
    public class AudioSessionControl 
    {
        internal IAudioSessionControl2 _AudioSessionControl;
        internal AudioMeterInformation _AudioMeterInformation;
        internal SimpleAudioVolume _SimpleAudioVolume;

        public AudioMeterInformation AudioMeterInformation
        {
            get
            {
                return _AudioMeterInformation;
            }
        }

        public SimpleAudioVolume SimpleAudioVolume
        {
            get
            {
                return _SimpleAudioVolume;
            }
        }


        internal AudioSessionControl(IAudioSessionControl2 realAudioSessionControl)
        {
            IAudioMeterInformation _meters = realAudioSessionControl as IAudioMeterInformation;
            ISimpleAudioVolume _volume = realAudioSessionControl as ISimpleAudioVolume; 
            if (_meters != null)
                _AudioMeterInformation = new CoreAudioApi.AudioMeterInformation(_meters);
            if (_volume != null)
                _SimpleAudioVolume = new SimpleAudioVolume(_volume);
            _AudioSessionControl = realAudioSessionControl;
            
        }

        public void RegisterAudioSessionNotification(IAudioSessionEvents eventConsumer)
        {
             Marshal.ThrowExceptionForHR(_AudioSessionControl.RegisterAudioSessionNotification(eventConsumer));
        }

        public void UnregisterAudioSessionNotification(IAudioSessionEvents eventConsumer)
        {
            Marshal.ThrowExceptionForHR(_AudioSessionControl.UnregisterAudioSessionNotification(eventConsumer));
        }

        public AudioSessionState State
        {
            get
            {
                AudioSessionState res;
                Marshal.ThrowExceptionForHR(_AudioSessionControl.GetState(out res));
                return res;
            }
        }

        public string DisplayName
        {
            get
            {
                IntPtr NamePtr;
                Marshal.ThrowExceptionForHR(_AudioSessionControl.GetDisplayName(out NamePtr));
                string res = Marshal.PtrToStringAuto(NamePtr);
                Marshal.FreeCoTaskMem(NamePtr);
                return res;
            }
        }

        public string IconPath
        {
            get
            {
                IntPtr NamePtr;
                Marshal.ThrowExceptionForHR(_AudioSessionControl.GetIconPath(out NamePtr));
                string res = Marshal.PtrToStringAuto(NamePtr);
                Marshal.FreeCoTaskMem(NamePtr);
                return res;
            }
        }

        public string SessionIdentifier
        {
            get
            {
                IntPtr NamePtr;
                Marshal.ThrowExceptionForHR(_AudioSessionControl.GetSessionIdentifier(out NamePtr));
                string res = Marshal.PtrToStringAuto(NamePtr);
                Marshal.FreeCoTaskMem(NamePtr);
                return res;
            }
        }

        public string SessionInstanceIdentifier
        {
            get
            {
                IntPtr NamePtr;
                Marshal.ThrowExceptionForHR(_AudioSessionControl.GetSessionInstanceIdentifier(out NamePtr));
                string res = Marshal.PtrToStringAuto(NamePtr);
                Marshal.FreeCoTaskMem(NamePtr);
                return res;
            }
        }

        public uint ProcessID
        {
            get
            {
                uint pid;
                Marshal.ThrowExceptionForHR(_AudioSessionControl.GetProcessId(out pid));
                return pid;
            }
        }

        public bool IsSystemIsSystemSoundsSession
        {
            get
            {
                return (_AudioSessionControl.IsSystemSoundsSession() == 0);  //S_OK
            }

        }


    }
}
