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
using ApplicationLogging;
using ApplicationMetrics;

namespace MathematicsModularFramework
{
    /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="T:MathematicsModularFramework.IModule"]/*'/>
    public interface IModule
    {
        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Description"]/*'/>
        String Description
        {
            get;
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Inputs"]/*'/>
        IEnumerable<InputSlot> Inputs
        {
            get;
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Outputs"]/*'/>
        IEnumerable<OutputSlot> Outputs
        {
            get;
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Processed"]/*'/>
        Boolean Processed
        {
            get;
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.Logger"]/*'/>
        IApplicationLogger Logger
        {
            set;
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="P:MathematicsModularFramework.IModule.MetricLogger"]/*'/>
        IMetricLogger MetricLogger
        {
            set;
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModule.GetInputSlot(System.String)"]/*'/>
        InputSlot GetInputSlot(String name);

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModule.GetOutputSlot(System.String)"]/*'/>
        OutputSlot GetOutputSlot(String name);

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModule.Process"]/*'/>
        void Process();
    }
}
