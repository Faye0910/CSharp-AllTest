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
using System.Text;
using CoreAudioApi.Interfaces;
using System.Runtime.InteropServices;

namespace CoreAudioApi
{
    [Flags]
    public enum EDeviceState : uint
    {
        DEVICE_STATE_ACTIVE = 0x00000001,
        DEVICE_STATE_UNPLUGGED = 0x00000002,
        DEVICE_STATE_NOTPRESENT = 0x00000004,
        DEVICE_STATEMASK_ALL = 0x00000007
    }

    [Flags]
    public enum EEndpointHardwareSupport
    {
        Volume = 0x00000001,
        Mute = 0x00000002,
        Meter = 0x00000004
    }

    public enum ERole
    {
        eConsole = 0,
        eMultimedia = 1,
        eCommunications = 2,
        ERole_enum_count = 3
    }

    public enum EDataFlow
    {
        eRender = 0,
        eCapture = 1,
        eAll = 2,
        EDataFlow_enum_count = 3
    }

    public class AudioEndpointVolume : IDisposable
    {
        private IAudioEndpointVolume _AudioEndPointVolume;
        private AudioEndpointVolumeChannels _Channels;
        private AudioEndpointVolumeStepInformation _StepInformation;
        private AudioEndPointVolumeVolumeRange _VolumeRange;
        private EEndpointHardwareSupport _HardwareSupport;
        private AudioEndpointVolumeCallback _CallBack;
        public  event AudioEndpointVolumeNotificationDelegate OnVolumeNotification;

