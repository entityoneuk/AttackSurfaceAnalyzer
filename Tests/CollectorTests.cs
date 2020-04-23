// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using AttackSurfaceAnalyzer.Collectors;
using AttackSurfaceAnalyzer.Objects;
using AttackSurfaceAnalyzer.Types;
using AttackSurfaceAnalyzer.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsFirewallHelper;

namespace AttackSurfaceAnalyzer.Tests
{
    [TestClass]
    public class CollectorTests
    {
        [TestInitialize]
        public void Setup()
        {
            Logger.Setup(false, true);
            Strings.Setup();
            AsaTelemetry.Setup(test: true);
        }

        [TestCleanup]
        public void TearDown()
        {
        }

        [TestMethod]
        public void TestFileCollector()
        {
            var testFolder = AsaHelpers.GetTempFolder();
            Directory.CreateDirectory(testFolder);

            var opts = new CollectCommandOptions()
            {
                EnableFileSystemCollector = true,
                GatherHashes = true,
                SelectedDirectories = testFolder,
                DownloadCloud = false,
                CertificatesFromFiles = false
            };

            using (var file = File.Open(Path.Combine(testFolder, "AsaLibTesterMZ"), FileMode.OpenOrCreate))
            {
                file.Write(FileSystemUtils.WindowsMagicNumber, 0, 2);
                file.Write(FileSystemUtils.WindowsMagicNumber, 0, 2);

                file.Close();
            }

            using (var file = File.Open(Path.Combine(testFolder, "AsaLibTesterJavaClass"), FileMode.OpenOrCreate))
            {
                file.Write(FileSystemUtils.JavaMagicNumber, 0, 4);
                file.Close();
            }

            var fsc = new FileSystemCollector(opts);
            fsc.Execute();

            Assert.IsTrue(fsc.Results.Any(x => x is FileSystemObject FSO && FSO.Path.EndsWith("AsaLibTesterJavaClass") && FSO.IsExecutable == true));
            Assert.IsTrue(fsc.Results.Any(x => x is FileSystemObject FSO && FSO.Path.EndsWith("AsaLibTesterMZ") && FSO.IsExecutable == true));
        }

        /// <summary>
        /// Requires Admin
        /// </summary>
        [TestMethod]
        public void TestEventCollectorWindows()
        {
            var source = "AsaTests";
            var logname = "AsaTestLogs";

            if (EventLog.SourceExists(source))
            {
                // Delete the source and the log.
                EventLog.DeleteEventSource(source);
                EventLog.Delete(logname);
            }

            // Create the event source to make next try successful.
            EventLog.CreateEventSource(source, logname);

            using EventLog eventLog = new EventLog("Application");
            eventLog.Source = "Attack Surface Analyzer Tests";
            eventLog.WriteEntry("This Log Entry was created for testing the Attack Surface Analyzer library.", EventLogEntryType.Warning, 101, 1);

            var fsc = new EventLogCollector();
            fsc.Execute();

            EventLog.DeleteEventSource(source);
            EventLog.Delete(logname);

            Assert.IsTrue(fsc.Results.Any(x => x is EventLogObject ELO && ELO.Source == "Attack Surface Analyzer Tests" && ELO.Timestamp is DateTime DT && DT.AddMinutes(1).CompareTo(DateTime.Now) > 0));
        }

        /// <summary>
        /// Requires Admin
        /// </summary>
        [TestMethod]
        public void TestTpmCollector()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //var tpmSim = new TpmSim();
                //tpmSim.StartSimulator();

                using var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "TpmSim\\Simulator.exe",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };

                process.Start();

                // Let the simulator get started.
                Thread.Sleep(10);

                var tpmc = new TpmCollector(TestMode: true);

                // Write to NV
                // Persist a key
                // Measure to a PCR

                tpmc.Execute();

