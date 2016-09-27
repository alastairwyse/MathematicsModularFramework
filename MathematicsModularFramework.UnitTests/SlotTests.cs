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
    /// Unit tests for class MathematicsModularFramework.Slot.
    /// </summary>
    /// <remarks>As the Slot class is abstract, tests are performed via derived class OutputSlot.</remarks>
    public class SlotTests
    {
        private OutputSlot testOutputSlot;

        [SetUp]
        protected void SetUp()
        {
        }

        /// <summary>
        /// Tests if a null 'name' parameter is passed to the constructor.
        /// </summary>
        [Test]
        public void Constructor_NullName()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testOutputSlot = new OutputSlot(null, "Output slot created for unit testing.", typeof(Int32), new TestModule());
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'name' must be non-null and greater than 0 length."));
            Assert.AreEqual("name", e.ParamName);
        }

        /// <summary>
        /// Tests if a blank 'name' parameter is passed to the constructor.
        /// </summary>
        [Test]
        public void Constructor_BlankName()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testOutputSlot = new OutputSlot("", "Output slot created for unit testing.", typeof(Int32), new TestModule());
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'name' must be non-null and greater than 0 length."));
            Assert.AreEqual("name", e.ParamName);
        }

        /// <summary>
        /// Tests if a null 'description' parameter is passed to the constructor.
        /// </summary>
        [Test]
        public void Constructor_NullDescription()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testOutputSlot = new OutputSlot("TestOutputSlot", null, typeof(Int32), new TestModule());
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'description' must be non-null and greater than 0 length."));
            Assert.AreEqual("description", e.ParamName);
        }

        /// <summary>
        /// Tests if a blank 'description' parameter is passed to the constructor.
        /// </summary>
        [Test]
        public void Constructor_BlankDescription()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testOutputSlot = new OutputSlot("TestOutputSlot", "", typeof(Int32), new TestModule());
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'description' must be non-null and greater than 0 length."));
            Assert.AreEqual("description", e.ParamName);
        }

        /// <summary>
        /// Tests if a null 'dataType' parameter is passed to the constructor.
        /// </summary>
        [Test]
        public void Constructor_NullDataType()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testOutputSlot = new OutputSlot("TestOutputSlot", "Output slot created for unit testing.", null, new TestModule());
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'dataType' must be non-null."));
            Assert.AreEqual("dataType", e.ParamName);
        }

        /// <summary>
        /// Tests if a null 'module' parameter is passed to the constructor.
        /// </summary>
        [Test]
        public void Constructor_NullModule()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testOutputSlot = new OutputSlot("TestOutputSlot", "Output slot created for unit testing.", typeof(Int32), null);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'module' must be non-null."));
            Assert.AreEqual("module", e.ParamName);
        }

        #region Test Classes

        private class TestModule : ModuleBase
        {
            public TestModule()
                : base()
            {
                description = "Module created for unit testing.";
                AddInputSlot("Slot1", "Slot1 (type = String)", typeof(String));
            }

            protected override void ImplementProcess()
            {
            }
        }

        #endregion
    }
}
