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
using System.Xml;
using NUnit.Framework;
using NMock2;
using FrameworkAbstraction;
using MathematicsModularFramework;
using MathematicsModularFramework.UnitTests;
using MathematicsModularFramework.UnitTests.TestModules;
using MathematicsModularFramework.Serialization;

namespace MathematicsModularFramework.Serialization.UnitTests
{
    /// <summary>
    /// Unit tests for class MathematicsModularFramework.Serialization.ModuleGraphXmlSerializer.
    /// </summary>
    public class ModuleGraphXmlSerializerTests
    {
        private String xmlDocumentPath;
        private TestUtilities testUtilities;
        private Mockery mockery;
        private IFileInfo mockFileInfo;
        private IXmlDataSerializer mockXmlDataSerializer;
        private ModuleGraphXmlSerializer testModuleGraphXmlSerializer;

        [SetUp]
        protected void SetUp()
        {
            testUtilities = new TestUtilities();
            mockery = new Mockery();
            mockFileInfo = mockery.NewMock<IFileInfo>();
            mockXmlDataSerializer = mockery.NewMock<IXmlDataSerializer>();
            testModuleGraphXmlSerializer = new ModuleGraphXmlSerializer(mockFileInfo, mockXmlDataSerializer);
            System.Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            xmlDocumentPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\", @"Resources"));
        }

        /// <summary>
        /// Tests that an exception is thrown if the Serialize() method is called with a graph which contains modules from assemblies in different paths.
        /// </summary>
        [Test]
        public void Serialize_DifferingAssemblyPaths()
        {
            Module0_2 module1 = new Module0_2();
            Module2_0 module2 = new Module2_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module2.GetInputSlot("Input2"));

            using (mockery.Ordered)
            {
                Expect.Exactly(2).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Once.On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies2"));
            }

            ModuleGraphSerializationException e = Assert.Throws<ModuleGraphSerializationException>(delegate
            {
                using (MemoryStream memoryStream = new MemoryStream())
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
                {
                    testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                }
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith(@"Error encountered serializing module graph."));
            Assert.IsInstanceOf<Exception>(e.InnerException);
            Assert.That(e.InnerException.Message, NUnit.Framework.Does.StartWith(@"The graph contains modules from differing paths 'C:\Modules\Assemblies' and 'C:\Modules\Assemblies2'.  To allow serialization of the graph, all module's assemblies must exist in the same path."));
        }

        /// <summary>
        /// Tests that XML escape characters are properly serialized and deserialized if they are included in a module's assembly path.
        /// </summary>
        [Test]
        public void Serialize_XmlExcapeCharactersInAssemblyPath()
        {
            const String assemblyFolderName = @"C:\Module%s\Assemblie&s";

            Module0_2 module1 = new Module0_2();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);

            using (mockery.Ordered)
            {
                Expect.Exactly(2).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(assemblyFolderName));
                Expect.Once.On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;

