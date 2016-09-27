/*
 * Copyright 2016 Alastair Wyse (http://www.oraclepermissiongenerator.net/mathematicsmodularframework/)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ApplicationMetrics;

namespace MathematicsModularFramework
{
    /// <summary>
    /// Contains common methods used log application metric events.
    /// </summary>
    class MetricsUtilities
    {
        private IMetricLogger metricLogger;

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.MetricsUtilities class.
        /// </summary>
        /// <param name="metricLogger">The metric logger to write metric events to.</param>
        public MetricsUtilities(IMetricLogger metricLogger)
        {
            this.metricLogger = metricLogger;
        }

        /// <summary>
        /// Records a single instance of the specified count event.
        /// </summary>
        /// <param name="countMetric">The count metric that occurred.</param>
        [Conditional("METRICS_ON")]
        public void Increment(CountMetric countMetric)
        {
            metricLogger.Increment(countMetric);
        }

        /// <summary>
        /// Records an instance of the specified amount metric event, and the associated amount.
        /// </summary>
        /// <param name="amountMetric">The amount metric that occurred.</param>
        [Conditional("METRICS_ON")]
        public void Add(AmountMetric amountMetric)
        {
            metricLogger.Add(amountMetric);
        }

        /// <summary>
        /// Records an instance of the specified status metric event, and the associated value.
        /// </summary>
        /// <param name="statusMetric">The status metric that occurred.</param>
        [Conditional("METRICS_ON")]
        public void Set(StatusMetric statusMetric)
        {
            metricLogger.Set(statusMetric);
        }

        /// <summary>
        /// Records the starting of the specified interval event.
        /// </summary>
        /// <param name="intervalMetric">The interval metric that started.</param>
        [Conditional("METRICS_ON")]
        public void Begin(IntervalMetric intervalMetric)
        {
            metricLogger.Begin(intervalMetric);
        }

        /// <summary>
        /// Records the completion of the specified interval event.
        /// </summary>
        /// <param name="intervalMetric">The interval metric that completed.</param>
        [Conditional("METRICS_ON")]
        public void End(IntervalMetric intervalMetric)
        {
            metricLogger.End(intervalMetric);
        }

        /// <summary>
        /// Cancels the starting of the specified interval event (e.g. in the case that an exeception occurs between the starting and completion of the interval event).
        /// </summary>
        /// <param name="intervalMetric">The interval metric that should be cancelled.</param>
        [Conditional("METRICS_ON")]
        public void CancelBegin(IntervalMetric intervalMetric)
        {
            metricLogger.CancelBegin(intervalMetric);
        }
    }
}
