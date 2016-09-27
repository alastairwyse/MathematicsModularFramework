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

namespace MathematicsModularFramework.UnitTests.TestModules
{
    /// <summary>
    /// A test module with 0 inputs and 1 output.
    /// </summary>
    public class Module0_1 : ModuleBase
    {
        private Int32 value1;

        public Int32 Value1
        {
            set
            {
                value1 = value;
            }
        }

        public Module0_1()
            : base()
        {
            Description = "Module with 0 inputs and 1 output";
            AddOutputSlot("Output1", "The first output slot", typeof(Int32));
        }

        protected override void ImplementProcess()
        {
            GetOutputSlot("Output1").DataValue = value1;
        }
    }
}
