﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.ML.Runtime;

namespace Microsoft.ML.AutoML
{
    internal interface IMonitor
    {
        void ReportCompletedTrial(TrialResult result);

        void ReportBestTrial(TrialResult result);

        void ReportFailTrial(TrialResult result);

        void ReportRunningTrial(TrialSettings setting);
    }

    // this monitor redirects output result to context.log
    internal class MLContextMonitor : IMonitor
    {
        private readonly MLContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly IChannel _logger;
        private readonly List<TrialResult> _completedTrials;

        public MLContextMonitor(MLContext context, IServiceProvider provider)
        {
            _context = context;
            _serviceProvider = provider;
            _logger = ((IChannelProvider)context).Start(nameof(AutoMLExperiment));
            _completedTrials = new List<TrialResult>();
        }

        public void ReportBestTrial(TrialResult result)
        {
            _logger.Info($"Update Best Trial - Id: {result.TrialSettings.TrialId} - Metric: {result.Metric} - Pipeline: {result.TrialSettings.Pipeline}");
        }

        public void ReportCompletedTrial(TrialResult result)
        {
            _logger.Info($"Update Completed Trial - Id: {result.TrialSettings.TrialId} - Metric: {result.Metric} - Pipeline: {result.TrialSettings.Pipeline} - Duration: {result.DurationInMilliseconds}");
            _completedTrials.Add(result);
        }

        public void ReportFailTrial(TrialResult result)
        {
            _logger.Info($"Update Failed Trial - Id: {result.TrialSettings.TrialId} - Metric: {result.Metric} - Pipeline: {result.TrialSettings.Pipeline}");
        }

        public void ReportRunningTrial(TrialSettings setting)
        {
            _logger.Info($"Update Running Trial - Id: {setting.TrialId} - Pipeline: {setting.Pipeline}");
        }
    }
}