                process.Kill();
            }
        }

        /// <summary>
        /// Requires Admin
        /// </summary>
        [TestMethod]
        public void TestTpmCollectorInProc()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var simulator = new TpmSim();

                simulator.StartSimulator();

                // Let the simulator get started.
                Thread.Sleep(10);

                var tpmc = new TpmCollector(TestMode: true);

                // Write to NV
                // Persist a key
                // Measure to a PCR

                tpmc.Execute();

                simulator.StopSimulator();
            }
        }

        /// <summary>
        /// Does not require admin.
        /// </summary>
        [TestMethod]
        public void TestCertificateCollectorWindows()
        {
            var fsc = new CertificateCollector();
            fsc.Execute();

            Assert.IsTrue(fsc.Results.Where(x => x.ResultType == RESULT_TYPE.CERTIFICATE).Count() > 0);
        }

        /// <summary>
        /// Does not require Admin.
        /// </summary>
        [TestMethod]
        public void TestPortCollectorWindows()
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                int port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to open port.");
            }
            var fsc = new OpenPortCollector();
            fsc.Execute();
            server.Stop();

            Assert.IsTrue(fsc.Results.Any(x => x is OpenPortObject OPO && OPO.Port == 13000));
        }

        /// <summary>
        /// Requires root.
        /// </summary>
        [TestMethod]
        public void TestFirewallCollectorOSX()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _ = ExternalCommandRunner.RunExternalCommand("/usr/libexec/ApplicationFirewall/socketfilterfw", "--add /bin/bash");

                var fwc = new FirewallCollector();
                fwc.Execute();
                _ = ExternalCommandRunner.RunExternalCommand("/usr/libexec/ApplicationFirewall/socketfilterfw", "--remove /bin/bash");
                Assert.IsTrue(fwc.Results.Any(x => x is FirewallObject FWO && FWO.ApplicationName == "/bin/bash"));
            }
        }

        /// <summary>
        /// Requires root.
        /// </summary>
        [TestMethod]
        public void TestFirewallCollectorLinux()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var result = ExternalCommandRunner.RunExternalCommand("iptables", "-A INPUT -p tcp --dport 19999 -j DROP");

                var fwc = new FirewallCollector();
                fwc.Execute();

                Assert.IsTrue(fwc.Results.Any(x => x is FirewallObject FWO && FWO.LocalPorts.Contains("19999")));

                result = ExternalCommandRunner.RunExternalCommand("iptables", "-D INPUT -p tcp --dport 19999 -j DROP");
            }
        }


        [TestMethod]
        public void TestFirewallCollectorWindows()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.IsTrue(AsaHelpers.IsAdmin());

                var rule = FirewallManager.Instance.CreatePortRule(
                    @"TestFirewallPortRule",
                    FirewallAction.Allow,
                    9999,
                    FirewallProtocol.TCP
                );
                FirewallManager.Instance.Rules.Add(rule);

                rule = FirewallManager.Instance.CreateApplicationRule(
                    @"TestFirewallAppRule",
                    FirewallAction.Allow,
                    @"C:\MyApp.exe"
                );
                rule.Direction = FirewallDirection.Outbound;
                FirewallManager.Instance.Rules.Add(rule);

                var fwc = new FirewallCollector();
                fwc.Execute();

                Assert.IsTrue(fwc.Results.Any(x => x is FirewallObject FWO && FWO.LocalPorts.Contains("9999")));
                Assert.IsTrue(fwc.Results.Any(x => x is FirewallObject FWO && FWO.ApplicationName is string && FWO.ApplicationName.Equals(@"C:\MyApp.exe")));


                var rules = FirewallManager.Instance.Rules.Where(r => r.Name == "TestFirewallPortRule");
                foreach (var ruleIn in rules)
                {
                    FirewallManager.Instance.Rules.Remove(ruleIn);
                }

                rules = FirewallManager.Instance.Rules.Where(r => r.Name == "TestFirewallAppRule");
                foreach (var ruleIn in rules)
                {
                    FirewallManager.Instance.Rules.Remove(ruleIn);
                }
            }
        }

        /// <summary>
        /// Does not require administrator.
        /// </summary>
        [TestMethod]
        public void TestRegistryCollectorWindows()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Create a registry key
                var name = Guid.NewGuid().ToString();
                var value = Guid.NewGuid().ToString();
                var value2 = Guid.NewGuid().ToString();

                RegistryKey key;
                key = Registry.CurrentUser.CreateSubKey(name);
                key.SetValue(value, value2);
                key.Close();

                var rc = new RegistryCollector(new List<RegistryHive>() { RegistryHive.CurrentUser }, true);
                rc.Execute();

                Registry.CurrentUser.DeleteSubKey(name);

                Assert.IsTrue(rc.Results.Any(x => x is RegistryObject RO && RO.Key.EndsWith(name)));
                Assert.IsTrue(rc.Results.Any(x => x is RegistryObject RO && RO.Key.EndsWith(name) && RO.Values.ContainsKey(value) && RO.Values[value] == value2));
            }
        }

        /// <summary>
        /// Requires Administrator Priviledges.
        /// </summary>
        [TestMethod]
        public void TestServiceCollectorWindows()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Create a service - This won't throw an exception, but it won't work if you are not an Admin.
                var serviceName = "AsaDemoService";
                var exeName = "AsaDemoService.exe";
                var cmd = string.Format("create {0} binPath=\"{1}\"", serviceName, exeName);
                ExternalCommandRunner.RunExternalCommand("sc.exe", cmd);

                var sc = new ServiceCollector();
                sc.Execute();

                Assert.IsTrue(sc.Results.Any(x => x is ServiceObject RO && RO.Name.Equals(serviceName)));

                // Clean up
                cmd = string.Format("delete {0}", serviceName);
                ExternalCommandRunner.RunExternalCommand("sc.exe", cmd);
            }
        }

        /// <summary>
        /// Requires admin.
        /// </summary>
        [TestMethod]
        public void TestComObjectCollector()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var coc = new ComObjectCollector();
                coc.Execute();

                Assert.IsTrue(coc.Results.Any(x => x is ComObject y && y.x86_Binary != null));
            }
        }

        /// <summary>
        /// Requires Administrator Priviledges.
        /// </summary>
        [TestMethod]
        public void TestUserCollectorWindows()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.IsTrue(AsaHelpers.IsAdmin());
                var user = System.Guid.NewGuid().ToString().Substring(0, 10);
                var password = "$" + CryptoHelpers.GetRandomString(13);

                var cmd = string.Format("user /add {0} {1}", user, password);
                ExternalCommandRunner.RunExternalCommand("net", cmd);

                var uac = new UserAccountCollector();
                uac.Execute();

                Assert.IsTrue(uac.Results.Any(x => x is UserAccountObject y && y.Name.Equals(user)));

                cmd = string.Format("user /delete {0}", user);
                ExternalCommandRunner.RunExternalCommand("net", cmd);
            }
        }
    }
}
