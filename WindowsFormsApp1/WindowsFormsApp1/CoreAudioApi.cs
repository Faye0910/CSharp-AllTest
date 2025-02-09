﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public static class CoreAudioApi
    {
        public static async Task Mute(float volumeStep = 0.05f, int millisDelay = 50)
        {
            var endpointVolume = GetAudioEndpointVolume();
            var dummy = Guid.Empty;

            bool masterVolumeMute;
            float masterVolumeLevel;
            endpointVolume.GetMasterVolumeLevelScalar(out masterVolumeLevel);
            endpointVolume.GetMute(out masterVolumeMute);

            if (masterVolumeMute)
            {
                endpointVolume.SetMasterVolumeLevelScalar(0f, ref dummy);
                endpointVolume.SetMute(false, ref dummy);

                if (masterVolumeLevel == 0f)
                    masterVolumeLevel = 1;

                float currentMasterVolumeLevel = 0f;
                while (currentMasterVolumeLevel < masterVolumeLevel)
                {
                    currentMasterVolumeLevel += volumeStep;
                    currentMasterVolumeLevel = Math.Min(currentMasterVolumeLevel, masterVolumeLevel);

                    endpointVolume.SetMasterVolumeLevelScalar(currentMasterVolumeLevel, ref dummy);
                    await Task.Delay(millisDelay);
                }
            }
            else
            {
                float currentMasterVolumeLevel = masterVolumeLevel;
                while (currentMasterVolumeLevel > 0f)
                {
                    currentMasterVolumeLevel -= volumeStep;
                    currentMasterVolumeLevel = Math.Max(currentMasterVolumeLevel, 0);

                    endpointVolume.SetMasterVolumeLevelScalar(currentMasterVolumeLevel, ref dummy);
                    await Task.Delay(millisDelay);
                }

                endpointVolume.SetMute(true, ref dummy);
                endpointVolume.SetMasterVolumeLevelScalar(masterVolumeLevel, ref dummy);
            }
        }

        public static async Task MuteProcess(string processName, float volumeStep = 0.05f, int millisDelay = 50)
        {
            var session = GetProcessSession(processName);
            var volume = session._ctl as ISimpleAudioVolume;
            var dummy = Guid.Empty;

            bool masterVolumeMute;
            float masterVolumeLevel;
            volume.GetMasterVolume(out masterVolumeLevel);
            volume.GetMute(out masterVolumeMute);

            if (masterVolumeMute)
            {
                volume.SetMasterVolume(0f, dummy);
                volume.SetMute(false, dummy);

                if (masterVolumeLevel == 0f)
                    masterVolumeLevel = 1;

                float currentMasterVolumeLevel = 0f;
                while (currentMasterVolumeLevel < masterVolumeLevel)
                {
                    currentMasterVolumeLevel += volumeStep;
                    currentMasterVolumeLevel = Math.Min(currentMasterVolumeLevel, masterVolumeLevel);

                    volume.SetMasterVolume(currentMasterVolumeLevel, dummy);
                    await Task.Delay(millisDelay);
                }
            }
            else
            {
                float currentMasterVolumeLevel = masterVolumeLevel;
                while (currentMasterVolumeLevel > 0f)
                {
                    currentMasterVolumeLevel -= volumeStep;
                    currentMasterVolumeLevel = Math.Max(currentMasterVolumeLevel, 0);

                    volume.SetMasterVolume(currentMasterVolumeLevel, dummy);
                    await Task.Delay(millisDelay);
                }

                volume.SetMute(true, dummy);
                volume.SetMasterVolume(masterVolumeLevel, dummy);
            }
        }

        public static IList<AudioDevice> GetAllDevices()
        {
            List<AudioDevice> list = new List<AudioDevice>();
            IMMDeviceEnumerator deviceEnumerator = null;
            try
            {
                deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            }
            catch
            {
            }
            if (deviceEnumerator == null)
                return list;

            IMMDeviceCollection collection;
            deviceEnumerator.EnumAudioEndpoints(EDataFlow.eAll, DEVICE_STATE.MASK_ALL, out collection);
            if (collection == null)
                return list;

            int count;
            collection.GetCount(out count);
            for (int i = 0; i < count; i++)
            {
                IMMDevice dev;
                collection.Item(i, out dev);
                if (dev != null)
                {
                    list.Add(CreateDevice(dev));
                }
            }
            return list;
        }

        public static IList<AudioSession> GetAllSessions()
        {
            List<AudioSession> list = new List<AudioSession>();
            IAudioSessionManager2 mgr = GetAudioSessionManager();
            if (mgr == null)
                return list;

            IAudioSessionEnumerator sessionEnumerator;
            mgr.GetSessionEnumerator(out sessionEnumerator);
            int count;
            sessionEnumerator.GetCount(out count);

            for (int i = 0; i < count; i++)
            {
                IAudioSessionControl ctl;
                sessionEnumerator.GetSession(i, out ctl);
                if (ctl == null)
                    continue;

                IAudioSessionControl2 ctl2 = ctl as IAudioSessionControl2;
                if (ctl2 != null)
                {
                    list.Add(new AudioSession(ctl2));
                }
            }
            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            return list;
        }

        public static AudioSession GetProcessSession(string name = null)
        {
            var sessions = GetAllSessions();
            AudioSession session = null;

            if (!string.IsNullOrEmpty(name))
                session = sessions.FirstOrDefault(s => s.Process?.ProcessName == name);

            if (session == null)
            {
                int id = Process.GetCurrentProcess().Id;
                session = sessions.FirstOrDefault(s => s.Process?.Id == id);
            }

            return session;
        }

        public static AudioDevice GetSpeakersDevice()
        {
            return CreateDevice(GetSpeakers());
        }

        private static IAudioSessionManager2 GetAudioSessionManager()
        {
            IMMDevice speakers = GetSpeakers();
            if (speakers == null)
                return null;

            // win7+ only
            object o;
            if (speakers.Activate(typeof(IAudioSessionManager2).GUID, CLSCTX.CLSCTX_ALL, IntPtr.Zero, out o) != 0 || o == null)
                return null;

            return o as IAudioSessionManager2;
        }

        private static IAudioEndpointVolume GetAudioEndpointVolume()
        {
            IMMDevice speakers = GetSpeakers();
            if (speakers == null)
                return null;

            // win7+ only
            object o;
            if (speakers.Activate(typeof(IAudioEndpointVolume).GUID, CLSCTX.CLSCTX_ALL, IntPtr.Zero, out o) != 0 || o == null)
                return null;

            return o as IAudioEndpointVolume;
        }

        private static AudioDevice CreateDevice(IMMDevice dev)
        {

            if (dev == null)
                return null;

            string id;
            dev.GetId(out id);
            DEVICE_STATE state;
            dev.GetState(out state);
            Dictionary<string, object> properties = new Dictionary<string, object>();
            IPropertyStore store;
            dev.OpenPropertyStore(STGM.STGM_READ, out store);
            if (store != null)
            {
                int propCount;
                store.GetCount(out propCount);
                for (int j = 0; j < propCount; j++)
                {
                    PROPERTYKEY pk;
                    if (store.GetAt(j, out pk) == 0)
                    {
                        PROPVARIANT value = new PROPVARIANT();
                        int hr = store.GetValue(ref pk, ref value);
                        object v = value.GetValue();
                        try
                        {
                            if (value.vt != VARTYPE.VT_BLOB) // for some reason, this fails?
                            {
                                PropVariantClear(ref value);
                            }
                        }
                        catch
                        {
                        }
                        string name = pk.ToString();
                        properties[name] = v;
                    }
                }
            }
            return new AudioDevice(id, (AudioDeviceState)state, properties);
        }

        private static IMMDevice GetSpeakers()
        {
            // get the speakers (1st render + multimedia) device
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
                IMMDevice speakers;
                deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);
                return speakers;
            }
            catch
            {
                return null;
            }
        }

        #region Classes

        public sealed class AudioSession : IDisposable
        {
            internal CoreAudioApi.IAudioSessionControl2 _ctl;
            private Process _process;

            internal AudioSession(CoreAudioApi.IAudioSessionControl2 ctl)
            {
                _ctl = ctl;
            }

            public float Volume { get; set; }
            public bool IsMute { get; set; }

            public Process Process
            {
                get
                {
                    if (_process == null && ProcessId != 0)
                    {
                        try
                        {
                            _process = Process.GetProcessById(ProcessId);
                        }
                        catch
                        {
                            // do nothing
                        }
                    }
                    return _process;
                }
            }

            public int ProcessId
            {
                get
                {
                    CheckDisposed();
                    int i;
                    _ctl.GetProcessId(out i);
                    return i;
                }
            }

            public string Identifier
            {
                get
                {
                    CheckDisposed();
                    string s;
                    _ctl.GetSessionIdentifier(out s);
                    return s;
                }
            }

            public string InstanceIdentifier
            {
                get
                {
                    CheckDisposed();
                    string s;
                    _ctl.GetSessionInstanceIdentifier(out s);
                    return s;
                }
            }

            public AudioSessionState State
            {
                get
                {
                    CheckDisposed();
                    AudioSessionState s;
                    _ctl.GetState(out s);
                    return s;
                }
            }

            public Guid GroupingParam
            {
                get
                {
                    CheckDisposed();
                    Guid g;
                    _ctl.GetGroupingParam(out g);
                    return g;
                }
                set
                {
                    CheckDisposed();
                    _ctl.SetGroupingParam(value, Guid.Empty);
                }
            }

            public string DisplayName
            {
                get
                {
                    CheckDisposed();
                    string s;
                    _ctl.GetDisplayName(out s);
                    return s;
                }
                set
                {
                    CheckDisposed();
                    string s;
                    _ctl.GetDisplayName(out s);
                    if (s != value)
                    {
                        _ctl.SetDisplayName(value, Guid.Empty);
                    }
                }
            }

            public string IconPath
            {
                get
                {
                    CheckDisposed();
                    string s;
                    _ctl.GetIconPath(out s);
                    return s;
                }
                set
                {
                    CheckDisposed();
                    string s;
                    _ctl.GetIconPath(out s);
                    if (s != value)
                    {
                        _ctl.SetIconPath(value, Guid.Empty);
                    }
                }
            }

            private void CheckDisposed()
            {
                if (_ctl == null)
                    throw new ObjectDisposedException("Control");
            }

            public override string ToString()
            {
                string s = DisplayName;
                if (!string.IsNullOrEmpty(s))
                    return "DisplayName: " + s;

                if (Process != null)
                    return "Process: " + Process.ProcessName;

                return "Pid: " + ProcessId;
            }

            public void Dispose()
            {
                if (_ctl != null)
                {
                    Marshal.ReleaseComObject(_ctl);
                    _ctl = null;
                }
            }
        }

        public sealed class AudioDevice
        {
            internal AudioDevice(string id, AudioDeviceState state, IDictionary<string, object> properties)
            {
                Id = id;
                State = state;
                Properties = properties;
            }

            public string Id { get; private set; }
            public AudioDeviceState State { get; private set; }
            public IDictionary<string, object> Properties { get; private set; }

            public string Description
            {
                get
                {
                    const string PKEY_Device_DeviceDesc = "{a45c254e-df1c-4efd-8020-67d146a850e0} 2";
                    object value;
                    Properties.TryGetValue(PKEY_Device_DeviceDesc, out value);
                    return string.Format("{0}", value);
                }
            }

            public string ContainerId
            {
                get
                {
                    const string PKEY_Devices_ContainerId = "{8c7ed206-3f8a-4827-b3ab-ae9e1faefc6c} 2";
                    object value;
                    Properties.TryGetValue(PKEY_Devices_ContainerId, out value);
                    return string.Format("{0}", value);
                }
            }

            public string EnumeratorName
            {
                get
                {
                    const string PKEY_Device_EnumeratorName = "{a45c254e-df1c-4efd-8020-67d146a850e0} 24";
                    object value;
                    Properties.TryGetValue(PKEY_Device_EnumeratorName, out value);
                    return string.Format("{0}", value);
                }
            }

            public string InterfaceFriendlyName
            {
                get
                {
                    const string DEVPKEY_DeviceInterface_FriendlyName = "{026e516e-b814-414b-83cd-856d6fef4822} 2";
                    object value;
                    Properties.TryGetValue(DEVPKEY_DeviceInterface_FriendlyName, out value);
                    return string.Format("{0}", value);
                }
            }

            public string FriendlyName
            {
                get
                {
                    const string DEVPKEY_Device_FriendlyName = "{a45c254e-df1c-4efd-8020-67d146a850e0} 14";
                    object value;
                    Properties.TryGetValue(DEVPKEY_Device_FriendlyName, out value);
                    return string.Format("{0}", value);
                }
            }

            public override string ToString()
            {
                return FriendlyName;
            }
        }

        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        private class MMDeviceEnumerator
        {
        }

        #endregion 

        #region Enums and Structs

        public enum AudioSessionState
        {
            Inactive = 0,
            Active = 1,
            Expired = 2
        }

        public enum AudioDeviceState
        {
            Active = 0x1,
            Disabled = 0x2,
            NotPresent = 0x4,
            Unplugged = 0x8,
        }

        public enum AudioSessionDisconnectReason
        {
            DisconnectReasonDeviceRemoval = 0,
            DisconnectReasonServerShutdown = 1,
            DisconnectReasonFormatChanged = 2,
            DisconnectReasonSessionLogoff = 3,
            DisconnectReasonSessionDisconnected = 4,
            DisconnectReasonExclusiveModeOverride = 5
        }

        [Flags]
        private enum CLSCTX
        {
            CLSCTX_INPROC_SERVER = 0x1,
            CLSCTX_INPROC_HANDLER = 0x2,
            CLSCTX_LOCAL_SERVER = 0x4,
            CLSCTX_REMOTE_SERVER = 0x10,
            CLSCTX_ALL = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER
        }

        private enum STGM
        {
            STGM_READ = 0x00000000,
        }

        private enum EDataFlow
        {
            eRender,
            eCapture,
            eAll,
        }

        private enum ERole
        {
            eConsole,
            eMultimedia,
            eCommunications,
        }

        private enum DEVICE_STATE
        {
            ACTIVE = 0x00000001,
            DISABLED = 0x00000002,
            NOTPRESENT = 0x00000004,
            UNPLUGGED = 0x00000008,
            MASK_ALL = 0x0000000F
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROPERTYKEY
        {
            public Guid fmtid;
            public int pid;

            public override string ToString()
            {
                return fmtid.ToString("B") + " " + pid;
            }
        }

        // NOTE: we only define what we handle
        [Flags]
        private enum VARTYPE : short
        {
            VT_I4 = 3,
            VT_BOOL = 11,
            VT_UI4 = 19,
            VT_LPWSTR = 31,
            VT_BLOB = 65,
            VT_CLSID = 72,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROPVARIANT
        {
            public VARTYPE vt;
            public ushort wReserved1;
            public ushort wReserved2;
            public ushort wReserved3;
            public PROPVARIANTunion union;

            public object GetValue()
            {
                switch (vt)
                {
                    case VARTYPE.VT_BOOL:
                        return union.boolVal != 0;

                    case VARTYPE.VT_LPWSTR:
                        return Marshal.PtrToStringUni(union.pwszVal);

                    case VARTYPE.VT_UI4:
                        return union.lVal;

                    case VARTYPE.VT_CLSID:
                        return (Guid)Marshal.PtrToStructure(union.puuid, typeof(Guid));

                    default:
                        return vt.ToString() + ":?";
                }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct PROPVARIANTunion
        {
            [FieldOffset(0)]
            public int lVal;
            [FieldOffset(0)]
            public ulong uhVal;
            [FieldOffset(0)]
            public short boolVal;
            [FieldOffset(0)]
            public IntPtr pwszVal;
            [FieldOffset(0)]
            public IntPtr puuid;
        }

        #endregion

        #region Interfaces

        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDeviceEnumerator
        {
            [PreserveSig]
            int EnumAudioEndpoints(EDataFlow dataFlow, DEVICE_STATE dwStateMask, out IMMDeviceCollection ppDevices);

            [PreserveSig]
            int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppEndpoint);

            [PreserveSig]
            int GetDevice([MarshalAs(UnmanagedType.LPWStr)] string pwstrId, out IMMDevice ppDevice);

            [PreserveSig]
            int RegisterEndpointNotificationCallback(IMMNotificationClient pClient);

            [PreserveSig]
            int UnregisterEndpointNotificationCallback(IMMNotificationClient pClient);
        }

        [Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMNotificationClient
        {
            void OnDeviceStateChanged([MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId, DEVICE_STATE dwNewState);
            void OnDeviceAdded([MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);
            void OnDeviceRemoved([MarshalAs(UnmanagedType.LPWStr)] string deviceId);
            void OnDefaultDeviceChanged(EDataFlow flow, ERole role, string pwstrDefaultDeviceId);
            void OnPropertyValueChanged([MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId, PROPERTYKEY key);
        }

        [Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDeviceCollection
        {
            [PreserveSig]
            int GetCount(out int pcDevices);

            [PreserveSig]
            int Item(int nDevice, out IMMDevice ppDevice);
        }

        [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDevice
        {
            [PreserveSig]
            int Activate([MarshalAs(UnmanagedType.LPStruct)] Guid riid, CLSCTX dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);

            [PreserveSig]
            int OpenPropertyStore(STGM stgmAccess, out IPropertyStore ppProperties);

            [PreserveSig]
            int GetId([MarshalAs(UnmanagedType.LPWStr)] out string ppstrId);

            [PreserveSig]
            int GetState(out DEVICE_STATE pdwState);
        }

        [Guid("6f79d558-3e96-4549-a1d1-7d75d2288814"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPropertyDescription
        {
            [PreserveSig]
            int GetPropertyKey(out PROPERTYKEY pkey);

            [PreserveSig]
            int GetCanonicalName(out IntPtr ppszName);

            [PreserveSig]
            int GetPropertyType(out short pvartype);

            [PreserveSig]
            int GetDisplayName(out IntPtr ppszName);

            // WARNING: the rest is undefined. you *can't* implement it, only use it.
        }

        [Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPropertyStore
        {
            [PreserveSig]
            int GetCount(out int cProps);

            [PreserveSig]
            int GetAt(int iProp, out PROPERTYKEY pkey);

            [PreserveSig]
            int GetValue(ref PROPERTYKEY key, ref PROPVARIANT pv);

            [PreserveSig]
            int SetValue(ref PROPERTYKEY key, ref PROPVARIANT propvar);

            [PreserveSig]
            int Commit();
        }

        [Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionManager
        {
            [PreserveSig]
            int GetAudioSessionControl([MarshalAs(UnmanagedType.LPStruct)] Guid AudioSessionGuid, int StreamFlags, out IAudioSessionControl SessionControl);

            [PreserveSig]
            int GetSimpleAudioVolume([MarshalAs(UnmanagedType.LPStruct)] Guid AudioSessionGuid, int StreamFlags, ISimpleAudioVolume AudioVolume);
        }

        [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionManager2
        {
            [PreserveSig]
            int GetAudioSessionControl([MarshalAs(UnmanagedType.LPStruct)] Guid AudioSessionGuid, int StreamFlags, out IAudioSessionControl SessionControl);

            [PreserveSig]
            int GetSimpleAudioVolume([MarshalAs(UnmanagedType.LPStruct)] Guid AudioSessionGuid, int StreamFlags, ISimpleAudioVolume AudioVolume);

            [PreserveSig]
            int GetSessionEnumerator(out IAudioSessionEnumerator SessionEnum);

            [PreserveSig]
            int RegisterSessionNotification(IAudioSessionNotification SessionNotification);

            [PreserveSig]
            int UnregisterSessionNotification(IAudioSessionNotification SessionNotification);

            int RegisterDuckNotificationNotImpl();
            int UnregisterDuckNotificationNotImpl();
        }

        [Guid("641DD20B-4D41-49CC-ABA3-174B9477BB08"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionNotification
        {
            void OnSessionCreated(IAudioSessionControl NewSession);
        }

        [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioSessionEnumerator
        {
            [PreserveSig]
            int GetCount(out int SessionCount);

            [PreserveSig]
            int GetSession(int SessionCount, out IAudioSessionControl Session);
        }

        [Guid("bfb7ff88-7239-4fc9-8fa2-07c950be9c6d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioSessionControl2
        {
            // IAudioSessionControl
            [PreserveSig]
            int GetState(out AudioSessionState pRetVal);

            [PreserveSig]
            int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

            [PreserveSig]
            int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)]string Value, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

            [PreserveSig]
            int GetIconPath([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

            [PreserveSig]
            int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string Value, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

            [PreserveSig]
            int GetGroupingParam(out Guid pRetVal);

            [PreserveSig]
            int SetGroupingParam([MarshalAs(UnmanagedType.LPStruct)] Guid Override, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

            [PreserveSig]
            int RegisterAudioSessionNotification(IAudioSessionEvents NewNotifications);

            [PreserveSig]
            int UnregisterAudioSessionNotification(IAudioSessionEvents NewNotifications);

            // IAudioSessionControl2
            [PreserveSig]
            int GetSessionIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

            [PreserveSig]
            int GetSessionInstanceIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

            [PreserveSig]
            int GetProcessId(out int pRetVal);

            [PreserveSig]
            int IsSystemSoundsSession();

            [PreserveSig]
            int SetDuckingPreference(bool optOut);
        }

        [Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioSessionControl
        {
            [PreserveSig]
            int GetState(out AudioSessionState pRetVal);

            [PreserveSig]
            int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

            [PreserveSig]
            int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)]string Value, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

            [PreserveSig]
            int GetIconPath([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

            [PreserveSig]
            int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string Value, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

            [PreserveSig]
            int GetGroupingParam(out Guid pRetVal);

            [PreserveSig]
            int SetGroupingParam([MarshalAs(UnmanagedType.LPStruct)] Guid Override, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

            [PreserveSig]
            int RegisterAudioSessionNotification(IAudioSessionEvents NewNotifications);

            [PreserveSig]
            int UnregisterAudioSessionNotification(IAudioSessionEvents NewNotifications);
        }

        [Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioSessionEvents
        {
            void OnDisplayNameChanged([MarshalAs(UnmanagedType.LPWStr)] string NewDisplayName, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);
            void OnIconPathChanged([MarshalAs(UnmanagedType.LPWStr)] string NewIconPath, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);
            void OnSimpleVolumeChanged(float NewVolume, bool NewMute, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);
            void OnChannelVolumeChanged(int ChannelCount, IntPtr NewChannelVolumeArray, int ChangedChannel, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);
            void OnGroupingParamChanged([MarshalAs(UnmanagedType.LPStruct)] Guid NewGroupingParam, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);
            void OnStateChanged(AudioSessionState NewState);
            void OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason);
        }

        [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface ISimpleAudioVolume
        {
            [PreserveSig]
            int SetMasterVolume(float fLevel, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

            [PreserveSig]
            int GetMasterVolume(out float pfLevel);

            [PreserveSig]
            int SetMute(bool bMute, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

            [PreserveSig]
            int GetMute(out bool pbMute);
        }

        [Guid("657804FA-D6AD-4496-8A60-352752AF4F89"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioEndpointVolumeCallback
        {
            void OnNotify(IntPtr notifyData);
        };

        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioEndpointVolume
        {
            int RegisterControlChangeNotify(IAudioEndpointVolumeCallback pNotify);
            int UnregisterControlChangeNotify(IAudioEndpointVolumeCallback pNotify);
            int GetChannelCount(out int pnChannelCount);
            int SetMasterVolumeLevel(float fLevelDB, ref Guid pguidEventContext);
            int SetMasterVolumeLevelScalar(float fLevel, ref Guid pguidEventContext);
            int GetMasterVolumeLevel(out float pfLevelDB);
            int GetMasterVolumeLevelScalar(out float pfLevel);
            int SetChannelVolumeLevel(uint nChannel, float fLevelDB, ref Guid pguidEventContext);
            int SetChannelVolumeLevelScalar(uint nChannel, float fLevel, ref Guid pguidEventContext);
            int GetChannelVolumeLevel(uint nChannel, out float pfLevelDB);
            int GetChannelVolumeLevelScalar(uint nChannel, out float pfLevel);
            int SetMute([MarshalAs(UnmanagedType.Bool)] Boolean bMute, ref Guid pguidEventContext);
            int GetMute(out bool pbMute);
            int GetVolumeStepInfo(out uint pnStep, out uint pnStepCount);
            int VolumeStepUp(ref Guid pguidEventContext);
            int VolumeStepDown(ref Guid pguidEventContext);
            int QueryHardwareSupport(out uint pdwHardwareSupportMask);
            int GetVolumeRange(out float pflVolumeMindB, out float pflVolumeMaxdB, out float pflVolumeIncrementdB);
        }

        #endregion 

        #region Pinvokes

        [DllImport("ole32.dll")]
        private static extern int PropVariantClear(ref PROPVARIANT pvar);

        #endregion 
    }

}
