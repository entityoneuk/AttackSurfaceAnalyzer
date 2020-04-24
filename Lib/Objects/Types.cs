﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
namespace AttackSurfaceAnalyzer.Types
{
    /// <summary>
    /// Specifies the child class type for a CollectObject
    /// </summary>
    public enum RESULT_TYPE
    {
        /// <summary>
        /// Indicates an Invalid CollectObject
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// See FileSystemObject
        /// </summary>
        FILE,
        /// <summary>
        /// See OpenPortObject
        /// </summary>
        PORT,
        /// <summary>
        /// See RegistryObject
        /// </summary>
        REGISTRY,
        /// <summary>
        /// See CertificateObject
        /// </summary>
        CERTIFICATE,
        /// <summary>
        /// See ServiceObject
        /// </summary>
        SERVICE,
        /// <summary>
        /// See UserAccountObject
        /// </summary>
        USER,
        /// <summary>
        /// See UserGroupObject
        /// </summary>
        GROUP,
        /// <summary>
        /// See FirewallObject
        /// </summary>
        FIREWALL,
        /// <summary>
        /// See ComObject
        /// </summary>
        COM,
        /// <summary>
        /// See EventLogObject
        /// </summary>
        LOG,
        /// <summary>
        /// See FileMonitorObject
        /// </summary>
        FILEMONITOR,
        /// <summary>
        /// A TPM Device
        /// </summary>
        TPM,
        /// <summary>
        /// A cryptographic key
        /// </summary>
        KEY
    };

    /// <summary>
    /// The change type of a CompareResult object.
    /// </summary>
    public enum CHANGE_TYPE
    {
        INVALID,
        CREATED,
        DELETED,
        MODIFIED,
        RENAMED
    }

    /// <summary>
    /// The running status of a Comparator or Collector
    /// </summary>
    public enum RUN_STATUS
    {
        NOT_STARTED,
        RUNNING,
        FAILED,
        COMPLETED,
        NO_RESULTS
    }

    /// <summary>
    /// Errors enum.
    /// </summary>
    public enum ASA_ERROR
    {
        NONE,
        UNIQUE_ID,
        INVALID_PATH,
        ALREADY_RUNNING,
        NO_COLLECTORS,
        MATCHING_SCHEMA,
        FAILED_TO_CREATE_DATABASE,
        INVALID_ID,
        INVALID_RULES,
        FAILED_TO_ESTABLISH_MAIN_DB_CONNECTION
    }


    /// <summary>
    /// These are the characteristics defined in the PE Header for Windows executables.
    /// See https://docs.microsoft.com/en-us/windows/win32/debug/pe-format for the oracle definitions of these values
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1717:Only FlagsAttribute enums should have plural names", Justification = "This is the official name for the enum.")]
    public enum DLLCHARACTERISTICS
    {
        /// <summary>
        /// 64 Bit ASLR
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_HIGH_ENTROPY_VA = 0x0020,
        /// <summary>
        /// ASLR
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_DYNAMIC_BASE = 0x0040,
        /// <summary>
        /// Don't run unless properly signed
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_FORCE_INTEGRITY = 0x0080,
        /// <summary>
        /// DEP
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NX_COMPAT = 0x0100,
        /// <summary>
        /// No Isolation
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NO_ISOLATION = 0x0200,
        /// <summary>
        /// No Exceptions
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NO_SEH = 0x0400,
        /// <summary>
        /// Do not bind the image
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_NO_BIND = 0x0800,
        /// <summary>
        /// Must execute in an app container
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_APPCONTAINER = 0x1000,
        /// <summary>
        /// Image is a WDM driver
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_WDM_DRIVER = 0x2000,
        /// <summary>
        /// Supports Control Flow Guard
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_GUARD_CF = 0x4000,
        /// <summary>
        /// Image is terminal server aware
        /// </summary>
        IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE = 0x8000
    }

    /// <summary>
    /// Flags available for analysis rules.
    /// </summary>
    public enum ANALYSIS_RESULT_TYPE
    {
        /// <summary>
        /// Lowest Level - 0
        /// </summary>
        NONE,
        /// <summary>
        /// 1
        /// </summary>
        VERBOSE,
        /// <summary>
        /// 2
        /// </summary>
        DEBUG,
        /// <summary>
        /// 3
        /// </summary>
        INFORMATION,
        /// <summary>
        /// 4
        /// </summary>
        WARNING,
        /// <summary>
        /// 5
        /// </summary>
        ERROR,
        /// <summary>
        /// 6 - Highest level
        /// </summary>
        FATAL
    }

