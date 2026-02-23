using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Arcturus.DevHost.Hosting.Interop;

internal class JobObjectHost : IDisposable
{
    private IntPtr _jobHandle = IntPtr.Zero;
    private ILogger<OrchestratorHost>? _logger { get; }

    internal JobObjectHost(ILogger<OrchestratorHost>? logger)
    {
        _logger = logger;
    }

    public void Dispose()
    {
        if (_jobHandle != IntPtr.Zero)
        {
            CloseHandle(_jobHandle);
        }
    }

    internal void CreateJobObject()
    {
        // Create Windows Job Object - child processes will be automatically killed when parent dies
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        try
        {
            _jobHandle = CreateJobObjectW(IntPtr.Zero, null);

            if (_jobHandle == IntPtr.Zero)
            {
                _logger?.LogDebug("[OrchestratorHost] WARNING: Failed to create job object");
                return;
            }

            var info = new JOBOBJECT_BASIC_LIMIT_INFORMATION
            {
                LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE
            };

            var extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
            {
                BasicLimitInformation = info
            };

            int length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
            IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

            if (!SetInformationJobObject(_jobHandle, JobObjectInfoType.ExtendedLimitInformation,
                extendedInfoPtr, (uint)length))
            {
                _logger?.LogDebug("[OrchestratorHost] WARNING: Failed to set job object info");
            }
            else
            {
                _logger?.LogInformation("[OrchestratorHost] ✓ Job object created - child processes will be killed when parent dies");
            }

            Marshal.FreeHGlobal(extendedInfoPtr);
        }
        catch (Exception ex)
        {
            _logger?.LogDebug($"[OrchestratorHost] ERROR creating job object: {ex.Message}");
        }
    }

    internal void AssignProcessToJobObject(Process process)
    {
        // Add to job object - will be killed when this process dies
        if (!OperatingSystem.IsWindows() || _jobHandle == IntPtr.Zero)
        {
            return;
        }

        try
        {
            if (!AssignProcessToJobObject(_jobHandle, process.Handle))
            {
                _logger?.LogDebug($"[OrchestratorHost] WARNING: Failed to assign PID {process.Id} to job object");
            }
            else
            {
                _logger?.LogInformation($"[OrchestratorHost] ✓ PID {process.Id} assigned to job object");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogDebug($"[OrchestratorHost] ERROR assigning process to job: {ex.Message}");
        }
    }


    #region Windows Job Object P/Invoke

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    [SupportedOSPlatform("windows")]
    private static extern IntPtr CreateJobObjectW(IntPtr lpJobAttributes, string? lpName);

    [DllImport("kernel32.dll")]
    [SupportedOSPlatform("windows")]
    private static extern bool SetInformationJobObject(IntPtr hJob, JobObjectInfoType infoType,
        IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

    [DllImport("kernel32.dll", SetLastError = true)]
    [SupportedOSPlatform("windows")]
    private static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);

    [SupportedOSPlatform("windows")]
    private enum JobObjectInfoType
    {
        AssociateCompletionPortInformation = 7,
        BasicLimitInformation = 2,
        BasicUIRestrictions = 4,
        EndOfJobTimeInformation = 6,
        ExtendedLimitInformation = 9,
        SecurityLimitInformation = 5,
        GroupInformation = 11
    }

    [StructLayout(LayoutKind.Sequential)]
    [SupportedOSPlatform("windows")]
    private struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
        public long PerProcessUserTimeLimit;
        public long PerJobUserTimeLimit;
        public uint LimitFlags;
        public UIntPtr MinimumWorkingSetSize;
        public UIntPtr MaximumWorkingSetSize;
        public uint ActiveProcessLimit;
        public UIntPtr Affinity;
        public uint PriorityClass;
        public uint SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    [SupportedOSPlatform("windows")]
    private struct IO_COUNTERS
    {
        public ulong ReadOperationCount;
        public ulong WriteOperationCount;
        public ulong OtherOperationCount;
        public ulong ReadTransferCount;
        public ulong WriteTransferCount;
        public ulong OtherTransferCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    [SupportedOSPlatform("windows")]
    private struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
        public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
        public IO_COUNTERS IoInfo;
        public UIntPtr ProcessMemoryLimit;
        public UIntPtr JobMemoryLimit;
        public UIntPtr PeakProcessMemoryUsed;
        public UIntPtr PeakJobMemoryUsed;
    }

    private const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x00002000;

    
    #endregion
}
