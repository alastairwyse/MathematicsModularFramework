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
    /// The exception that is thrown when an attempt is made to re-process a module which has already been processed.
    /// </summary>
    public class ModuleAlreadyProcessedException : Exception
    {
        /// <summary>The module that was attempted to be re-processed.</summary>
        protected ModuleBase module;

        /// <summary>
        /// The module that was attempted to be re-processed.
        /// </summary>
        public ModuleBase Module
        {
            get
            {
                return module;
            }
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.ModuleAlreadyProcessedException class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="module">The module that was attempted to be re-processed.</param>
        public ModuleAlreadyProcessedException(String message, ModuleBase module)
            : base (message)
        {
            this.module = module;
        }
    }
}