    /// <summary>
    /// Operations available for Analysis rules.
    /// </summary>
    public enum OPERATION
    {
        /// <summary>
        /// Generates regular expressions from the Data list provided and tests them against the specified field.  If any match it is a success.
        /// </summary>
        REGEX,
        /// <summary>
        /// Checks that any value in the Data list or DictData dictionary have a match in the specified field's object as appropriate.
        /// </summary>
        EQ,
        /// <summary>
        /// Checks that any value in the Data list or DictData dictionary does not have a match in the specified field's object as appropriate.
        /// </summary>
        NEQ,
        /// <summary>
        /// Checks whether the specified fields value when parsed as an int is less than first value in the Data list as Parsed as an Int
        /// </summary>
        LT,
        /// <summary>
        /// Checks whether the specified fields value when parsed as an int is greater than first value in the Data list as Parsed as an Int
        /// </summary>
        GT,
        /// <summary>
        /// Checks if the specified fields values contain all of the data in the Data list or DictData dictionary as appropriate for the field.
        /// </summary>
        CONTAINS,
        /// <summary>
        /// Checks if the specified fields values does not contain any of the data in the Data list or DictData dictionary as appropriate for the field.
        /// </summary>
        DOES_NOT_CONTAIN,
        /// <summary>
        /// Checks if the specified field was modified between the two runs.
        /// </summary>
        WAS_MODIFIED,
        /// <summary>
        /// Checks if the specified field ends with any of the strings in the Data list.
        /// </summary>
        ENDS_WITH,
        /// <summary>
        /// Checks if the specified field starts with any of the strings in the Data list.
        /// </summary>
        STARTS_WITH,
        /// <summary>
        /// Checks if the specified fields values contain any of the data in the Data list or DictData dictionary as appropriate for the field.
        /// </summary>
        CONTAINS_ANY,
        /// <summary>
        /// Checks if the specified fields values does not contain all of the data in the Data list or DictData dictionary as appropriate for the field.
        /// </summary>
        DOES_NOT_CONTAIN_ALL,
        /// <summary>
        /// Checks if the specified field is null in both runs.
        /// </summary>
        IS_NULL,
        /// <summary>
        /// Checks if the specified field is true in either run.
        /// </summary>
        IS_TRUE,
        /// <summary>
        /// Checks if the specified field, as parsed as time, is before the time specified in the first entry of the Data list
        /// </summary>
        IS_BEFORE,
        /// <summary>
        /// Checks if the specified field, as parsed as time, is after the time specified in the first entry of the Data list
        /// </summary>
        IS_AFTER,
        /// <summary>
        /// Checks if the specified field, as parsed as time, is before DateTime.Now.
        /// </summary>
        IS_EXPIRED
    }

    /// <summary>
    /// Platform definitions for Analysis rules.
    /// </summary>
    public enum PLATFORM
    {
        WINDOWS,
        LINUX,
        MACOS,
        UNKNOWN
    }

    public enum RUN_TYPE
    {
        COLLECT,
        MONITOR,
        COMPARE
    }

    public enum BOOL_OPERATOR
    {
        AND,
        OR,
        XOR,
        NAND,
        NOR,
        NOT
    }

    public enum TRANSPORT
    {
        UNKNOWN,
        TCP,
        UDP
    }

    /// <summary>
    /// From https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.addressfamily?view=netcore-3.1
    /// </summary>
    public enum ADDRESS_FAMILY
    {
        AppleTalk = 16,
        Atm = 22,
        Banyan = 21,
        Ccitt = 10,
        Chaos = 5,
        Cluster = 24,
        ControllerAreaNetwork = 65537,
        DataKits = 9,
        DataLink = 13,
        DecNet = 12,
        Ecma = 8,
        FireFox = 19,
        HyperChannel = 15,
        Ieee12844 = 25,
        ImpLink = 3,
        InterNetwork = 2,
        InterNetworkV6 = 23,
        Ipx = 6,
        Irda = 26,
        Iso = 7,
        Lat = 14,
        Max = 29,
        NetBios = 17,
        NetworkDesigners = 28,
        NS = 6,
        Osi = 7,
        Packet = 65536,
        Pup = 4,
        Sna = 11,
        Unix = 1,
        Unknown = -1,
        Unspecified = 0,
        VoiceView = 18
    }
}