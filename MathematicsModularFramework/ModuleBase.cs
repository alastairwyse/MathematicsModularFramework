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
using ApplicationLogging;
using ApplicationMetrics;

namespace MathematicsModularFramework
{
    /// <summary>
    /// Base class for classes implementing interface IModule, containing common functionality.
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Description"]/*'/>
        protected String description;
        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Inputs"]/*'/>
        protected List<InputSlot> inputs;
        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Outputs"]/*'/>
        protected List<OutputSlot> outputs;
        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Processed"]/*'/>
        protected Boolean processed;
        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Logger"]/*'/>
        protected IApplicationLogger logger;
        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.MetricLogger"]/*'/>
        protected IMetricLogger metricLogger;
        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.CancellationToken"]/*'/>
        protected CancellationToken cancellationToken;

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Description"]/*'/>
        /// <exception cref="System.ArgumentException">If the inputted description is null or 0 length.</exception>
        public virtual String Description
        {
            get 
            {
                return description;
            }

            protected set
            {
                if (value == null || value.Length == 0)
                {
                    throw new ArgumentException("Parameter 'Description' must be non-null and greater than 0 length.", "Description");
                }
                description = value;
            }
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Inputs"]/*'/>
        public virtual IEnumerable<InputSlot> Inputs
        {
            get
            {
                return inputs;
            }
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Outputs"]/*'/>
        public virtual IEnumerable<OutputSlot> Outputs
        {
            get
            {
                return outputs;
            }
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Processed"]/*'/>
        public virtual bool Processed
        {
            get
            {
                return processed;
            }
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Logger"]/*'/>
        public IApplicationLogger Logger
        {
            set
            {
                logger = value;
            }
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.MetricLogger"]/*'/>
        public IMetricLogger MetricLogger
        {
            set
            {
                metricLogger = value;
            }
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.CancellationToken"]/*'/>
        public CancellationToken CancellationToken
        {
            set
            {
                cancellationToken = value;
            }
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModule.GetInputSlot(System.String)"]/*'/>
        /// <exception cref="System.ArgumentException">If an input slot with the specified name does not exist.</exception>
        public InputSlot GetInputSlot(String name)
        {
            foreach(InputSlot currentInputSlot in inputs)
            {
                if (currentInputSlot.Name == name)
                {
                    return currentInputSlot;
                }
            }
            
            throw new ArgumentException("An input slot with name '" + name + "' does not exist on module '" + this.GetType().FullName + "'.", "name");
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModule.GetOutputSlot(System.String)"]/*'/>
        /// <exception cref="System.ArgumentException">If an output slot with the specified name does not exist.</exception>
        public OutputSlot GetOutputSlot(String name)
        {
            foreach (OutputSlot currentOutputSlot in outputs)
            {
                if (currentOutputSlot.Name == name)
                {
                    return currentOutputSlot;
                }
            }

            throw new ArgumentException("An output slot with name '" + name + "' does not exist on module '" + this.GetType().FullName + "'.", "name");
        }

        /// <summary>
        /// Gets whether cancellation has been requested on this module (from the underlying CancellationToken field).
        /// </summary>
        protected Boolean IsCancellationRequested
        {
            get
            {
                return cancellationToken.IsCancellationRequested;
            }
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.ModuleBase class.
        /// </summary>
        protected ModuleBase()
        {
            inputs = new List<InputSlot>();
            outputs = new List<OutputSlot>();
            processed = false;
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModule.Process"]/*'/>
        public void Process()
        {
            PreProcess();
            ImplementProcess();
            PostProcess();
        }

        /// <summary>
        /// The actual implementation of the public Process() method.  Deriving classes must override and implement their actual processing logic in this method.
        /// </summary>
        protected virtual void ImplementProcess()
        {
        }

        /// <summary>
        /// Performs preprocessing steps common to all modules.
        /// </summary>
        protected void PreProcess()
        {
            // Check that this module has not already been processed
            if (processed == true)
            {
                throw new ModuleAlreadyProcessedException("Module '" + this.GetType().FullName + "' has already been processed.", this);
            }

            // Check the data types specified on all the input slots match the actual data contained in them.
            foreach (InputSlot currentInputSlot in inputs)
            {
                if ((currentInputSlot.DataValue != null) && (currentInputSlot.DataType.IsAssignableFrom(currentInputSlot.DataValue.GetType()) == false))
                {
                    throw new Exception("Input slot '" + currentInputSlot.Name + "' on module '" + this.GetType().FullName + "' specifies data type '" + currentInputSlot.DataType.FullName + "', but contains data of type '" + currentInputSlot.DataValue.GetType().FullName + "'.");
                }
            }
        }

        /// <summary>
        /// Performs postprocessing steps common to all modules.
        /// </summary>
        protected void PostProcess()
        {
            processed = true;
        }

        /// <summary>
        /// Adds an input slot to the module.
        /// </summary>
        /// <param name="name">The name of the data held by the slot.</param>
        /// <param name="description">A description of the data held by the slot.</param>
        /// <param name="dataType">The type of the data held by the slot.</param>
        protected void AddInputSlot(String name, String description, Type dataType)
        {
            // Check that an input slot with the same name doesn't already exist
            foreach(InputSlot currentInputSlot in inputs)
            {
                if(currentInputSlot.Name == name)
                {
                    throw new ArgumentException("Failed to add input slot to module '" + this.GetType().FullName + "'.  The module already contains an input slot with name '" + currentInputSlot.Name + "'.", "name");
                }
            }

            inputs.Add(new InputSlot(name, description, dataType, this));
        }

        /// <summary>
        /// Adds an output slot to the module.
        /// </summary>
        /// <param name="name">The name of the data held by the slot.</param>
        /// <param name="description">A description of the data held by the slot.</param>
        /// <param name="dataType">The type of the data held by the slot.</param>
        protected void AddOutputSlot(String name, String description, Type dataType)
        {
            // Check that an output slot with the same name doesn't already exist
            foreach (OutputSlot currentOutputSlot in outputs)
            {
                if (currentOutputSlot.Name == name)
                {
                    throw new ArgumentException("Failed to add output slot to module '" + this.GetType().FullName + "'.  The module already contains an output slot with name '" + currentOutputSlot.Name + "'.", "name");
                }
            }

            outputs.Add(new OutputSlot(name, description, dataType, this));
        }

        /// <summary>
        /// Throws an OperationCanceledException if this module has had cancellation requested.
        /// </summary>
        /// <exception cref="System.OperationCanceledException">If cancellation has been requested.</exception>
        protected void ThrowIfCancellationRequested()
        {
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
