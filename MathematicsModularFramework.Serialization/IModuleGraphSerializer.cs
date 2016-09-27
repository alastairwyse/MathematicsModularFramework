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
using System.IO;

namespace MathematicsModularFramework.Serialization
{
    /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="T:MathematicsModularFramework.Serialization.IModuleGraphSerializer"]/*'/>
    public interface IModuleGraphSerializer
    {
        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IModuleGraphSerializer.Serialize(MathematicsModularFramework.ModuleGraph)"]/*'/>
        String Serialize(ModuleGraph moduleGraph);

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IModuleGraphSerializer.Serialize(MathematicsModularFramework.ModuleGraph,System.IO.StreamWriter)"]/*'/>
        void Serialize(ModuleGraph moduleGraph, StreamWriter streamWriter);

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IModuleGraphSerializer.Deserialize(System.String)"]/*'/>
        ModuleGraph Deserialize(String serializedModuleGraph);

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IModuleGraphSerializer.Deserialize(System.IO.StreamReader)"]/*'/>
        ModuleGraph Deserialize(StreamReader streamReader);
    }
}
