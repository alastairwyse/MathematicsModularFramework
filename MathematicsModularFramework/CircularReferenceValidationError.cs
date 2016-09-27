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

namespace MathematicsModularFramework
{
    /// <summary>
    /// Module graph validation error indicating a circular reference within the graph.
    /// </summary>
    public class CircularReferenceValidationError : ValidationError
    {
        private IModule module;

        /// <summary>
        /// The module involved in the circular reference.
        /// </summary>
        public IModule Module
        {
            get
            {
                return module;
            }
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.CircularReferenceValidationError class.
        /// </summary>
        /// <param name="module">The module involved in the circular reference.</param>
        public CircularReferenceValidationError(IModule module)
        {
            this.module = module;
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.ValidationError.ToString"]/*'/>
        public override string ToString()
        {
            return "Graph contains a circular reference involving module '" + module.GetType().FullName + "'.";
        }
    }
}