        public AudioEndPointVolumeVolumeRange VolumeRange
        {
            get
            {
                return _VolumeRange;
            }
        }
        public EEndpointHardwareSupport HardwareSupport
        {
            get
            {
                return _HardwareSupport;
            }
        }
        public AudioEndpointVolumeStepInformation StepInformation
        {
            get
            {
                return _StepInformation;
            }
        }
        public AudioEndpointVolumeChannels Channels
        {
            get
            {
                return _Channels;
            }
        }
        public float MasterVolumeLevel
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(_AudioEndPointVolume.GetMasterVolumeLevel(out result));
                return result;
            }
            set
            {
                Marshal.ThrowExceptionForHR(_AudioEndPointVolume.SetMasterVolumeLevel(value, Guid.Empty));
            }
        }
        public float MasterVolumeLevelScalar
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(_AudioEndPointVolume.GetMasterVolumeLevelScalar(out result));
                return result;
            }
            set
            {
                Marshal.ThrowExceptionForHR(_AudioEndPointVolume.SetMasterVolumeLevelScalar(value, Guid.Empty));
            }
        }
        public bool Mute
        {
            get
            {
                bool result;
                Marshal.ThrowExceptionForHR(_AudioEndPointVolume.GetMute(out result));
                return result;
            }
            set
            {
                Marshal.ThrowExceptionForHR(_AudioEndPointVolume.SetMute(value, Guid.Empty));
            }
        }
        public void VolumeStepUp()
        {
            Marshal.ThrowExceptionForHR(_AudioEndPointVolume.VolumeStepUp(Guid.Empty));
        }
        public void VolumeStepDown()
        {
            Marshal.ThrowExceptionForHR(_AudioEndPointVolume.VolumeStepDown(Guid.Empty));
        }
        internal AudioEndpointVolume(IAudioEndpointVolume realEndpointVolume)
        {
            uint HardwareSupp;

            _AudioEndPointVolume = realEndpointVolume;
            _Channels = new AudioEndpointVolumeChannels(_AudioEndPointVolume);
            _StepInformation = new AudioEndpointVolumeStepInformation(_AudioEndPointVolume);
            Marshal.ThrowExceptionForHR(_AudioEndPointVolume.QueryHardwareSupport(out HardwareSupp));
            _HardwareSupport = (EEndpointHardwareSupport)HardwareSupp;
            _VolumeRange = new AudioEndPointVolumeVolumeRange(_AudioEndPointVolume);
            _CallBack = new AudioEndpointVolumeCallback(this);
            Marshal.ThrowExceptionForHR(_AudioEndPointVolume.RegisterControlChangeNotify( _CallBack));
        }
        internal void FireNotification(AudioVolumeNotificationData NotificationData)
        {
            AudioEndpointVolumeNotificationDelegate del = OnVolumeNotification;
            if (del != null)
            {
                del(NotificationData);
            }
        }
        #region IDisposable Members

        public void Dispose()
        {
            if (_CallBack != null)
            {
                Marshal.ThrowExceptionForHR(_AudioEndPointVolume.UnregisterControlChangeNotify( _CallBack ));
                _CallBack = null;
            }
        }

        ~AudioEndpointVolume()
        {
            Dispose();
        }

        #endregion
       
    }

    internal class AudioEndpointVolumeCallback : IAudioEndpointVolumeCallback
    {
        private AudioEndpointVolume _Parent;

        internal AudioEndpointVolumeCallback(AudioEndpointVolume parent)
        {
            _Parent = parent;
        }

        [PreserveSig]
        public int OnNotify(IntPtr NotifyData)
        {
            //Since AUDIO_VOLUME_NOTIFICATION_DATA is dynamic in length based on the
            //number of audio channels available we cannot just call PtrToStructure 
            //to get all data, thats why it is split up into two steps, first the static
            //data is marshalled into the data structure, then with some IntPtr math the
            //remaining floats are read from memory.
            //
            AUDIO_VOLUME_NOTIFICATION_DATA data = (AUDIO_VOLUME_NOTIFICATION_DATA)Marshal.PtrToStructure(NotifyData, typeof(AUDIO_VOLUME_NOTIFICATION_DATA));

            //Determine offset in structure of the first float
            IntPtr Offset = Marshal.OffsetOf(typeof(AUDIO_VOLUME_NOTIFICATION_DATA), "ChannelVolume");
            //Determine offset in memory of the first float
            IntPtr FirstFloatPtr = (IntPtr)((long)NotifyData + (long)Offset);

            float[] voldata = new float[data.nChannels];

            //Read all floats from memory.
            for (int i = 0; i < data.nChannels; i++)
            {
                voldata[i] = (float)Marshal.PtrToStructure(FirstFloatPtr, typeof(float));
            }

            //Create combined structure and Fire Event in parent class.
            AudioVolumeNotificationData NotificationData = new AudioVolumeNotificationData(data.guidEventContext, data.bMuted, data.fMasterVolume, voldata);
            _Parent.FireNotification(NotificationData);
            return 0; //S_OK
        }
    }

    public class AudioEndpointVolumeChannel
    {
        private uint _Channel;
        private IAudioEndpointVolume _AudioEndpointVolume;

        internal AudioEndpointVolumeChannel(IAudioEndpointVolume parent, int channel)
        {
            _Channel = (uint)channel;
            _AudioEndpointVolume = parent;
        }

        public float VolumeLevel
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(_AudioEndpointVolume.GetChannelVolumeLevel(_Channel, out result));
                return result;
            }
            set
            {
                Marshal.ThrowExceptionForHR(_AudioEndpointVolume.SetChannelVolumeLevel(_Channel, value, Guid.Empty));
            }
        }

        public float VolumeLevelScalar
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(_AudioEndpointVolume.GetChannelVolumeLevelScalar(_Channel, out result));
                return result;
            }
            set
            {
                Marshal.ThrowExceptionForHR(_AudioEndpointVolume.SetChannelVolumeLevelScalar(_Channel, value, Guid.Empty));
            }
        }

    }

    public delegate void AudioEndpointVolumeNotificationDelegate(AudioVolumeNotificationData data);

    public class AudioEndpointVolumeChannels
    {
        IAudioEndpointVolume _AudioEndPointVolume;
        AudioEndpointVolumeChannel[] _Channels;
        public int Count
        {
            get
            {
                int result;
                Marshal.ThrowExceptionForHR(_AudioEndPointVolume.GetChannelCount(out result));
                return result;
            }
        }

        public AudioEndpointVolumeChannel this[int index]
        {
            get
            {
                return _Channels[index];
            }
        }

        internal AudioEndpointVolumeChannels(IAudioEndpointVolume parent)
        {
            int ChannelCount;
            _AudioEndPointVolume = parent;

            ChannelCount = Count;
            _Channels = new AudioEndpointVolumeChannel[ChannelCount];
            for (int i = 0; i < ChannelCount; i++)
            {
                _Channels[i] = new AudioEndpointVolumeChannel(_AudioEndPointVolume, i);
            }
        }


    }
    public class AudioEndpointVolumeStepInformation
    {
        private uint _Step;
        private uint _StepCount;
        internal AudioEndpointVolumeStepInformation(IAudioEndpointVolume parent)
        {
            Marshal.ThrowExceptionForHR(parent.GetVolumeStepInfo(out _Step, out _StepCount));
        }

        public uint Step
        {
            get
            {
                return _Step;
            }
        }

        public uint StepCount
        {
            get
            {
                return _StepCount;
            }
        }

    }

    public class AudioEndPointVolumeVolumeRange
    {
        float _VolumeMindB;
        float _VolumeMaxdB;
        float _VolumeIncrementdB;

        internal AudioEndPointVolumeVolumeRange(IAudioEndpointVolume parent)
        {
            Marshal.ThrowExceptionForHR(parent.GetVolumeRange(out _VolumeMindB, out _VolumeMaxdB, out _VolumeIncrementdB));
        }

        public float MindB
        {
            get
            {
                return _VolumeMindB;
            }
        }

        public float MaxdB
        {
            get
            {
                return _VolumeMaxdB;
            }
        }

        public float IncrementdB
        {
            get
            {
                return _VolumeIncrementdB;
            }
        }
    }

    public class AudioMeterInformation
    {
        private IAudioMeterInformation _AudioMeterInformation;
        private EEndpointHardwareSupport _HardwareSupport;
        private AudioMeterInformationChannels _Channels;

        internal AudioMeterInformation(IAudioMeterInformation realInterface)
        {
            int HardwareSupp;

            _AudioMeterInformation = realInterface;
            Marshal.ThrowExceptionForHR(_AudioMeterInformation.QueryHardwareSupport(out HardwareSupp));
            _HardwareSupport = (EEndpointHardwareSupport)HardwareSupp;
            _Channels = new AudioMeterInformationChannels(_AudioMeterInformation);

        }

        public AudioMeterInformationChannels PeakValues
        {
            get
            {
                return _Channels;
            }
        }

        public EEndpointHardwareSupport HardwareSupport
        {
            get
            {
                return _HardwareSupport;
            }
        }

        public float MasterPeakValue
        {
            get
            {
                float result;
                Marshal.ThrowExceptionForHR(_AudioMeterInformation.GetPeakValue(out result));
                return result;
            }
        }
    }

    public class AudioMeterInformationChannels
    {
        IAudioMeterInformation _AudioMeterInformation;

        public int Count
        {
            get
            {
                int result;
                Marshal.ThrowExceptionForHR(_AudioMeterInformation.GetMeteringChannelCount(out result));
                return result;
            }
        }

        public float this[int index]
        {
            get
            {
                float[] peakValues = new float[Count];
                GCHandle Params = GCHandle.Alloc(peakValues, GCHandleType.Pinned);
                Marshal.ThrowExceptionForHR(_AudioMeterInformation.GetChannelsPeakValues(peakValues.Length, Params.AddrOfPinnedObject()));
                Params.Free();
                return peakValues[index];
            }
        }

        internal AudioMeterInformationChannels(IAudioMeterInformation parent)
        {
            _AudioMeterInformation = parent;
        }
    }
}
