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

namespace MathematicsModularFramework
{
    /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="T:MathematicsModularFramework.IModuleGraphProcessor"]/*'/>
    public interface IModuleGraphProcessor
    {
        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModuleGraphProcessor.Process(MathematicsModularFramework.ModuleGraph,System.Boolean)"]/*'/>
        void Process(ModuleGraph moduleGraph, Boolean allowEmptyModuleInputSlots);

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModuleGraphProcessor.CancelProcessing"]/*'/>
        void CancelProcessing();

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModuleGraphProcessor.Copy(MathematicsModularFramework.ModuleGraph)"]/*'/>
        ModuleGraph Copy(ModuleGraph moduleGraph);

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.IModuleGraphProcessor.Validate(MathematicsModularFramework.ModuleGraph)"]/*'/>
        List<ValidationError> Validate(ModuleGraph moduleGraph);
    }
}
