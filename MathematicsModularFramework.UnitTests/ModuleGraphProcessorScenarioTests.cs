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
using MathematicsModularFramework.UnitTests.TestModules;

namespace MathematicsModularFramework.UnitTests
{
    /// <summary>
    /// Unit tests for class MathematicsModularFramework.ModuleGraphProcessor.
    /// <remarks>While test class ModuleGraphProcessorTests contains tests for exception handlers and synthetic tests using mock modules, this class contains various module graph scenarios created using real module classes (i.e. classes derived from the ModuleBase class).  Visual depiction of the graphs in each of the tests is included in file Resources/ModuleGraphProcessorScenarioTests.gif</remarks>
    /// </summary>
    public class ModuleGraphProcessorScenarioTests
    {
        private TestUtilities testUtilities;
        private ModuleGraphProcessor testModuleGraphProcessor;

        [SetUp]
        protected void SetUp()
        {
            testUtilities = new TestUtilities();
            testModuleGraphProcessor = new ModuleGraphProcessor();
        }

        [Test]
        public void Process_ScenarioTest1()
        {
            Module0_2 module1 = new Module0_2();
            module1.Value1 = 3;
            module1.Value2 = 5;
            Module2_0 module2 = new Module2_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module2.GetInputSlot("Input2"));

            testModuleGraphProcessor.Process(graph, false);

            Assert.AreEqual(3, module2.Input1Value);
            Assert.AreEqual(5, module2.Input2Value);
            Assert.AreEqual(0, graph.EndPoints.Count<IModule>());
        }

        [Test]
        public void Process_ScenarioTest2()
        {
            Module0_1 module1 = new Module0_1();
            module1.Value1 = 3;
            Module1_0 module2 = new Module1_0();
            Module1_0 module3 = new Module1_0();
            Module1_0 module4 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.AddModule(module4);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module3.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module4.GetInputSlot("Input1"));

            testModuleGraphProcessor.Process(graph, false);

