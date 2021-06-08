﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using OneIdentity.DevOps.Common;

namespace OneIdentity.DevOps.Logic
{
    internal static class WellKnownData
    {
        public const string AppSettings = "appsettings";
        public const string ServiceIdentifier = "svcid";
        public const string CredentialTarget = "SSBfDbgn";
        public const string CredentialEnvVar = "SSBEncPasswd";

        public const string PluginInfoClassName = "PluginDescriptor";

        public const string DevOpsServiceName = "SafeguardDevOpsService";

        private const string DevOpsServiceUserName = "SafeguardDevOpsUser";
        private const string DevOpsA2ARegistrationName = DevOpsServiceName;
        private const string DevOpsVaultA2ARegistrationName = DevOpsA2ARegistrationName + "VaultCredentials";

        private const string DevOpsServiceClientCertificateSubject = "CN=DevOpsServiceClientCertificate";
        private const string DevOpsServiceWebSslCertificateSubject = "CN=DevOpsServiceWebSslCertificate";

        public const string ManifestPattern = "Manifest.json";
        public const string DllPattern = "*.dll";

        public const string PluginDirName = "ExternalPlugins";
        public const string PluginStageName = "PluginStaging";
        public const string AddonServiceStageName = "AddonServiceStaging";

        public const string MonitorEnabled = "Enabled";
        public const string MonitorDisabled = "Disabled";

        public const string SentPasswordSuccess = "Success";
        public const string SentPasswordFailure = "Failure";

        public const string PluginsDeleteFile = "DeletePlugins.all";
        public const string AddonDeleteFile = "DeleteAddon.all";


        public static readonly string ProgramDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), DevOpsServiceName);
        public static readonly string ServiceDirPath = Path.GetDirectoryName(
            System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) ?
                Assembly.GetExecutingAssembly().Location : Process.GetCurrentProcess().MainModule?.FileName);
        public static readonly string PluginDirPath = Path.Combine(ProgramDataPath, PluginDirName);
        public static readonly string PluginStageDirPath = Path.Combine(ProgramDataPath, PluginStageName);
        public static readonly string AddonServiceStageDirPath = Path.Combine(ProgramDataPath, AddonServiceStageName);
        public static readonly string SvcIdPath = Path.Combine(ServiceDirPath, ServiceIdentifier);
        public static readonly string DeleteAllPlugins = Path.Combine(PluginDirPath, PluginsDeleteFile);
        public static readonly string LogDirPath = Path.Combine(ProgramDataPath, $"{DevOpsServiceName}.log");

        public static readonly string RestartNotice =
            "Safeguard Secrets Broker for DevOps needs to be restarted to complete installing this action.";



        public static string GetSppToken(HttpContext context)
        {
            var authHeader = context.Request.Headers.FirstOrDefault(c => c.Key == "Authorization");
            var sppToken = authHeader.Value.ToString();
            if (!sppToken.StartsWith("spp-token ", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return sppToken.Split(" ").LastOrDefault();
        }

        public static string DevOpsUserName(string svcId)
        {
            return $"{DevOpsServiceUserName}-{svcId}";
        }

        public static string DevOpsRegistrationName(string svcId)
        {
            return $"{DevOpsA2ARegistrationName}-{svcId}";
        }

        public static string DevOpsVaultRegistrationName(string svcId)
        {
            return $"{DevOpsVaultA2ARegistrationName}-{svcId}";
        }

        public static string DevOpsServiceClientCertificate(string svcId)
        {
            return $"{DevOpsServiceClientCertificateSubject}-{svcId}";
        }

        public static string DevOpsServiceWebSslCertificate(string svcId)
        {
            return $"{DevOpsServiceWebSslCertificateSubject}-{svcId}";
        }

        public static string DevOpsServiceVersion()
        {
            var assembly = Assembly.GetAssembly(typeof(ILoadablePlugin));
            if (assembly != null)
            {
                var version = assembly.GetName().Version;
                var buildType =
                    assembly.GetCustomAttributes(false).OfType<DebuggableAttribute>().Any(da => da.IsJITTrackingEnabled)
                        ? "Debug"
                        : "Release";
                return $"{buildType}-{version}";
            }

            return "UnknownConfig-UnknownVersion";
        }

    }
}
