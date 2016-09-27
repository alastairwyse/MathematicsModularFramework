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
using NUnit.Framework;

namespace MathematicsModularFramework.UnitTests
{
    /// <summary>
    /// Contains common utility methods used by multiple test classes.
    /// </summary>
    public class TestUtilities
    {
        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.UnitTests.TestUtilities class.
        /// </summary>
        public TestUtilities()
        {
        }

        /// <summary>
        /// Asserts that specified input slot is linked to an output slot with the specified name.
        /// </summary>
        /// <param name="module">The module that the input slot exists on.</param>
        /// <param name="inputSlotName">The name of the input slot.</param>
        /// <param name="outputSlotName">The name of the outputslot to assert.</param>
        /// <param name="graph">The module graph containing the module and slot link.</param>
        public void AssertLinkedOutputSlotHasName(IModule module, String inputSlotName, String outputSlotName, ModuleGraph graph)
        {
            OutputSlot linkedOutputSlot = graph.GetOutputSlotLinkedToInputSlot(module.GetInputSlot(inputSlotName));
            Assert.AreEqual(outputSlotName, linkedOutputSlot.Name);
        }

        /// <summary>
        /// Asserts that the parent module of the specified module linked by the specified input slot is of the specified type.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="inputSlotName">The input slot linking to the parent module to assert.</param>
        /// <param name="parentModuleType">The type of the parent module to assert.</param>
        /// <param name="graph">The module graph containing the modules.</param>
        public void AssertParentModuleIsOfType(IModule module, String inputSlotName, Type parentModuleType, ModuleGraph graph)
        {
            IModule parentModule = graph.GetOutputSlotLinkedToInputSlot(module.GetInputSlot(inputSlotName)).Module;
            Assert.AreEqual(parentModuleType, parentModule.GetType());
        }

        /// <summary>
        /// Asserts that the parent modules specified by the two sets of module and input slot parameters are the same module (i.e. that both input slots are linked to the same parent module).
        /// </summary>
        /// <param name="module1">The first module.</param>
        /// <param name="inputSlotName1">The input slot of the first module.</param>
        /// <param name="module2">The second module.</param>
        /// <param name="inputSlotName2">The input slot of the second module.</param>
        /// <param name="graph">The module graph containing the modules.</param>
        public void AssertParentModulesAreTheSame(IModule module1, String inputSlotName1, IModule module2, String inputSlotName2, ModuleGraph graph)
        {
            IModule parentModule1 = graph.GetOutputSlotLinkedToInputSlot(module1.GetInputSlot(inputSlotName1)).Module;
            IModule parentModule2 = graph.GetOutputSlotLinkedToInputSlot(module2.GetInputSlot(inputSlotName2)).Module;
            Assert.AreSame(parentModule1, parentModule2);
        }

        /// <summary>
        /// Asserts that the parent modules specified by the two sets of module and input slot parameters are NOT the same module (i.e. that each input slot is linked to a different parent module).
        /// </summary>
        /// <param name="module1">The first module.</param>
        /// <param name="inputSlotName1">The input slot of the first module.</param>
        /// <param name="module2">The second module.</param>
        /// <param name="inputSlotName2">The input slot of the second module.</param>
        /// <param name="graph">The module graph containing the modules.</param>
        public void AssertParentModulesAreNotTheSame(IModule module1, String inputSlotName1, IModule module2, String inputSlotName2, ModuleGraph graph)
        {
            IModule parentModule1 = graph.GetOutputSlotLinkedToInputSlot(module1.GetInputSlot(inputSlotName1)).Module;
            IModule parentModule2 = graph.GetOutputSlotLinkedToInputSlot(module2.GetInputSlot(inputSlotName2)).Module;
            Assert.AreNotSame(parentModule1, parentModule2);
        }
    }
}
