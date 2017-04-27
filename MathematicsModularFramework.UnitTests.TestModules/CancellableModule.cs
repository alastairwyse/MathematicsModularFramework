/*
 * Copyright 2017 Alastair Wyse (http://www.oraclepermissiongenerator.net/mathematicsmodularframework/)
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
using System.Threading;

namespace MathematicsModularFramework.UnitTests.TestModules
{
    /// <summary>
    /// A test module which sets a boolean value on its output slot to true if it is cancelled, and optionally throws an OperationCanceledException.  Also accepts an AutoResetEvent on its input slot for signalling with a thread which does the cancelling.
    /// </summary>
    public class CancellableModule : ModuleBase
    {
        public CancellableModule()
            : base()
        {
            Description = "A test module which sets a boolean value on its output slot to true if it is cancelled, and optionally throws an OperationCanceledException.  Also accepts an AutoResetEvent on its input slot for signalling with a thread which does the cancelling.";
            AddInputSlot("CancellationThreadSignal", "An AutoResetEvent for signalling with a thread which does the cancelling", typeof(AutoResetEvent));
            AddInputSlot("ThrowExceptionIfCancelledSwitch", "If true, will call the ThrowIfCancellationRequested() method on the underlying CancellationToken", typeof(Boolean));
            AddOutputSlot("CancelWasCalled", "Set to true if the ThrowIfCancellationRequested() method was called on the underlying CancellationToken", typeof(Boolean));
        }

        protected override void ImplementProcess()
        {
            AutoResetEvent cancellationThreadSignal = (AutoResetEvent)GetInputSlot("CancellationThreadSignal").DataValue;
            Boolean throwExceptionIfCancelledSwitch = (Boolean)GetInputSlot("ThrowExceptionIfCancelledSwitch").DataValue;
            Boolean cancelWasCalled = false;

            // Tell the cancellation thread it can now call Cancel()
            cancellationThreadSignal.Set();
            // Wait for the cancellation thread to cancel
            cancellationThreadSignal.WaitOne();
            if (IsCancellationRequested == true)
            {
                cancelWasCalled = true;
            }
            GetOutputSlot("CancelWasCalled").DataValue = cancelWasCalled;
            if (throwExceptionIfCancelledSwitch == true)
            {
                ThrowIfCancellationRequested();
            }
        }
    }
}
