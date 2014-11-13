using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace NumberRecoginzer.Cloud.Service
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // Heben Sie die Auskommentierung des entsprechenden Abschnitts in der web.config-Datei auf, um AzureLocalStorageTraceListener zu aktivieren.  
            DiagnosticMonitorConfiguration diagnosticConfig = DiagnosticMonitor.GetDefaultInitialConfiguration();
            diagnosticConfig.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
            diagnosticConfig.Directories.DataSources.Add(AzureLocalStorageTraceListener.GetLogDirectory());

            // Informationen zum Behandeln von Konfigurationsänderungen
            // finden Sie im MSDN-Thema unter http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
