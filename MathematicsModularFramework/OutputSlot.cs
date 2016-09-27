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
    /// A container class representing an output slot.  An output slot is a virtual 'container' which can hold a single piece of output data.    This data is produced by calling the module's Process() method, and can passed to input slots of other modules.
    /// </summary>
    public class OutputSlot : Slot
    {
        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.OutputSlot class.
        /// </summary>
        /// <param name="name">The name of the data held by the slot.</param>
        /// <param name="description">A description of the data held by the slot.</param>
        /// <param name="dataType">The type of the data held by the slot.</param>
        /// <param name="module">The module that this output slot belongs to.</param>
        public OutputSlot(String name, String description, Type dataType, IModule module)
            : base(name, description, dataType, module)
        {
        }
    }
}