            Assert.AreEqual(3, module2.Input1Value);
            Assert.AreEqual(3, module3.Input1Value);
            Assert.AreEqual(3, module4.Input1Value);
            Assert.AreEqual(0, graph.EndPoints.Count<IModule>());
        }

        [Test]
        public void Process_ScenarioTest3()
        {
            Module0_2 module1 = new Module0_2();
            module1.Value1 = 7;
            module1.Value2 = 11;
            Module1_0 module2 = new Module1_0();
            Module1_0 module3 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module3.GetInputSlot("Input1"));

            testModuleGraphProcessor.Process(graph, false);

            Assert.AreEqual(7, module2.Input1Value);
            Assert.AreEqual(11, module3.Input1Value);
            Assert.AreEqual(0, graph.EndPoints.Count<IModule>());
        }

        [Test]
        public void Process_ScenarioTest4()
        {
            Module0_1 module1 = new Module0_1();
            module1.Value1 = 53;
            Module2_0 module2 = new Module2_0();
            Module0_1 module3 = new Module0_1();
            module3.Value1 = 59;
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module3.GetOutputSlot("Output1"), module2.GetInputSlot("Input2"));

            testModuleGraphProcessor.Process(graph, false);

            Assert.AreEqual(53, module2.Input1Value);
            Assert.AreEqual(59, module2.Input2Value);
            Assert.AreEqual(0, graph.EndPoints.Count<IModule>());
        }

        [Test]
        public void Process_ScenarioTest5()
        {
            Module0_1 module1 = new Module0_1();
            module1.Value1 = 13;
            Module1_0 module2 = new Module1_0();
            Module0_1 module3 = new Module0_1();
            module3.Value1 = 17;
            Module1_0 module4 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.AddModule(module4);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module3.GetOutputSlot("Output1"), module4.GetInputSlot("Input1"));

            testModuleGraphProcessor.Process(graph, false);

            Assert.AreEqual(13, module2.Input1Value);
            Assert.AreEqual(17, module4.Input1Value);
            Assert.AreEqual(0, graph.EndPoints.Count<IModule>());
        }

        [Test]
        public void Process_ScenarioTest6()
        {
            Module0_2 module1 = new Module0_2();
            module1.Value1 = 19;
            module1.Value2 = 23;
            Module1_1 module2 = new Module1_1();
            Module2_0 module3 = new Module2_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module2.GetOutputSlot("Output1"), module3.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module3.GetInputSlot("Input2"));

            testModuleGraphProcessor.Process(graph, false);

            Assert.AreEqual(19, module3.Input1Value);
            Assert.AreEqual(23, module3.Input2Value);
            Assert.AreEqual(0, graph.EndPoints.Count<IModule>());
        }

        [Test]
        public void Process_ScenarioTest7()
        {
            Module0_1 module1 = new Module0_1();
            module1.Value1 = 29;
            Module1_0 module2 = new Module1_0();
            Module2_0 module3 = new Module2_0();
            Module0_1 module4 = new Module0_1();
            module4.Value1 = 31;
            Module1_0 module5 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.AddModule(module4);
            graph.AddModule(module5);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module3.GetInputSlot("Input1"));
            graph.CreateSlotLink(module4.GetOutputSlot("Output1"), module3.GetInputSlot("Input2"));
            graph.CreateSlotLink(module4.GetOutputSlot("Output1"), module5.GetInputSlot("Input1"));

            testModuleGraphProcessor.Process(graph, false);

            Assert.AreEqual(29, module2.Input1Value);
            Assert.AreEqual(29, module3.Input1Value);
            Assert.AreEqual(31, module3.Input2Value);
            Assert.AreEqual(31, module5.Input1Value);
            Assert.AreEqual(0, graph.EndPoints.Count<IModule>());
        }

        [Test]
        public void Process_ScenarioTest8()
        {
            Module0_1 module1 = new Module0_1();
            module1.Value1 = 37;
            Module2_0 module2 = new Module2_0();
            Module0_2 module3 = new Module0_2();
            module3.Value1 = 41;
            module3.Value2 = 43;
            Module2_0 module4 = new Module2_0();
            Module0_1 module5 = new Module0_1();
            module5.Value1 = 47;
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.AddModule(module4);
            graph.AddModule(module5);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module3.GetOutputSlot("Output1"), module2.GetInputSlot("Input2"));
            graph.CreateSlotLink(module3.GetOutputSlot("Output2"), module4.GetInputSlot("Input1"));
            graph.CreateSlotLink(module5.GetOutputSlot("Output1"), module4.GetInputSlot("Input2"));

            testModuleGraphProcessor.Process(graph, false);

            Assert.AreEqual(37, module2.Input1Value);
            Assert.AreEqual(41, module2.Input2Value);
            Assert.AreEqual(43, module4.Input1Value);
            Assert.AreEqual(47, module4.Input2Value);
            Assert.AreEqual(0, graph.EndPoints.Count<IModule>());
        }

        [Test]
        public void Process_ScenarioTest9()
        {
            Module1_2 module1 = new Module1_2();
            Module1_1 module2 = new Module1_1();
            Module1_0 module3 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module3.GetInputSlot("Input1"));
            graph.CreateSlotLink(module2.GetOutputSlot("Output1"), module1.GetInputSlot("Input1"));

            Exception e = Assert.Throws<Exception>(delegate
            {
                testModuleGraphProcessor.Process(graph, false);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Graph contains a circular reference involving module 'MathematicsModularFramework.UnitTests.TestModules.Module1_2'."));
        }

        [Test]
        public void Process_ScenarioTest10()
        {
            Module0_1 module1 = new Module0_1();
            module1.Value1 = 61;
            Module1_2 module2 = new Module1_2();
            Module1_0 module3 = new Module1_0();
            Module1_0 module4 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.AddModule(module4);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module2.GetOutputSlot("Output1"), module3.GetInputSlot("Input1"));
            graph.CreateSlotLink(module2.GetOutputSlot("Output2"), module4.GetInputSlot("Input1"));

            testModuleGraphProcessor.Process(graph, false);

            Assert.AreEqual(61, module3.Input1Value);
            Assert.AreEqual(61, module4.Input1Value);
            Assert.AreEqual(0, graph.EndPoints.Count<IModule>());
        }

        [Test]
        public void Copy_ScenarioTest1()
        {
            Module0_2 module1 = new Module0_2();
            module1.Value1 = 3;
            module1.Value2 = 5;
            Module2_0 module2 = new Module2_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module2.GetInputSlot("Input2"));

            ModuleGraph copiedGraph = testModuleGraphProcessor.Copy(graph);

            Assert.AreEqual(1, copiedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module2_0), copiedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            IModule endpointModule = copiedGraph.EndPoints.ElementAt<IModule>(0);
            testUtilities.AssertLinkedOutputSlotHasName(endpointModule, "Input1", "Output1", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(endpointModule, "Input2", "Output2", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(endpointModule, "Input1", typeof(Module0_2), copiedGraph);
            testUtilities.AssertParentModulesAreTheSame(endpointModule, "Input1", endpointModule, "Input2", copiedGraph);
        }

        [Test]
        public void Copy_ScenarioTest2()
        {
            Module0_1 module1 = new Module0_1();
            Module1_0 module2 = new Module1_0();
            Module1_0 module3 = new Module1_0();
            Module1_0 module4 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.AddModule(module4);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module3.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module4.GetInputSlot("Input1"));

            ModuleGraph copiedGraph = testModuleGraphProcessor.Copy(graph);

            Assert.AreEqual(3, copiedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module1_0), copiedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            Assert.AreEqual(typeof(Module1_0), copiedGraph.EndPoints.ElementAt<IModule>(1).GetType());
            Assert.AreEqual(typeof(Module1_0), copiedGraph.EndPoints.ElementAt<IModule>(2).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", "Output1", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(2), "Input1", "Output1", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module0_1), copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", typeof(Module0_1), copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(2), "Input1", typeof(Module0_1), copiedGraph);
            testUtilities.AssertParentModulesAreTheSame(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", copiedGraph);
            testUtilities.AssertParentModulesAreTheSame(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", copiedGraph.EndPoints.ElementAt<IModule>(2), "Input1", copiedGraph);
        }


        [Test]
        public void Copy_ScenarioTest3()
        {
            Module0_2 module1 = new Module0_2();
            Module1_0 module2 = new Module1_0();
            Module1_0 module3 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module3.GetInputSlot("Input1"));

            ModuleGraph copiedGraph = testModuleGraphProcessor.Copy(graph);

            Assert.AreEqual(2, copiedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module1_0), copiedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            Assert.AreEqual(typeof(Module1_0), copiedGraph.EndPoints.ElementAt<IModule>(1).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", "Output2", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module0_2), copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", typeof(Module0_2), copiedGraph);
            testUtilities.AssertParentModulesAreTheSame(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", copiedGraph);
        }

        [Test]
        public void Copy_ScenarioTest4()
        {
            Module0_1 module1 = new Module0_1();
            module1.Value1 = 53;
            Module2_0 module2 = new Module2_0();
            Module0_1 module3 = new Module0_1();
            module3.Value1 = 59;
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module3.GetOutputSlot("Output1"), module2.GetInputSlot("Input2"));

            ModuleGraph copiedGraph = testModuleGraphProcessor.Copy(graph);

            Assert.AreEqual(1, copiedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module2_0), copiedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input2", "Output1", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module0_1), copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input2", typeof(Module0_1), copiedGraph);
            testUtilities.AssertParentModulesAreNotTheSame(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", copiedGraph.EndPoints.ElementAt<IModule>(0), "Input2", copiedGraph);
        }

        [Test]
        public void Copy_ScenarioTest5()
        {
            Module0_1 module1 = new Module0_1();
            module1.Value1 = 13;
            Module1_0 module2 = new Module1_0();
            Module0_1 module3 = new Module0_1();
            module3.Value1 = 17;
            Module1_0 module4 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.AddModule(module4);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module3.GetOutputSlot("Output1"), module4.GetInputSlot("Input1"));

            ModuleGraph copiedGraph = testModuleGraphProcessor.Copy(graph);

            Assert.AreEqual(2, copiedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module1_0), copiedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            Assert.AreEqual(typeof(Module1_0), copiedGraph.EndPoints.ElementAt<IModule>(1).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", "Output1", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module0_1), copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", typeof(Module0_1), copiedGraph);
            testUtilities.AssertParentModulesAreNotTheSame(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", copiedGraph);
        }

        [Test]
        public void Copy_ScenarioTest6()
        {
            Module0_2 module1 = new Module0_2();
            module1.Value1 = 19;
            module1.Value2 = 23;
            Module1_1 module2 = new Module1_1();
            Module2_0 module3 = new Module2_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module2.GetOutputSlot("Output1"), module3.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module3.GetInputSlot("Input2"));

            ModuleGraph copiedGraph = testModuleGraphProcessor.Copy(graph);

            Assert.AreEqual(1, copiedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module2_0), copiedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input2", "Output2", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module1_1), copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input2", typeof(Module0_2), copiedGraph);
            IModule module1_1Parent = copiedGraph.GetOutputSlotLinkedToInputSlot(copiedGraph.EndPoints.ElementAt<IModule>(0).GetInputSlot("Input1")).Module;
            testUtilities.AssertLinkedOutputSlotHasName(module1_1Parent, "Input1", "Output1", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(module1_1Parent, "Input1", typeof(Module0_2), copiedGraph);
            testUtilities.AssertParentModulesAreTheSame(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input2", module1_1Parent, "Input1", copiedGraph);
        }

        [Test]
        public void Copy_ScenarioTest7()
        {
            Module0_1 module1 = new Module0_1();
            module1.Value1 = 29;
            Module1_0 module2 = new Module1_0();
            Module2_0 module3 = new Module2_0();
            Module0_1 module4 = new Module0_1();
            module4.Value1 = 31;
            Module1_0 module5 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.AddModule(module4);
            graph.AddModule(module5);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module3.GetInputSlot("Input1"));
            graph.CreateSlotLink(module4.GetOutputSlot("Output1"), module3.GetInputSlot("Input2"));
            graph.CreateSlotLink(module4.GetOutputSlot("Output1"), module5.GetInputSlot("Input1"));

            ModuleGraph copiedGraph = testModuleGraphProcessor.Copy(graph);

            Assert.AreEqual(3, copiedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module1_0), copiedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            Assert.AreEqual(typeof(Module2_0), copiedGraph.EndPoints.ElementAt<IModule>(1).GetType());
            Assert.AreEqual(typeof(Module1_0), copiedGraph.EndPoints.ElementAt<IModule>(2).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", "Output1", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input2", "Output1", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(2), "Input1", "Output1", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module0_1), copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", typeof(Module0_1), copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input2", typeof(Module0_1), copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(2), "Input1", typeof(Module0_1), copiedGraph);
            Assert.AreSame(copiedGraph.GetOutputSlotLinkedToInputSlot(copiedGraph.EndPoints.ElementAt<IModule>(0).GetInputSlot("Input1")), copiedGraph.GetOutputSlotLinkedToInputSlot(copiedGraph.EndPoints.ElementAt<IModule>(1).GetInputSlot("Input1")));
            Assert.AreSame(copiedGraph.GetOutputSlotLinkedToInputSlot(copiedGraph.EndPoints.ElementAt<IModule>(1).GetInputSlot("Input2")), copiedGraph.GetOutputSlotLinkedToInputSlot(copiedGraph.EndPoints.ElementAt<IModule>(2).GetInputSlot("Input1")));
            Assert.AreNotSame(copiedGraph.GetOutputSlotLinkedToInputSlot(copiedGraph.EndPoints.ElementAt<IModule>(1).GetInputSlot("Input1")), copiedGraph.GetOutputSlotLinkedToInputSlot(copiedGraph.EndPoints.ElementAt<IModule>(1).GetInputSlot("Input2")));
            testUtilities.AssertParentModulesAreTheSame(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", copiedGraph);
            testUtilities.AssertParentModulesAreTheSame(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input2", copiedGraph.EndPoints.ElementAt<IModule>(2), "Input1", copiedGraph);
            testUtilities.AssertParentModulesAreNotTheSame(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", copiedGraph.EndPoints.ElementAt<IModule>(1), "Input2", copiedGraph);
        }

        [Test]
        public void Copy_ScenarioTest8()
        {
            Module0_1 module1 = new Module0_1();
            module1.Value1 = 37;
            Module2_0 module2 = new Module2_0();
            Module0_2 module3 = new Module0_2();
            module3.Value1 = 41;
            module3.Value2 = 43;
            Module2_0 module4 = new Module2_0();
            Module0_1 module5 = new Module0_1();
            module5.Value1 = 47;
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.AddModule(module4);
            graph.AddModule(module5);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module3.GetOutputSlot("Output1"), module2.GetInputSlot("Input2"));
            graph.CreateSlotLink(module3.GetOutputSlot("Output2"), module4.GetInputSlot("Input1"));
            graph.CreateSlotLink(module5.GetOutputSlot("Output1"), module4.GetInputSlot("Input2"));

            ModuleGraph copiedGraph = testModuleGraphProcessor.Copy(graph);

            Assert.AreEqual(2, copiedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module2_0), copiedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            Assert.AreEqual(typeof(Module2_0), copiedGraph.EndPoints.ElementAt<IModule>(1).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input2", "Output1", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", "Output2", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input2", "Output1", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module0_1), copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input2", typeof(Module0_2), copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", typeof(Module0_2), copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input2", typeof(Module0_1), copiedGraph);
            testUtilities.AssertParentModulesAreTheSame(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input2", copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", copiedGraph);
            testUtilities.AssertParentModulesAreNotTheSame(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", copiedGraph.EndPoints.ElementAt<IModule>(1), "Input2", copiedGraph);
        }

        [Test]
        public void Copy_ScenarioTest9()
        {
            Module1_2 module1 = new Module1_2();
            Module1_1 module2 = new Module1_1();
            Module1_0 module3 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module3.GetInputSlot("Input1"));
            graph.CreateSlotLink(module2.GetOutputSlot("Output1"), module1.GetInputSlot("Input1"));

            ModuleGraph copiedGraph = testModuleGraphProcessor.Copy(graph);

            Assert.AreEqual(1, copiedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module1_0), copiedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output2", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module1_2), copiedGraph);
            IModule module1_0Parent = copiedGraph.GetOutputSlotLinkedToInputSlot(copiedGraph.EndPoints.ElementAt<IModule>(0).GetInputSlot("Input1")).Module;
            testUtilities.AssertLinkedOutputSlotHasName(module1_0Parent, "Input1", "Output1", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(module1_0Parent, "Input1", typeof(Module1_1), copiedGraph);
            IModule module1_2Parent = copiedGraph.GetOutputSlotLinkedToInputSlot(module1_0Parent.GetInputSlot("Input1")).Module;
            testUtilities.AssertLinkedOutputSlotHasName(module1_2Parent, "Input1", "Output1", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(module1_2Parent, "Input1", typeof(Module1_2), copiedGraph);
            testUtilities.AssertParentModulesAreTheSame(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", module1_2Parent, "Input1", copiedGraph);
        }

        [Test]
        public void Copy_ScenarioTest10()
        {
            Module0_1 module1 = new Module0_1();
            Module1_2 module2 = new Module1_2();
            Module1_0 module3 = new Module1_0();
            Module1_0 module4 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.AddModule(module4);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module2.GetOutputSlot("Output1"), module3.GetInputSlot("Input1"));
            graph.CreateSlotLink(module2.GetOutputSlot("Output2"), module4.GetInputSlot("Input1"));

            ModuleGraph copiedGraph = testModuleGraphProcessor.Copy(graph);

            Assert.AreEqual(2, copiedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module1_0), copiedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            Assert.AreEqual(typeof(Module1_0), copiedGraph.EndPoints.ElementAt<IModule>(1).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", copiedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", "Output2", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module1_2), copiedGraph);
            testUtilities.AssertParentModuleIsOfType(copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", typeof(Module1_2), copiedGraph);
            testUtilities.AssertParentModulesAreTheSame(copiedGraph.EndPoints.ElementAt<IModule>(0), "Input1", copiedGraph.EndPoints.ElementAt<IModule>(1), "Input1", copiedGraph);
            IModule module1_0Parent = copiedGraph.GetOutputSlotLinkedToInputSlot(copiedGraph.EndPoints.ElementAt<IModule>(0).GetInputSlot("Input1")).Module;
            testUtilities.AssertLinkedOutputSlotHasName(module1_0Parent, "Input1", "Output1", copiedGraph);
            testUtilities.AssertParentModuleIsOfType(module1_0Parent, "Input1", typeof(Module0_1), copiedGraph);
        }

        [Test]
        public void Validate_ScenarioTest9()
        {
            Module1_2 module1 = new Module1_2();
            Module1_1 module2 = new Module1_1();
            Module1_0 module3 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module3.GetInputSlot("Input1"));
            graph.CreateSlotLink(module2.GetOutputSlot("Output1"), module1.GetInputSlot("Input1"));

            List<ValidationError> errors = testModuleGraphProcessor.Validate(graph);

            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual(typeof(CircularReferenceValidationError), errors[0].GetType());
            Assert.AreSame(module1, ((CircularReferenceValidationError)errors[0]).Module);
        }

        [Test]
        public void Validate_ScenarioTest11()
        {
            Module2_2 module1 = new Module2_2();
            Module2_2 module2 = new Module2_2();
            Module2_2 module3 = new Module2_2();
            Module1_0 module4 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.AddModule(module4);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input2"));
            graph.CreateSlotLink(module2.GetOutputSlot("Output1"), module3.GetInputSlot("Input1"));
            graph.CreateSlotLink(module2.GetOutputSlot("Output1"), module4.GetInputSlot("Input1"));

            List<ValidationError> errors = testModuleGraphProcessor.Validate(graph);

            Assert.AreEqual(8, errors.Count);
            Assert.AreEqual(typeof(UnlinkedInputSlotValidationError), errors[0].GetType());
            Assert.AreSame(module2.GetInputSlot("Input1"), ((UnlinkedInputSlotValidationError)errors[0]).InputSlot);
            Assert.AreEqual(typeof(UnlinkedInputSlotValidationError), errors[1].GetType());
            Assert.AreSame(module1.GetInputSlot("Input1"), ((UnlinkedInputSlotValidationError)errors[1]).InputSlot);
            Assert.AreEqual(typeof(UnlinkedInputSlotValidationError), errors[2].GetType());
            Assert.AreSame(module1.GetInputSlot("Input2"), ((UnlinkedInputSlotValidationError)errors[2]).InputSlot);
            Assert.AreEqual(typeof(UnlinkedOutputSlotValidationError), errors[3].GetType());
            Assert.AreSame(module1.GetOutputSlot("Output2"), ((UnlinkedOutputSlotValidationError)errors[3]).OutputSlot);
            Assert.AreEqual(typeof(UnlinkedOutputSlotValidationError), errors[4].GetType());
            Assert.AreSame(module2.GetOutputSlot("Output2"), ((UnlinkedOutputSlotValidationError)errors[4]).OutputSlot);
            Assert.AreEqual(typeof(UnlinkedInputSlotValidationError), errors[5].GetType());
            Assert.AreSame(module3.GetInputSlot("Input2"), ((UnlinkedInputSlotValidationError)errors[5]).InputSlot);
            Assert.AreEqual(typeof(UnlinkedOutputSlotValidationError), errors[6].GetType());
            Assert.AreSame(module3.GetOutputSlot("Output1"), ((UnlinkedOutputSlotValidationError)errors[6]).OutputSlot);
            Assert.AreEqual(typeof(UnlinkedOutputSlotValidationError), errors[7].GetType());
            Assert.AreSame(module3.GetOutputSlot("Output2"), ((UnlinkedOutputSlotValidationError)errors[7]).OutputSlot);
        }
    }
}
