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
    /// A link between an output slot and an input slot.  A slot link represents a path in the module graph between an output slot of one module and an input slot of another module.  After a module is processed the resulting data in the module's output slots is passed to any input slots referenced by slot links.
    /// </summary>
    class SlotLink
    {
        /// <summary>The output slot in the link.</summary>
        private OutputSlot outputSlot;
        /// <summary>The input slot in the link.</summary>
        private InputSlot inputSlot;

        /// <summary>
        /// The output slot in the link.
        /// </summary>
        public OutputSlot OutputSlot
        {
            get
            {
                return outputSlot;
            }
        }

        /// <summary>
        /// The input slot in the link.
        /// </summary>
        public InputSlot InputSlot
        {
            get
            {
                return inputSlot;
            }
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.SlotLink class.
        /// </summary>
        /// <param name="outputSlot">The output slot in the link.</param>
        /// <param name="inputSlot">The input slot in the link.</param>
        public SlotLink(OutputSlot outputSlot, InputSlot inputSlot)
        {
            if (inputSlot.DataType.IsAssignableFrom(outputSlot.DataType) == false)
            {
                throw new ArgumentException("Data type '" + inputSlot.DataType.FullName + "' of input slot '" + inputSlot.Name + "' on module '" + inputSlot.Module.GetType().FullName + "' is not assignable from data type '" + outputSlot.DataType.FullName + "' of output slot '" + outputSlot.Name + "' on module '" + outputSlot.Module.GetType().FullName + "'.", "inputSlot");
            }

            this.inputSlot = inputSlot;
            this.outputSlot = outputSlot;
        }
    }
}