                using (StreamReader streamReader = new StreamReader(memoryStream))
                using (XmlReader xmlReader = XmlReader.Create(streamReader))
                {
                    XmlDocument serializedGraph = new XmlDocument();
                    serializedGraph.Load(xmlReader);

                    XmlNode assemblyNameNode = serializedGraph.ChildNodes[1].ChildNodes[0];

                    Assert.AreEqual(assemblyFolderName, assemblyNameNode.InnerText);
                }
            }
        }

        /// <summary>
        /// Success test for the overload of the Serialize() method using a String return type.  Uses the sample graph from scenario test 1.
        /// </summary>
        [Test]
        public void Serialize_StringOverload()
        {
            String comparisonSerializedGraph;
            using (StreamReader streamReader = new StreamReader(xmlDocumentPath + @"\ScenarioTest1Graph.xml"))
            {
                comparisonSerializedGraph = streamReader.ReadToEnd();
            }

            Module0_2 module1 = new Module0_2();
            Module2_0 module2 = new Module2_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module2.GetInputSlot("Input2"));

            using (mockery.Ordered)
            {
                Expect.Exactly(3).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(2).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            String serializedGraph = testModuleGraphXmlSerializer.Serialize(graph);

            Assert.AreEqual(comparisonSerializedGraph, serializedGraph);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Serialize() method is called on a module graph where an exception is thrown when attempting to serialize the input slot data of one of the modules.
        /// </summary>
        [Test]
        public void Serialize_InputSlotDataSerializationFailure()
        {
            const String exceptionMessage = "Mock Exception.";
            XmlDocument serializedGraph = new XmlDocument();
            InputSlotUnsupportedTypeModule inputSlotUnsupportedTypeModule = new InputSlotUnsupportedTypeModule();
            UnserializableObject inputSlotDataValue = new UnserializableObject();
            inputSlotUnsupportedTypeModule.GetInputSlot("UnserializableObjectInput").DataValue = inputSlotDataValue;
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(inputSlotUnsupportedTypeModule);

            using (mockery.Ordered)
            {
                Expect.Exactly(2).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(2).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
                Expect.Once.On(mockXmlDataSerializer).Method("SerializeDataValue").With(typeof(UnserializableObject), inputSlotDataValue, new NMock2.Matchers.TypeMatcher(typeof(XmlWriter))).Will(Throw.Exception(new Exception(exceptionMessage)));
            }

            ModuleGraphSerializationException e = Assert.Throws<ModuleGraphSerializationException>(delegate
            {
                using (MemoryStream memoryStream = new MemoryStream())
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
                {
                    testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                }
            });
            
            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Error encountered serializing module graph."));
            Assert.IsInstanceOf<DataSerializationException>(e.InnerException);
            Assert.That(e.InnerException.Message, NUnit.Framework.Does.StartWith("Failed to serialize data value for input slot 'UnserializableObjectInput' on module 'MathematicsModularFramework.UnitTests.TestModules.InputSlotUnsupportedTypeModule'."));
            Assert.AreSame(inputSlotDataValue, ((DataSerializationException)e.InnerException).DataValue);
            Assert.AreEqual(exceptionMessage, e.InnerException.InnerException.Message);
        }

        /// <summary>
        /// Tests serializing of an empty module graph.
        /// </summary>
        [Test]
        public void Serialize_EmptyModuleGraph()
        {
            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("EmptyGraph.xml");
            XmlDocument serializedGraph = new XmlDocument();
            DataInputModule dataInputModule = new DataInputModule();
            ModuleGraph graph = new ModuleGraph();

            using (mockery.Ordered)
            {
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        /// <summary>
        /// Tests serializing of modules which have input slots with data assigned.
        /// </summary>
        [Test]
        public void Serialize_InputSlotHasDataAssigned()
        {
            const String testDataValue = "Test String";

            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("GraphWithModuleWithSerializedInputData.xml");
            XmlDocument serializedGraph = new XmlDocument();
            DataInputModule dataInputModule = new DataInputModule();
            dataInputModule.GetInputSlot("StringInput").DataValue = testDataValue;
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(dataInputModule);

            using (mockery.Ordered)
            {
                Expect.Exactly(2).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Once.On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
                Expect.Once.On(mockXmlDataSerializer).Method("SerializeDataValue").With(typeof(String), testDataValue, new NMock2.Matchers.TypeMatcher(typeof(XmlWriter))).Will(new XmlWriterParameterWriteAction(2, testDataValue));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        /// <summary>
        /// Tests serializing of modules which have input slots with data assigned which comes from an external assembly.
        /// </summary>
        [Test]
        public void Serialize_InputSlotHasDataAssignedFromExternalAssembly()
        {
            CustomDataType testData = new CustomDataType();
            const String serializedTestData = "Serialized CustomDataType";

            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("GraphWithModuleWithSerializedInputDataAndDataTypeFromExternalAssembly.xml");
            XmlDocument serializedGraph = new XmlDocument();
            InputSlotCustomTypeModule inputSlotCustomTypeModule = new InputSlotCustomTypeModule();
            inputSlotCustomTypeModule.GetInputSlot("CustomDataTypeInput").DataValue = testData;
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(inputSlotCustomTypeModule);

            using (mockery.Ordered)
            {
                Expect.Exactly(2).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(2).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
                Expect.Once.On(mockXmlDataSerializer).Method("SerializeDataValue").With(typeof(CustomDataType), testData, new NMock2.Matchers.TypeMatcher(typeof(XmlWriter))).Will(new XmlWriterParameterWriteAction(2, serializedTestData));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        /// <summary>
        /// Tests serializing of modules which have input slots with data assigned and are linked via slot link to a parent module's output slot.
        /// </summary>
        /// <remarks>Note, this also implicitly tests input slots which have no data assigned nor are linked (via module 'DataInputModule' and input slot 'DoubleInput')</remarks>
        [Test]
        public void Serialize_LinkedModuleInputSlotHasDataAssigned()
        {
            const Int32 testDataValue = 321;

            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("GraphWithLinkedModuleWithSerializedInputData.xml");
            XmlDocument serializedGraph = new XmlDocument();
            DataInputModule dataInputModule = new DataInputModule();
            dataInputModule.GetInputSlot("Int32Input").DataValue = testDataValue;
            Module0_1 module0_1 = new Module0_1();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(dataInputModule);
            graph.AddModule(module0_1);
            graph.CreateSlotLink(module0_1.GetOutputSlot("Output1"), dataInputModule.GetInputSlot("Int32Input"));

            using (mockery.Ordered)
            {
                Expect.Exactly(3).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Once.On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
                Expect.Once.On(mockXmlDataSerializer).Method("SerializeDataValue").With(typeof(Int32), 321, new NMock2.Matchers.TypeMatcher(typeof(XmlWriter))).Will(new XmlWriterParameterWriteAction(2, testDataValue.ToString()));
                Expect.Once.On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        [Test]
        public void Serialize_ScenarioTest1()
        {
            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("ScenarioTest1Graph.xml");
            XmlDocument serializedGraph = new XmlDocument();
            Module0_2 module1 = new Module0_2();
            Module2_0 module2 = new Module2_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module2.GetInputSlot("Input2"));

            using (mockery.Ordered)
            {
                Expect.Exactly(3).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(2).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            using(MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        [Test]
        public void Serialize_ScenarioTest2()
        {
            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("ScenarioTest2Graph.xml");
            XmlDocument serializedGraph = new XmlDocument();
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

            using (mockery.Ordered)
            {
                Expect.Exactly(5).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(4).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        [Test]
        public void Serialize_ScenarioTest3()
        {
            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("ScenarioTest3Graph.xml");
            XmlDocument serializedGraph = new XmlDocument();
            Module0_2 module1 = new Module0_2();
            Module1_0 module2 = new Module1_0();
            Module1_0 module3 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module3.GetInputSlot("Input1"));

            using (mockery.Ordered)
            {
                Expect.Exactly(4).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(3).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        [Test]
        public void Serialize_ScenarioTest4()
        {
            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("ScenarioTest4Graph.xml");
            XmlDocument serializedGraph = new XmlDocument();
            Module0_1 module1 = new Module0_1();
            Module2_0 module2 = new Module2_0();
            Module0_1 module3 = new Module0_1();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module3.GetOutputSlot("Output1"), module2.GetInputSlot("Input2"));

            using (mockery.Ordered)
            {
                Expect.Exactly(4).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(3).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        [Test]
        public void Serialize_ScenarioTest5()
        {
            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("ScenarioTest5Graph.xml");
            XmlDocument serializedGraph = new XmlDocument();
            Module0_1 module1 = new Module0_1();
            Module1_0 module2 = new Module1_0();
            Module0_1 module3 = new Module0_1();
            Module1_0 module4 = new Module1_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.AddModule(module4);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module3.GetOutputSlot("Output1"), module4.GetInputSlot("Input1"));

            using (mockery.Ordered)
            {
                Expect.Exactly(5).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(4).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        [Test]
        public void Serialize_ScenarioTest6()
        {
            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("ScenarioTest6Graph.xml");
            XmlDocument serializedGraph = new XmlDocument();
            Module0_2 module1 = new Module0_2();
            Module1_1 module2 = new Module1_1();
            Module2_0 module3 = new Module2_0();
            ModuleGraph graph = new ModuleGraph();
            graph.AddModule(module1);
            graph.AddModule(module2);
            graph.AddModule(module3);
            graph.CreateSlotLink(module1.GetOutputSlot("Output1"), module2.GetInputSlot("Input1"));
            graph.CreateSlotLink(module2.GetOutputSlot("Output1"), module3.GetInputSlot("Input1"));
            graph.CreateSlotLink(module1.GetOutputSlot("Output2"), module3.GetInputSlot("Input2"));

            using (mockery.Ordered)
            {
                Expect.Exactly(4).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(3).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        [Test]
        public void Serialize_ScenarioTest7()
        {
            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("ScenarioTest7Graph.xml");
            XmlDocument serializedGraph = new XmlDocument();
            Module0_1 module1 = new Module0_1();
            Module1_0 module2 = new Module1_0();
            Module2_0 module3 = new Module2_0();
            Module0_1 module4 = new Module0_1();
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

            using (mockery.Ordered)
            {
                Expect.Exactly(6).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(5).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        [Test]
        public void Serialize_ScenarioTest8()
        {
            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("ScenarioTest8Graph.xml");
            XmlDocument serializedGraph = new XmlDocument();
            Module0_1 module1 = new Module0_1();
            Module2_0 module2 = new Module2_0();
            Module0_2 module3 = new Module0_2();
            Module2_0 module4 = new Module2_0();
            Module0_1 module5 = new Module0_1();
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

            using (mockery.Ordered)
            {
                Expect.Exactly(6).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(5).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        [Test]
        public void Serialize_ScenarioTest9()
        {
            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("ScenarioTest9Graph.xml");
            XmlDocument serializedGraph = new XmlDocument();
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

            using (mockery.Ordered)
            {
                Expect.Exactly(5).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(3).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        [Test]
        public void Serialize_ScenarioTest10()
        {
            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("ScenarioTest10Graph.xml");
            XmlDocument serializedGraph = new XmlDocument();
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

            using (mockery.Ordered)
            {
                Expect.Exactly(5).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(4).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        [Test]
        public void Serialize_ScenarioTest11()
        {
            XmlDocument comparisonDocument = ReadXmlDocumentFromFile("ScenarioTest11Graph.xml");
            XmlDocument serializedGraph = new XmlDocument();
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

            using (mockery.Ordered)
            {
                Expect.Exactly(5).On(mockFileInfo).GetProperty("DirectoryName").Will(Return.Value(@"C:\Modules\Assemblies"));
                Expect.Exactly(4).On(mockFileInfo).GetProperty("Name").Will(Return.Value("MathematicsModularFramework.UnitTests.TestModules.dll"));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                testModuleGraphXmlSerializer.Serialize(graph, streamWriter);
                memoryStream.Position = 0;
                serializedGraph.Load(memoryStream);

                Assert.AreEqual(comparisonDocument.OuterXml, serializedGraph.OuterXml);
            }
        }

        /// <summary>
        /// Tests that an exception is throw if the Deserialize() method is called on a serialized graph which contains an invalid module id.
        /// </summary>
        [Test]
        public void Deserialize_InvalidModuleId()
        {
            String invalidModuleIdGraph = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><ModuleGraph><AssemblyPath>C:\\Modules\\Assemblies</AssemblyPath><EndPointModules><Module><Id>7</Id></Module></EndPointModules></ModuleGraph>";

            ModuleGraphDeserializationException e = Assert.Throws<ModuleGraphDeserializationException>(delegate
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(invalidModuleIdGraph)))
                using (StreamReader streamReader = new StreamReader(memoryStream))
                {
                    testModuleGraphXmlSerializer.Deserialize(streamReader);
                }
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Error encountered deserializing module graph."));
            Assert.IsInstanceOf<Exception>(e.InnerException);
            Assert.That(e.InnerException.Message, NUnit.Framework.Does.StartWith("XML document refers to invalid module id '7'."));
        }

        /// <summary>
        /// Tests that an exception is thrown is the Deserialize() method is called on a serialized graph in which a module is defined twice with the same id.
        /// </summary>
        [Test]
        public void Deserialize_ModuleIdDefinedTwice()
        {
            String invalidModuleIdGraph = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><ModuleGraph><AssemblyPath>C:\\Modules\\Assemblies</AssemblyPath><EndPointModules><Module><Id>1</Id><Type>MathematicsModularFramework.UnitTests.TestModules.Module1_0</Type><Assembly>MathematicsModularFramework.UnitTests.TestModules.dll</Assembly><InputSlots><InputSlot><Name>Input1</Name></InputSlot></InputSlots></Module><Module><Id>1</Id><Type>MathematicsModularFramework.UnitTests.TestModules.Module1_0</Type><Assembly>MathematicsModularFramework.UnitTests.TestModules.dll</Assembly><InputSlots><InputSlot><Name>Input1</Name></InputSlot></InputSlots></Module></EndPointModules></ModuleGraph>";
            XmlDocument serializedGraph = new XmlDocument();
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(invalidModuleIdGraph)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            using (XmlReader xmlReader = XmlReader.Create(streamReader))
            {
                serializedGraph.Load(xmlReader);
            }
            UpdateGraphAssemblyPath(ref serializedGraph);

            ModuleGraphDeserializationException e = Assert.Throws<ModuleGraphDeserializationException>(delegate
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
                using (StreamReader streamReader = new StreamReader(memoryStream))
                {
                    testModuleGraphXmlSerializer.Deserialize(streamReader);
                }
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Error encountered deserializing module graph."));
            Assert.IsInstanceOf<Exception>(e.InnerException);
            Assert.That(e.InnerException.Message, NUnit.Framework.Does.StartWith("Module with id '1' has already been defined in the XML document."));
        }

        /// <summary>
        /// Tests that an exception is thrown if the Deserialize() method is called on a serialized module graph where an exception is thrown when attempting to deserialize the input slot data of one of the modules.
        /// </summary>
        [Test]
        public void Deserialize_InputSlotDataSerializationFailure()
        {
            const String exceptionMessage = "Mock Exception.";
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("GraphWithModuleWithSerializedInputData.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);

            using (mockery.Ordered)
            {
                Expect.Once.On(mockXmlDataSerializer).Method("DeserializeDataValue").With(typeof(String), new NMock2.Matchers.TypeMatcher(typeof(XmlReader))).Will(Throw.Exception(new Exception(exceptionMessage)));
            }

            ModuleGraphDeserializationException e = Assert.Throws<ModuleGraphDeserializationException>(delegate
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
                using (StreamReader streamReader = new StreamReader(memoryStream))
                {
                    testModuleGraphXmlSerializer.Deserialize(streamReader);
                }
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Error encountered deserializing module graph."));
            Assert.IsInstanceOf<DataDeserializationException>(e.InnerException);
            Assert.That(e.InnerException.Message, NUnit.Framework.Does.StartWith("Failed to deserialize data value for input slot 'StringInput' on module 'MathematicsModularFramework.UnitTests.TestModules.DataInputModule'."));
            Assert.AreEqual(exceptionMessage, e.InnerException.InnerException.Message);
        }

        /// <summary>
        /// Tests deserializing of an empty module graph.
        /// </summary>
        [Test]
        public void Deserialize_EmptyModuleGraph()
        {
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("EmptyGraph.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (mockery.Ordered)
            {
            }

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            Assert.AreEqual(0, deserializedGraph.EndPoints.Count<IModule>());
        }

        /// <summary>
        /// Tests deserializing of modules which have input slots with data assigned.
        /// </summary>
        [Test]
        public void Deserialize_InputSlotHasDataAssigned()
        {
            const String testDataValue = "Test String";
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("GraphWithModuleWithSerializedInputData.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockXmlDataSerializer).Method("DeserializeDataValue").With(typeof(String), new NMock2.Matchers.TypeMatcher(typeof(XmlReader))).Will(new XmlReaderElementConsumeAction(1), Return.Value(testDataValue));
            }

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            IModule dataInputModule = deserializedGraph.EndPoints.ElementAt<IModule>(0);
            Assert.AreEqual(testDataValue, dataInputModule.GetInputSlot("StringInput").DataValue);
        }

        /// <summary>
        /// Tests deserializing of modules which have input slots with data assigned and are linked via slot link to a parent module's output slot.
        /// </summary>
        /// <remarks>Note, this also implicitly tests input slots which have no data assigned nor are linked (via module 'DataInputModule' and input slot 'DoubleInput')</remarks>
        [Test]
        public void Deserialize_LinkedModuleInputSlotHasDataAssigned()
        {
            const Int32 testDataValue = 321;
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("GraphWithLinkedModuleWithSerializedInputData.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockXmlDataSerializer).Method("DeserializeDataValue").With(typeof(Int32), new NMock2.Matchers.TypeMatcher(typeof(XmlReader))).Will(new XmlReaderElementConsumeAction(1), Return.Value(testDataValue));
            }

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            IModule dataInputModule = deserializedGraph.EndPoints.ElementAt<IModule>(0);
            Assert.AreEqual(testDataValue, dataInputModule.GetInputSlot("Int32Input").DataValue);
        }

        /// <summary>
        /// Tests deserializing of modules which have input slots with data assigned which comes from an external assembly.
        /// </summary>
        [Test]
        public void Deserialize_InputSlotHasDataAssignedFromExternalAssembly()
        {
            CustomDataType deserializedCustomDataType = new CustomDataType();
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("GraphWithModuleWithSerializedInputDataAndDataTypeFromExternalAssembly.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockXmlDataSerializer).Method("DeserializeDataValue").With(typeof(CustomDataType), new NMock2.Matchers.TypeMatcher(typeof(XmlReader))).Will(new XmlReaderElementConsumeAction(1), Return.Value(deserializedCustomDataType));
            }

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            IModule inputSlotCustomTypeModule = deserializedGraph.EndPoints.ElementAt<IModule>(0);
            Assert.AreEqual(deserializedCustomDataType, inputSlotCustomTypeModule.GetInputSlot("CustomDataTypeInput").DataValue);
        }

        [Test]
        public void Deserialize_ScenarioTest1()
        {
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("ScenarioTest1Graph.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            Assert.AreEqual(1, deserializedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module2_0), deserializedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            IModule endpointModule = deserializedGraph.EndPoints.ElementAt<IModule>(0);
            testUtilities.AssertLinkedOutputSlotHasName(endpointModule, "Input1", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(endpointModule, "Input2", "Output2", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(endpointModule, "Input1", typeof(Module0_2), deserializedGraph);
            testUtilities.AssertParentModulesAreTheSame(endpointModule, "Input1", endpointModule, "Input2", deserializedGraph);
        }

        [Test]
        public void Deserialize_ScenarioTest2()
        {
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("ScenarioTest2Graph.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            Assert.AreEqual(3, deserializedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module1_0), deserializedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            Assert.AreEqual(typeof(Module1_0), deserializedGraph.EndPoints.ElementAt<IModule>(1).GetType());
            Assert.AreEqual(typeof(Module1_0), deserializedGraph.EndPoints.ElementAt<IModule>(2).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(2), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module0_1), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", typeof(Module0_1), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(2), "Input1", typeof(Module0_1), deserializedGraph);
            testUtilities.AssertParentModulesAreTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", deserializedGraph);
            testUtilities.AssertParentModulesAreTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", deserializedGraph.EndPoints.ElementAt<IModule>(2), "Input1", deserializedGraph);
        }

        [Test]
        public void Deserialize_ScenarioTest3()
        {
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("ScenarioTest3Graph.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            Assert.AreEqual(2, deserializedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module1_0), deserializedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            Assert.AreEqual(typeof(Module1_0), deserializedGraph.EndPoints.ElementAt<IModule>(1).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", "Output2", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module0_2), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", typeof(Module0_2), deserializedGraph);
            testUtilities.AssertParentModulesAreTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", deserializedGraph);
        }

        [Test]
        public void Deserialize_ScenarioTest4()
        {
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("ScenarioTest4Graph.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            Assert.AreEqual(1, deserializedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module2_0), deserializedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input2", "Output1", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module0_1), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input2", typeof(Module0_1), deserializedGraph);
            testUtilities.AssertParentModulesAreNotTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input2", deserializedGraph);
        }

        [Test]
        public void Deserialize_ScenarioTest5()
        {
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("ScenarioTest5Graph.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            Assert.AreEqual(2, deserializedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module1_0), deserializedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            Assert.AreEqual(typeof(Module1_0), deserializedGraph.EndPoints.ElementAt<IModule>(1).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module0_1), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", typeof(Module0_1), deserializedGraph);
            testUtilities.AssertParentModulesAreNotTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", deserializedGraph);
        }

        [Test]
        public void Deserialize_ScenarioTest6()
        {
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("ScenarioTest6Graph.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            Assert.AreEqual(1, deserializedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module2_0), deserializedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input2", "Output2", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module1_1), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input2", typeof(Module0_2), deserializedGraph);
            IModule module1_1Parent = deserializedGraph.GetOutputSlotLinkedToInputSlot(deserializedGraph.EndPoints.ElementAt<IModule>(0).GetInputSlot("Input1")).Module;
            testUtilities.AssertLinkedOutputSlotHasName(module1_1Parent, "Input1", "Output1", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(module1_1Parent, "Input1", typeof(Module0_2), deserializedGraph);
            testUtilities.AssertParentModulesAreTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input2", module1_1Parent, "Input1", deserializedGraph);
        }

        [Test]
        public void Deserialize_ScenarioTest7()
        {
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("ScenarioTest7Graph.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            Assert.AreEqual(3, deserializedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module1_0), deserializedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            Assert.AreEqual(typeof(Module2_0), deserializedGraph.EndPoints.ElementAt<IModule>(1).GetType());
            Assert.AreEqual(typeof(Module1_0), deserializedGraph.EndPoints.ElementAt<IModule>(2).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input2", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(2), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module0_1), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", typeof(Module0_1), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input2", typeof(Module0_1), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(2), "Input1", typeof(Module0_1), deserializedGraph);
            Assert.AreSame(deserializedGraph.GetOutputSlotLinkedToInputSlot(deserializedGraph.EndPoints.ElementAt<IModule>(0).GetInputSlot("Input1")), deserializedGraph.GetOutputSlotLinkedToInputSlot(deserializedGraph.EndPoints.ElementAt<IModule>(1).GetInputSlot("Input1")));
            Assert.AreSame(deserializedGraph.GetOutputSlotLinkedToInputSlot(deserializedGraph.EndPoints.ElementAt<IModule>(1).GetInputSlot("Input2")), deserializedGraph.GetOutputSlotLinkedToInputSlot(deserializedGraph.EndPoints.ElementAt<IModule>(2).GetInputSlot("Input1")));
            Assert.AreNotSame(deserializedGraph.GetOutputSlotLinkedToInputSlot(deserializedGraph.EndPoints.ElementAt<IModule>(1).GetInputSlot("Input1")), deserializedGraph.GetOutputSlotLinkedToInputSlot(deserializedGraph.EndPoints.ElementAt<IModule>(1).GetInputSlot("Input2")));
            testUtilities.AssertParentModulesAreTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", deserializedGraph);
            testUtilities.AssertParentModulesAreTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input2", deserializedGraph.EndPoints.ElementAt<IModule>(2), "Input1", deserializedGraph);
            testUtilities.AssertParentModulesAreNotTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input2", deserializedGraph);
        }

        [Test]
        public void Deserialize_ScenarioTest8()
        {
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("ScenarioTest8Graph.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            Assert.AreEqual(2, deserializedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module2_0), deserializedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            Assert.AreEqual(typeof(Module2_0), deserializedGraph.EndPoints.ElementAt<IModule>(1).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input2", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", "Output2", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input2", "Output1", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module0_1), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input2", typeof(Module0_2), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", typeof(Module0_2), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input2", typeof(Module0_1), deserializedGraph);
            testUtilities.AssertParentModulesAreTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input2", deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", deserializedGraph);
            testUtilities.AssertParentModulesAreNotTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input2", deserializedGraph);
        }

        [Test]
        public void Deserialize_ScenarioTest9()
        {
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("ScenarioTest9Graph.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            Assert.AreEqual(1, deserializedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module1_0), deserializedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output2", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module1_2), deserializedGraph);
            IModule module1_0Parent = deserializedGraph.GetOutputSlotLinkedToInputSlot(deserializedGraph.EndPoints.ElementAt<IModule>(0).GetInputSlot("Input1")).Module;
            testUtilities.AssertLinkedOutputSlotHasName(module1_0Parent, "Input1", "Output1", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(module1_0Parent, "Input1", typeof(Module1_1), deserializedGraph);
            IModule module1_2Parent = deserializedGraph.GetOutputSlotLinkedToInputSlot(module1_0Parent.GetInputSlot("Input1")).Module;
            testUtilities.AssertLinkedOutputSlotHasName(module1_2Parent, "Input1", "Output1", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(module1_2Parent, "Input1", typeof(Module1_2), deserializedGraph);
            testUtilities.AssertParentModulesAreTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", module1_2Parent, "Input1", deserializedGraph);
        }

        [Test]
        public void Deserialize_ScenarioTest10()
        {
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("ScenarioTest10Graph.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            Assert.AreEqual(2, deserializedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module1_0), deserializedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            Assert.AreEqual(typeof(Module1_0), deserializedGraph.EndPoints.ElementAt<IModule>(1).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", "Output2", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module1_2), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", typeof(Module1_2), deserializedGraph);
            testUtilities.AssertParentModulesAreTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", deserializedGraph);
            IModule module1_0Parent = deserializedGraph.GetOutputSlotLinkedToInputSlot(deserializedGraph.EndPoints.ElementAt<IModule>(0).GetInputSlot("Input1")).Module;
            testUtilities.AssertLinkedOutputSlotHasName(module1_0Parent, "Input1", "Output1", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(module1_0Parent, "Input1", typeof(Module0_1), deserializedGraph);
        }

        [Test]
        public void Deserialize_ScenarioTest11()
        {
            XmlDocument serializedGraph = ReadXmlDocumentFromFile("ScenarioTest11Graph.xml");
            UpdateGraphAssemblyPath(ref serializedGraph);
            ModuleGraph deserializedGraph = null;

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedGraph.OuterXml)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                deserializedGraph = testModuleGraphXmlSerializer.Deserialize(streamReader);
            }

            Assert.AreEqual(2, deserializedGraph.EndPoints.Count<IModule>());
            Assert.AreEqual(typeof(Module2_2), deserializedGraph.EndPoints.ElementAt<IModule>(0).GetType());
            Assert.AreEqual(typeof(Module1_0), deserializedGraph.EndPoints.ElementAt<IModule>(1).GetType());
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertLinkedOutputSlotHasName(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", "Output1", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", typeof(Module2_2), deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", typeof(Module2_2), deserializedGraph);
            testUtilities.AssertParentModulesAreTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", deserializedGraph.EndPoints.ElementAt<IModule>(1), "Input1", deserializedGraph);
            Assert.IsFalse(deserializedGraph.OutputSlotIsLinkedToInputSlot(deserializedGraph.EndPoints.ElementAt<IModule>(0).GetInputSlot("Input2")));
            IModule module2_2Parent = deserializedGraph.GetOutputSlotLinkedToInputSlot(deserializedGraph.EndPoints.ElementAt<IModule>(0).GetInputSlot("Input1")).Module;
            testUtilities.AssertLinkedOutputSlotHasName(module2_2Parent, "Input2", "Output1", deserializedGraph);
            testUtilities.AssertParentModuleIsOfType(module2_2Parent, "Input2", typeof(Module2_2), deserializedGraph);
            testUtilities.AssertParentModulesAreNotTheSame(deserializedGraph.EndPoints.ElementAt<IModule>(0), "Input1", module2_2Parent, "Input2", deserializedGraph);
            Assert.IsFalse(deserializedGraph.OutputSlotIsLinkedToInputSlot(module2_2Parent.GetInputSlot("Input1")));
            IModule module2_2ParentsParent = deserializedGraph.GetOutputSlotLinkedToInputSlot(module2_2Parent.GetInputSlot("Input2")).Module;
            Assert.IsFalse(deserializedGraph.OutputSlotIsLinkedToInputSlot(module2_2ParentsParent.GetInputSlot("Input1")));
            Assert.IsFalse(deserializedGraph.OutputSlotIsLinkedToInputSlot(module2_2ParentsParent.GetInputSlot("Input2")));
        }

        /// <summary>
        /// Reads an XML document from the specified file (using the path specified by constant 'xmlDocumentPath').
        /// </summary>
        /// <param name="fileName">The name of the file to read the XML document from.</param>
        /// <returns>The XML document.</returns>
        private XmlDocument ReadXmlDocumentFromFile(String fileName)
        {
            XmlDocument returnDocument = new XmlDocument();

            using (StreamReader streamReader = new StreamReader(xmlDocumentPath + @"\" + fileName))
            using (XmlReader xmlReader = XmlReader.Create(streamReader))
            {
                returnDocument.Load(xmlReader);
            }

            return returnDocument;
        }

        /// <summary>
        /// Updates the &lt;AssemblyPath&gt; path tag in the specified serialized module graph to contain the path for the assembly of the MathematicsModularFramework.UnitTests.TestModules namesapce.
        /// </summary>
        /// <param name="serializedGraph">The serialized module graph to update.</param>
        private void UpdateGraphAssemblyPath(ref XmlDocument serializedGraph)
        {
            XmlNode assemblyNameNode = serializedGraph.ChildNodes[1].ChildNodes[0];
            String testModulesAssemblyLocation = typeof(Module1_0).Assembly.Location;
            String testModulesAssemblyDirectory = new System.IO.FileInfo(testModulesAssemblyLocation).DirectoryName;
            assemblyNameNode.InnerText = testModulesAssemblyDirectory;
        }
    }
}
