/*
 * Copyright 2017 Alastair Wyse (http://www.oraclepermissiongenerator.net/simpleml/)
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
using ApplicationLogging;
using ApplicationMetrics;
using MathematicsModularFramework.Metrics;

namespace MathematicsModularFramework
{
    /// <summary>
    /// Provides methods associated with executing the computational flow defined by a module graph.
    /// </summary>
    public class ModuleGraphProcessor : IModuleGraphProcessor, IDisposable
    {
        /// <summary>Maintains a list of mappings between a module in a source graph, and its equivalent module in a copy of that graph.</summary>
        private Dictionary<IModule, IModule> copiedGraphModuleMap;
        /// <summary>Logger class passed to modules prior to processing.</summary>
        private IApplicationLogger logger;
        /// <summary>Metric logger class passed to modules prior to processing.</summary>
        private IMetricLogger metricLogger;
        /// <summary>Utility class used to write log events.</summary>
        private LoggingUtilities loggingUtilities;
        /// <summary>Utility class used to write metric events.</summary>
        private MetricsUtilities metricsUtilities;
        /// <summary>Passes CancellationTokens to modules being processed, and allows for cancelling of processing.</summary>
        private CancellationTokenSource cancellationTokenSource;
        /// <summary>Indicates whether the object has been disposed.</summary>
        protected bool disposed;
        
        /// <summary>
        /// Indicates whether the ModuleGraphProcessor has been disposed.
        /// </summary>
        public Boolean IsDisposed
        {
            get
            {
                return disposed;
            }
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.ModuleGraphProcessor class.
        /// </summary>
        public ModuleGraphProcessor()
        {
            copiedGraphModuleMap = new Dictionary<IModule, IModule>();
            logger = new NullApplicationLogger();
            metricLogger = new NullMetricLogger();
            loggingUtilities = new LoggingUtilities(logger);
            metricsUtilities = new MetricsUtilities(metricLogger);
            cancellationTokenSource = new CancellationTokenSource();
            disposed = false;
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.ModuleGraphProcessor class.
        /// </summary>
        /// <param name="logger">The logger to write log events to.</param>
        public ModuleGraphProcessor(IApplicationLogger logger)
            : this()
        {
            this.logger = logger;
            loggingUtilities = new LoggingUtilities(logger);
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.ModuleGraphProcessor class.
        /// </summary>
        /// <param name="metricLogger">>The metric logger to write metric and instrumentation events to.</param>
        public ModuleGraphProcessor(IMetricLogger metricLogger)
            : this()
        {
            this.metricLogger = metricLogger;
            metricsUtilities = new MetricsUtilities(metricLogger);
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.ModuleGraphProcessor class.
        /// </summary>
        /// <param name="logger">The logger to write log events to.</param>
        /// <param name="metricLogger">The metric logger to write metric and instrumentation events to.</param>
        public ModuleGraphProcessor(IApplicationLogger logger, IMetricLogger metricLogger)
            : this()
        {
            this.logger = logger;
            this.metricLogger = metricLogger;
            loggingUtilities = new LoggingUtilities(logger);
            metricsUtilities = new MetricsUtilities(metricLogger);
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModuleGraphProcessor.Process(MathematicsModularFramework.ModuleGraph,System.Boolean)"]/*'/>
        public void Process(ModuleGraph moduleGraph, Boolean allowEmptyModuleInputSlots)
        {
            ThrowExceptionIfDisposed();

            // Define all processing actions to pass to the ModuleGraphRecurser class
            Action<IModule> circularReferenceAction = (IModule module) => { throw new Exception("Graph contains a circular reference involving module '" + module.GetType().FullName + "'."); };
            Action<InputSlot> unlinkedInputSlotAction = (InputSlot inputSlot) => { };
            Action<InputSlot> dataUnassignedInputSlotAction = (InputSlot inputSlot) => 
            {
                String errorMessage = "Input slot '" + inputSlot.Name + "' on module '" + inputSlot.Module.GetType().FullName + "' does not have any data assigned to it.";
                loggingUtilities.Log(this, LogLevel.Warning, errorMessage);
                if (allowEmptyModuleInputSlots == false)
                {
                    throw new InputSlotDataUnassignedException(errorMessage, inputSlot);
                }
            };
            Action<IModule> recursionAction = (IModule module) => 
            {
                module.Logger = logger;
                module.MetricLogger = metricLogger;
                module.CancellationToken = cancellationTokenSource.Token;
                loggingUtilities.Log(this, LogLevel.Information, "Processing module '" + module.GetType().FullName + "'.");
                metricsUtilities.Begin(new ModuleProcessingTime());
                try
                {
                    module.Process();
                }
                catch (OperationCanceledException)
                {
                    metricsUtilities.Increment(new ModuleGraphProcessingCancelled());
                    loggingUtilities.Log(this, LogLevel.Information, "Processing of module '" + module.GetType().FullName + "' cancelled.");
                    metricsUtilities.CancelBegin(new ModuleProcessingTime());
                    throw;
                }
                catch (Exception)
                {
                    metricsUtilities.CancelBegin(new ModuleProcessingTime());
                    throw;
                }
                metricsUtilities.End(new ModuleProcessingTime());
                metricsUtilities.Increment(new ModuleProcessed());
            };
            Action<OutputSlot> outputSlotAction = (OutputSlot outputSlot) =>
            {
                if (moduleGraph.GetInputSlotsLinkedToOutputSlot(outputSlot).Count<InputSlot>() == 0)
                {
                    loggingUtilities.Log(this, LogLevel.Warning, "Output slot '" + outputSlot.Name + "' on module '" + outputSlot.Module.GetType().FullName + "' is not referenced by a slot link.");
                }
                else
                {
                    foreach (InputSlot currentInputSlot in moduleGraph.GetInputSlotsLinkedToOutputSlot(outputSlot))
                    {
                        currentInputSlot.DataValue = outputSlot.DataValue;
                        // Remove the slot link between the output and input slot, as it is not needed after the module is processed (also frees up the slot link and source module for garbage collection)
                        loggingUtilities.Log(this, LogLevel.Debug, "Removing slot link between output slot '" + outputSlot.Name + "' on module '" + outputSlot.Module.GetType().FullName + "' and input slot '" + currentInputSlot.Name + "' on module '" + currentInputSlot.Module.GetType().FullName + "' from the module graph.");
                        moduleGraph.RemoveSlotLink(outputSlot, currentInputSlot);
                    }
                }
            };
            Action<IModule> postRecursionAction = (IModule module) =>
            {
                loggingUtilities.Log(this, LogLevel.Debug, "Removing module '" + module.GetType().FullName + "' from the module graph.");
                moduleGraph.RemoveModule(module);
            };

            ModuleGraphRecurser processRecurser = new ModuleGraphRecurser(circularReferenceAction, unlinkedInputSlotAction, dataUnassignedInputSlotAction, recursionAction, outputSlotAction, postRecursionAction, logger);

            loggingUtilities.Log(this, LogLevel.Information, "Starting module graph processing.");
            metricsUtilities.Begin(new ModuleGraphProcessingTime());
            try
            {
                processRecurser.Recurse(moduleGraph, true);
            }
            catch (Exception)
            {
                metricsUtilities.CancelBegin(new ModuleGraphProcessingTime());
                throw;
            }
            metricsUtilities.End(new ModuleGraphProcessingTime());
            metricsUtilities.Increment(new ModuleGraphProcessed());
            loggingUtilities.Log(this, LogLevel.Information, "Module graph processing completed.");
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModuleGraphProcessor.CancelProcessing"]/*'/>
        public void CancelProcessing()
        {
            ThrowExceptionIfDisposed();
            cancellationTokenSource.Cancel();
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModuleGraphProcessor.Copy(MathematicsModularFramework.ModuleGraph)"]/*'/>
        public ModuleGraph Copy(ModuleGraph moduleGraph)
        {
            ThrowExceptionIfDisposed();

            ModuleGraph destinationModuleGraph = new ModuleGraph();

            metricsUtilities.Begin(new ModuleGraphCopyingTime());
            try
            {
                copiedGraphModuleMap.Clear();
                foreach (IModule currentModule in moduleGraph.EndPoints)
                {
                    CopyModule(currentModule, moduleGraph, destinationModuleGraph);
                }
            }
            catch (Exception)
            {
                metricsUtilities.CancelBegin(new ModuleGraphCopyingTime());
                throw;
            }
            metricsUtilities.End(new ModuleGraphCopyingTime());
            metricsUtilities.Increment(new ModuleGraphCopied());

            return destinationModuleGraph;
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModuleGraphProcessor.Validate(MathematicsModularFramework.ModuleGraph)"]/*'/>
        public List<ValidationError> Validate(ModuleGraph moduleGraph)
        {
            ThrowExceptionIfDisposed();

            List<ValidationError> validationErrors = new List<ValidationError>();

            if (moduleGraph.EndPoints.Count<IModule>() == 0)
            {
                validationErrors.Add(new EmptyGraphValidationError());
            }
            else
            {
                // Define all processing actions to pass to the ModuleGraphRecurser class
                Action<IModule> circularReferenceAction = (IModule module) => { validationErrors.Add(new CircularReferenceValidationError(module)); };
                Action<InputSlot> unlinkedInputSlotAction = (InputSlot inputSlot) => { validationErrors.Add(new UnlinkedInputSlotValidationError(inputSlot)); };
                Action<InputSlot> dataUnassignedInputSlotAction = (InputSlot inputSlot) => { };
                Action<IModule> recursionAction = (IModule module) => { };
                Action<OutputSlot> outputSlotAction = (OutputSlot outputSlot) =>
                {
                    if (moduleGraph.GetInputSlotsLinkedToOutputSlot(outputSlot).Count<InputSlot>() == 0)
                    {
                        validationErrors.Add(new UnlinkedOutputSlotValidationError(outputSlot));
                    }
                };
                Action<IModule> postRecursionAction = (IModule module) => { };

                ModuleGraphRecurser validateRecurser = new ModuleGraphRecurser(circularReferenceAction, unlinkedInputSlotAction, dataUnassignedInputSlotAction, recursionAction, outputSlotAction, postRecursionAction, logger);

                validateRecurser.Recurse(moduleGraph, false);
            }

            return validationErrors;
        }

        /// <summary>
        /// Recurses the specified source module graph starting at the specified module, making a copy of the graph in the destination module graph.
        /// </summary>
        /// <param name="sourceModule">The module in the source graph to copy to the destination graph.</param>
        /// <param name="sourceModuleGraph">The module graph to copy.</param>
        /// <param name="destinationModuleGraph">The module graph to copy to.</param>
        /// <returns>The parent of the module equivalent to the 'sourceModule' parameter in the destination graph.</returns>
        private IModule CopyModule(IModule sourceModule, ModuleGraph sourceModuleGraph, ModuleGraph destinationModuleGraph)
        {
            IModule destinationModule = null;
            // Check whether this module has already been recursed, and either copy it to the destination and add to the dictionary of recursed  modules, or return the equivalent module in the copied graph
            if (copiedGraphModuleMap.ContainsKey(sourceModule) == false)
            {
                destinationModule = (IModule)Activator.CreateInstance(sourceModule.GetType());
                copiedGraphModuleMap.Add(sourceModule, destinationModule);
                destinationModuleGraph.AddModule(destinationModule);
                loggingUtilities.Log(this, LogLevel.Debug, "Created a copy of module '" + sourceModule.GetType().FullName + "' in destination module graph.");

                // Recurse to each parent of the current module
                foreach (InputSlot currentInputSlot in sourceModule.Inputs)
                {
                    if (sourceModuleGraph.OutputSlotIsLinkedToInputSlot(currentInputSlot) == true)
                    {
                        OutputSlot sourceParentModuleOutputSlot = sourceModuleGraph.GetOutputSlotLinkedToInputSlot(currentInputSlot);

                        IModule destinationParentModule = CopyModule(sourceParentModuleOutputSlot.Module, sourceModuleGraph, destinationModuleGraph);

                        // Create a slot link between the parent and the current module
                        if (destinationParentModule != null)
                        {
                            OutputSlot destinationParentModuleOutputSlot = destinationParentModule.GetOutputSlot(sourceParentModuleOutputSlot.Name);
                            InputSlot destinationModuleInputSlot = destinationModule.GetInputSlot(currentInputSlot.Name);
                            destinationModuleGraph.CreateSlotLink(destinationParentModuleOutputSlot, destinationModuleInputSlot);
                            loggingUtilities.Log(this, LogLevel.Debug, "Created a slot link between output slot '" + destinationParentModuleOutputSlot.Name + "' on module '" + destinationParentModuleOutputSlot.Module.GetType().FullName + "' and input slot '" + destinationModuleInputSlot.Name + "' on module '" + destinationModuleInputSlot.Module.GetType().FullName + "' in destination module graph.");
                        }
                    }
                }
            }
            else
            {
                destinationModule = copiedGraphModuleMap[sourceModule];
                loggingUtilities.Log(this, LogLevel.Debug, "Module '" + sourceModule.GetType().FullName + "' has already been copied to the destination module graph.");
            }

            return destinationModule;
        }

        /// <summary>
        /// Throws an ObjectDisposedException if Dispose() has been called on the object.
        /// </summary>
        private void ThrowExceptionIfDisposed()
        {
            if (disposed == true)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        #region Finalize / Dispose Methods

        /// <summary>
        /// Releases the unmanaged resources used by the ModuleGraphProcessor.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #pragma warning disable 1591
        ~ModuleGraphProcessor()
        {
            Dispose(false);
        }
        #pragma warning restore 1591

        /// <summary>
        /// Provides a method to free unmanaged resources used by this class.
        /// </summary>
        /// <param name="disposing">Whether the method is being called as part of an explicit Dispose routine, and hence whether managed resources should also be freed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                    if (cancellationTokenSource != null)
                    {
                        cancellationTokenSource.Dispose();
                    }
                }
                // Free your own state (unmanaged objects).

                // Set large fields to null.

                disposed = true;
            }
        }

        #endregion
    }
}
