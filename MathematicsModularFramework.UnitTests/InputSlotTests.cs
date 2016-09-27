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
using MathematicsModularFramework;

namespace MathematicsModularFramework.UnitTests
{
    /// <summary>
    /// Unit tests for class MathematicsModularFramework.InputSlot.
    /// </summary>
    public class InputSlotTests
    {
        private InputSlot testInputSlot;

        [SetUp]
        protected void SetUp()
        {
            testInputSlot = new InputSlot("TestInputSlot", "Input slot created for unit testing", typeof(Int32), new TestModule());
        }

        /// <summary>
        /// Success test for property DataAssigned().
        /// </summary>
        [Test]
        public void DataAssigned()
        {
            Assert.IsFalse(testInputSlot.DataAssigned);

            testInputSlot.DataValue = 123;

            Assert.IsTrue(testInputSlot.DataAssigned);
        }

        #region Test Classes

        private class TestModule : ModuleBase
        {
            public TestModule()
                : base()
            {
                description = "Module created for unit testing.";
            }

            protected override void ImplementProcess()
            {
            }
        }

        #endregion
    }
}
