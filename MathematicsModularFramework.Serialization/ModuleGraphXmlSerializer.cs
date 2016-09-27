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
using System.Xml.Schema;
using System.Reflection;
using FrameworkAbstraction;

namespace MathematicsModularFramework.Serialization
{
    /// <summary>
    /// Serializes and deserializes a module graph to and from an XML document.
    /// </summary>
    public class ModuleGraphXmlSerializer : IModuleGraphSerializer
    {
        /// <summary>The name of the root element in the XML document written and read by the class.</summary>
        protected String rootElementName = "ModuleGraph";
        /// <summary>The name of the element storing the assembly path in the XML document written and read by the class.</summary>
        protected String AssemblyPathElementName = "AssemblyPath";
        /// <summary>The name of the element storing a collection of end point modules in the XML document written and read by the class.</summary>
        protected String endPointModulesElementName = "EndPointModules";
        /// <summary>The name of the element storing a module in the XML document written and read by the class.</summary>
        protected String moduleElementName = "Module";
        /// <summary>The name of the element storing a module id in the XML document written and read by the class.</summary>
        protected String moduleIdElementName = "Id";
        /// <summary>The name of the element storing a module type in the XML document written and read by the class.</summary>
        protected String moduleTypeElementName = "Type";
        /// <summary>The name of the element storing a module assmebly in the XML document written and read by the class.</summary>
        protected String moduleAssemblyElementName = "Assembly";
        /// <summary>The name of the element storing a collection of input slots in the XML document written and read by the class.</summary>
        protected String inputSlotsElementName = "InputSlots";
        /// <summary>The name of the element storing an individual input slot in the XML document written and read by the class.</summary>
        protected String inputSlotElementName = "InputSlot";
        /// <summary>The name of the element storing an individual input slot name in the XML document written and read by the class.</summary>
        protected String inputSlotNameElementName = "Name";
        /// <summary>The name of the element storing a slot's data in the XML document written and read by the class.</summary>
        protected String slotDataElementName = "Data";
        /// <summary>The name of the element storing the type of a slot's data in the XML document written and read by the class.</summary>
        protected String slotDataTypeElementName = "Type";
        /// <summary>The name of the element storing the assembly of the type of a slot's data in the XML document written and read by the class.</summary>
        protected String slotDataTypeAssemblyElementName = "Assembly";
        /// <summary>The name of the element storing the value of a slot's data in the XML document written and read by the class.</summary>
        protected String slotDataValueElementName = "Value";
        /// <summary>The name of the element storing a slot link in the XML document written and read by the class.</summary>
        protected String slotLinkElementName = "SlotLink";
        /// <summary>The name of the element storing an output slot name in the XML document written and read by the class.</summary>
        protected String outputSlotNameElementName = "OutputSlotName";
        private const String xmlSchemaFileLocation = @"Resources\ModuleGraph.xsd";

        /// <summary>Maintains a list of mappings between a module, and its equivalent id number in the XML document.</summary>
        private Dictionary<IModule, Int32> moduleToIdMap;
        /// <summary>Holds a list of modules serialized as part of the serialization process.</summary>
        private HashSet<IModule> serializedModules;
        /// <summary>Maintains a list of mappings between a id number in the XML document, and its equivalent module.</summary>
        private Dictionary<Int32, IModule> idToModuleMap;
        /// <summary> Maintains a list of the assembly modules loaded during deserialization.</summary>
        private Dictionary<String, Assembly> loadedAssemblies;
        /// <summary>Used to retrieve file information regarding a modules assembly.</summary>
        private IFileInfo fileInfo;
        /// <summary>Used to serialize and deserialize data values in module input and output slots.</summary>
        private IXmlDataSerializer dataSerializer;
        /// <summary>Indicates whether the object was instantiated using the test constructor.</summary>
        private Boolean testConstructor = false;
        /// <summary>The full path to the file containing the schema to use to validate XML during the deserializtion process.</summary>
        private String xmlSchemaFilePath;

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.Serialization.ModuleGraphXmlSerializer class.
        /// </summary>
        public ModuleGraphXmlSerializer()
        {
            moduleToIdMap = new Dictionary<IModule, Int32>();
            serializedModules = new HashSet<IModule>();
            idToModuleMap = new Dictionary<int, IModule>();
            loadedAssemblies = new Dictionary<String, Assembly>();
            dataSerializer = new XmlDataSerializer();
            String currentAssemblyPath = new System.IO.FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            xmlSchemaFilePath = Path.Combine(currentAssemblyPath, xmlSchemaFileLocation);
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.Serialization.ModuleGraphXmlSerializer class.
        /// </summary>
        /// <param name="xmlSchemaFilePath">The full path to the file containing the schema to use to validate XML during the deserializtion process.</param>
        public ModuleGraphXmlSerializer(String xmlSchemaFilePath)
            : this()
        {
            this.xmlSchemaFilePath = xmlSchemaFilePath;
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.Serialization.ModuleGraphXmlSerializer class.
        /// </summary>
        /// <param name="dataSerializer">A serializer class, used to serialize and deserialize data values in a module's input and output slots.</param>
        public ModuleGraphXmlSerializer(IXmlDataSerializer dataSerializer)
            : this()
        {
            this.dataSerializer = dataSerializer;
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.Serialization.ModuleGraphXmlSerializer class.
        /// </summary>
        /// <param name="xmlSchemaFilePath">The full path to the file containing the schema to use to validate XML during the deserializtion process.</param>
        /// <param name="dataSerializer">A serializer class, used to serialize and deserialize data values in a module's input and output slots.</param>
        public ModuleGraphXmlSerializer(String xmlSchemaFilePath, IXmlDataSerializer dataSerializer)
            : this()
        {
            this.xmlSchemaFilePath = xmlSchemaFilePath;
            this.dataSerializer = dataSerializer;
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.Serialization.ModuleGraphXmlSerializer class.  Note this is an additional constructor to facilitate unit tests, and should not be used to instantiate the class under normal conditions.
        /// </summary>
        /// <param name="fileInfo">A test (mock) FileInfo object.</param>
        /// <param name="dataSerializer">A serializer class, used to serialize and deserialize data values in a module's input and output slots.</param>
        public ModuleGraphXmlSerializer(IFileInfo fileInfo, IXmlDataSerializer dataSerializer)
            : this()
        {
            this.fileInfo = fileInfo;
            this.dataSerializer = dataSerializer;
            testConstructor = true;
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IModuleGraphSerializer.Serialize(MathematicsModularFramework.ModuleGraph)"]/*'/>
        public String Serialize(ModuleGraph moduleGraph)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(memoryStream))
            {
                Serialize(moduleGraph, streamWriter);
                memoryStream.Position = 0;
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IModuleGraphSerializer.Serialize(MathematicsModularFramework.ModuleGraph,System.IO.StreamWriter)"]/*'/>
        public void Serialize(ModuleGraph moduleGraph, System.IO.StreamWriter streamWriter)
        {
            moduleToIdMap.Clear();
            serializedModules.Clear();

            using (XmlWriter xmlWriter = XmlWriter.Create(streamWriter))
            {
                try
                {
                    // Write the root tag (e.g. <ModuleGraph>)
                    xmlWriter.WriteStartElement(rootElementName);
                    // Write the module assembly path (e.g. <AssemblyPath>)
                    WriteAssemblyPath(moduleGraph, xmlWriter);
                    SerializeEndPointModules(moduleGraph, xmlWriter);
                    // Write the root closing tag (e.g. </ModuleGraph>)
                    xmlWriter.WriteEndElement();
                }
                catch (Exception e)
                {
                    throw new ModuleGraphSerializationException("Error encountered serializing module graph.", e);
                }
            }

            moduleToIdMap.Clear();
            serializedModules.Clear();
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IModuleGraphSerializer.Deserialize(System.String)"]/*'/>
        public ModuleGraph Deserialize(String serializedModuleGraph)
        {
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedModuleGraph)))
            using (StreamReader streamReader = new StreamReader(memoryStream))
            {
                return Deserialize(streamReader);
            }
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MathematicsModularFramework.Serialization.IModuleGraphSerializer.Deserialize(System.IO.StreamReader)"]/*'/>
        public ModuleGraph Deserialize(StreamReader streamReader)
        {
            ModuleGraph returnGraph = new ModuleGraph();

            idToModuleMap.Clear();
            loadedAssemblies.Clear();

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.Schemas.Add(null, xmlSchemaFilePath);
            xmlReaderSettings.ValidationType = ValidationType.Schema;
            xmlReaderSettings.IgnoreWhitespace = true;

            using (XmlReader xmlReader = XmlReader.Create(streamReader, xmlReaderSettings))
            {
                try
                {
                    // Consume the root tag (e.g. <ModuleGraph>)
                    xmlReader.ReadStartElement(rootElementName);
                    String moduleAssemblyPath = ReadAssemblyPath(xmlReader);
                    DeserializeEndPointModules(xmlReader, ref returnGraph, moduleAssemblyPath);
                    // Consume the root closing tag (e.g. </ModuleGraph>)
                    xmlReader.ReadEndElement();
                }
                catch (Exception e)
                {
                    throw new ModuleGraphDeserializationException("Error encountered deserializing module graph.", e);
                }
            }

            idToModuleMap.Clear();

            return returnGraph;
        }

        #region Private Methods

        /// <summary>
        /// Retrieves the common path for all module assemblies in the specified graph, and writes it to the specifed XML writer.
        /// </summary>
        /// <param name="graph">The graph to retrieve the modules from.</param>
        /// <param name="xmlWriter">The XML writer to write the path to.</param>
        private void WriteAssemblyPath(ModuleGraph graph, XmlWriter xmlWriter)
        {
            // Get a list of all modules in the graph
            List<IModule> allModules = new List<IModule>();
            Action<IModule> recursionAction = (IModule module) => { allModules.Add(module); };
            ModuleGraphRecurser recurser = new ModuleGraphRecurser(x => { }, x => { }, x => { }, recursionAction, x => { }, x => { });
            recurser.Recurse(graph, false);

            String assemblyPath = "";
            if (allModules.Count != 0)
            {
                assemblyPath = GetModuleAssemblyDirectoryName(allModules[0]);
                // Check that the assembly location is the same for all modules
                foreach (IModule currentModule in allModules)
                {
                    String currentModuleAssemblyPath = GetModuleAssemblyDirectoryName(currentModule);
                    if (assemblyPath != currentModuleAssemblyPath)
                    {
                        throw new Exception("The graph contains modules from differing paths '" + assemblyPath + "' and '" + currentModuleAssemblyPath + "'.  To allow serialization of the graph, all module's assemblies must exist in the same path.");
                    }
                }
            }

            xmlWriter.WriteElementString(AssemblyPathElementName, assemblyPath);
        }

        /// <summary>
        /// Serializes the end point modules in the specified graph to the specified XML writer.
        /// </summary>
        /// <param name="graph">The graph that the end point modules belong to.</param>
        /// <param name="xmlWriter">The XML writer to serialize to.</param>
        private void SerializeEndPointModules(ModuleGraph graph, XmlWriter xmlWriter)
        {
            // Write the opening tag for the collection of end point modules (e.g. <EndPointModules>)
            xmlWriter.WriteStartElement(endPointModulesElementName);

            foreach (IModule currentModule in graph.EndPoints)
            {
                // Serialize the current module
                SerializeModule(currentModule, graph, xmlWriter);
            }

            // Write the closing tag for the collection of end point modules (e.g. </EndPointModules>)
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Serializes the specified module to the specified XML writer.
        /// </summary>
        /// <param name="module">The module to serialize.</param>
        /// <param name="graph">The graph that the module belongs to.</param>
        /// <param name="xmlWriter">The XML writer to serialize to.</param>
        private void SerializeModule(IModule module, ModuleGraph graph, XmlWriter xmlWriter)
        {
            Int32 moduleId;
            Boolean moduleAlreadySerialized = false;

            // Assign an id to the module
            if (moduleToIdMap.ContainsKey(module) == true)
            {
                moduleId = moduleToIdMap[module];
                moduleAlreadySerialized = true;
            }
            else
            {
                moduleId = GetModuleId(module);
            }

            // Write the module start tag (e.g. <Module>)
            xmlWriter.WriteStartElement(moduleElementName);
            xmlWriter.WriteElementString(moduleIdElementName, moduleId.ToString());
            if (moduleAlreadySerialized == false)
            {
                // Write the module's type and assembly
                xmlWriter.WriteElementString(moduleTypeElementName, module.GetType().FullName);
                String assemblyName = GetObjectAssemblyName(module);
                xmlWriter.WriteElementString(moduleAssemblyElementName, assemblyName);
                SerializeInputSlots(module.Inputs, graph, xmlWriter);
            }
            // Write the module closing tag (e.g. </Module>)
            xmlWriter.WriteEndElement();

            serializedModules.Add(module);
        }

        /// <summary>
        /// Serializes the specified collection of input slots to the specified XML writer.
        /// </summary>
        /// <param name="inputSlots">The input slots to serialize.</param>
        /// <param name="graph">The graph that the parent module of the input slots belongs to.</param>
        /// <param name="xmlWriter">The XML writer to serialize to.</param>
        private void SerializeInputSlots(IEnumerable<InputSlot> inputSlots, ModuleGraph graph, XmlWriter xmlWriter)
        {
            // Write the input slots start tag (e.g. <InputSlots>)
            xmlWriter.WriteStartElement(inputSlotsElementName);
            foreach (InputSlot currentInputSlot in inputSlots)
            {
                SerializeInputSlot(currentInputSlot, graph, xmlWriter);
            }
            // Write the input slots closing tag (e.g. </InputSlots>)
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Serializes the specified input slot to the specified XML writer.
        /// </summary>
        /// <param name="inputSlot">The input slot to serialize.</param>
        /// <param name="graph">The graph that the parent module of the input slot belongs to.</param>
        /// <param name="xmlWriter">The XML writer to serialize to.</param>
        private void SerializeInputSlot(InputSlot inputSlot, ModuleGraph graph, XmlWriter xmlWriter)
        {
            // Write the input slot start tag (e.g. <InputSlot>)
            xmlWriter.WriteStartElement(inputSlotElementName);
            xmlWriter.WriteElementString(inputSlotNameElementName, inputSlot.Name);
            if (inputSlot.DataValue != null)
            {
                SerializeSlotData(inputSlot, xmlWriter);
            }
            if (graph.OutputSlotIsLinkedToInputSlot(inputSlot) == true)
            {
                SerializeSlotLink(inputSlot, graph, xmlWriter);
            }
            // Write the input slot closing tag (e.g. </InputSlot>)
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Serializes the data held by the specified slot to the specified XML writer.
        /// </summary>
        /// <param name="slot">The slot to serialize the data of.</param>
        /// <param name="xmlWriter">The XML writer to serialize to.</param>
        private void SerializeSlotData(Slot slot, XmlWriter xmlWriter)
        {
            // Write the slot data start tag (e.g. <Data>)
            xmlWriter.WriteStartElement(slotDataElementName);
            xmlWriter.WriteElementString(slotDataTypeElementName, slot.DataType.FullName);
            if (DataTypeExistsInCommonLibrary(slot.DataType) == false)
            {
                String assemblyName = GetObjectAssemblyName(slot.DataValue);
                xmlWriter.WriteElementString(slotDataTypeAssemblyElementName, assemblyName);
            }
            xmlWriter.WriteStartElement(slotDataValueElementName);
            try
            {
                dataSerializer.SerializeDataValue(slot.DataType, slot.DataValue, xmlWriter);
            }
            catch (Exception e)
            {
                String slotType;
                if (slot is InputSlot)
                {
                    slotType = "input ";
                }
                else if (slot is OutputSlot)
                {
                    slotType = "output ";
                }
                else
                {
                    slotType = "";
                }
                throw new DataSerializationException("Failed to serialize data value for " + slotType + "slot '" + slot.Name + "' on module '" + slot.Module.GetType().FullName + "'.", slot.DataValue, e);
            }
            xmlWriter.WriteEndElement();
            // Write the slot data closing tag (e.g. </Data>)
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Serializes the slot link for the specified input slot to the specified XML writer.
        /// </summary>
        /// <param name="inputSlot">The input slot of the slot link to serialize.</param>
        /// <param name="graph">The graph that the parent module of the input slot belongs to.</param>
        /// <param name="xmlWriter">The XML writer to serialize to.</param>
        private void SerializeSlotLink(InputSlot inputSlot, ModuleGraph graph, XmlWriter xmlWriter)
        {
            OutputSlot linkedOutputSlot = graph.GetOutputSlotLinkedToInputSlot(inputSlot);

            // Write the slot link start tag (e.g. <SlotLink>)
            xmlWriter.WriteStartElement(slotLinkElementName);
            xmlWriter.WriteElementString(outputSlotNameElementName, linkedOutputSlot.Name);
            SerializeModule(linkedOutputSlot.Module, graph, xmlWriter);
            // Write the slot link closing tag (e.g. </SlotLink>)
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Reads the common path for all module assemblies.
        /// </summary>
        /// <param name="xmlReader">The XML reader to deserialize from.</param>
        /// <returns>The module assembly path.</returns>
        private String ReadAssemblyPath(XmlReader xmlReader)
        {
            return xmlReader.ReadElementString(AssemblyPathElementName);
        }

        /// <summary>
        /// Deserializes the set of end points modules from the specified XML reader.
        /// </summary>
        /// <param name="xmlReader">The XML reader to deserialize from.</param>
        /// <param name="graph">The module graph to create the modules in.</param>
        /// <param name="moduleAssemblyPath">The common path containing the assemblies for all modules in the graph.</param>
        private void DeserializeEndPointModules(XmlReader xmlReader, ref ModuleGraph graph, String moduleAssemblyPath)
        {
            if (xmlReader.IsEmptyElement == true)
            {
                // Consume end point modules self closing tag (e.g. <EndPointModules/>)
                xmlReader.ReadElementString(endPointModulesElementName);
            }
            else
            {
                // Consume the opening tag for the collection of end point modules (e.g. <EndPointModules>)
                xmlReader.ReadStartElement(endPointModulesElementName);

                if (IsStartElement(moduleElementName, true, xmlReader) == true)
                {
                    int baseDepth = xmlReader.Depth;
                    while (xmlReader.Depth >= baseDepth)
                    {
                        DeserializeModule(xmlReader, ref graph, moduleAssemblyPath);
                    }
                }

                // Consume the closing tag for the collection of end point modules (e.g. </EndPointModules>)
                xmlReader.ReadEndElement();
            }
        }

        /// <summary>
        /// Deserializes and returns an individual module from the specified XML reader.
        /// </summary>
        /// <param name="xmlReader">The XML reader to deserialize from.</param>
        /// <param name="graph">The module graph to create the module in.</param>
        /// <param name="moduleAssemblyPath">The common path containing the assemblies for all modules in the graph.</param>
        /// <returns>The deserialized module</returns>
        private IModule DeserializeModule(XmlReader xmlReader, ref ModuleGraph graph, String moduleAssemblyPath)
        {

            IModule module = null;

            // Consume the module start tag (e.g. <Module>)
            xmlReader.ReadStartElement(moduleElementName);
            Int32 moduleId = Convert.ToInt32(xmlReader.ReadElementString(moduleIdElementName));

            // If the next tag is a module type (e.g. <Type>) it indicates this is a new module
            if (IsStartElement(moduleTypeElementName, true, xmlReader) == true)
            {
                String moduleTypeName = xmlReader.ReadElementString(moduleTypeElementName);
                String moduleAssemblyName = xmlReader.ReadElementString(moduleAssemblyElementName);
                Assembly moduleAssembly = GetAssemblyFromName(moduleAssemblyName, moduleAssemblyPath);

                // Create the module and add it to the graph
                if (idToModuleMap.ContainsKey(moduleId) == true)
                {
                    throw new Exception("Module with id '" + moduleId + "' has already been defined in the XML document.");
                }
                Type moduleType = moduleAssembly.GetType(moduleTypeName, true);
                module = (IModule)Activator.CreateInstance(moduleType);
                idToModuleMap.Add(moduleId, module);
                graph.AddModule(module);

                DeserializeInputSlots(xmlReader, ref graph, module, moduleAssemblyPath);
            }
            // Otherwise the module should have already been added to the id map
            else
            {
                if (idToModuleMap.ContainsKey(moduleId) == true)
                {
                    module = idToModuleMap[moduleId];
                }
                else
                {
                    throw new Exception("XML document refers to invalid module id '" + moduleId + "'.");
                }
            }

            // Consume the module end tag (e.g. <Module>)
            xmlReader.ReadEndElement();

            return module;
        }

        /// <summary>
        /// Deserializes the set of input slots from the specified XML reader.
        /// </summary>
        /// <param name="xmlReader">The XML reader to deserialize from.</param>
        /// <param name="graph">The module graph in which to create any links assigned to the input slots.</param>
        /// <param name="module">The module that the input slots belong to.</param>
        /// <param name="moduleAssemblyPath">The common path containing the assemblies for all modules in the graph.</param>
        private void DeserializeInputSlots(XmlReader xmlReader, ref ModuleGraph graph, IModule module, String moduleAssemblyPath)
        {
            if (xmlReader.IsEmptyElement == true)
            {
                // Consume inputs slots self closing tag (e.g. <InputSlots/>)
                xmlReader.ReadElementString(inputSlotsElementName);
            }
            else
            {
                // Consume the input slots start tag (e.g. <InputSlots>)
                xmlReader.ReadStartElement(inputSlotsElementName);

                // If IsStartElement() returns true there are input slots to read.  If it returns false, the next tag is an input slots end tag (e.g. </InputSlot>).
                if (IsStartElement(inputSlotElementName, true, xmlReader) == true)
                {
                    int baseDepth = xmlReader.Depth;
                    while (xmlReader.Depth >= baseDepth)
                    {
                        DeserializeInputSlot(xmlReader, ref graph, module, moduleAssemblyPath);
                    }
                }

                // Consume the input slots end tag (e.g. </InputSlots>)
                xmlReader.ReadEndElement();
            }
        }

        /// <summary>
        /// Deserializes an individual input slot from the specified XML reader.
        /// </summary>
        /// <param name="xmlReader">The XML reader to deserialize from.</param>
        /// <param name="graph">The module graph in which to create any links assigned to the input slot.</param>
        /// <param name="module">The module that the input slot belongs to.</param>
        /// <param name="moduleAssemblyPath">The common path containing the assemblies for all modules in the graph.</param>
        private void DeserializeInputSlot(XmlReader xmlReader, ref ModuleGraph graph, IModule module, String moduleAssemblyPath)
        {
            // Consume the input slot start tag (e.g. <InputSlot>)
            xmlReader.ReadStartElement(inputSlotElementName);

            String inputSlotName = xmlReader.ReadElementString(inputSlotNameElementName);

            // If the next tag is a data tag (e.g <Data>) this input slot has its data value serialized in the graph
            if (IsStartElement(slotDataElementName, false, xmlReader) == true)
            {
                DeserializeInputSlotData(xmlReader, module, inputSlotName, moduleAssemblyPath);
            }

            // If the next tag is a slot link (e.g. <SlotLink>) it indicates this input slot is linked to an output slot
            if (IsStartElement(slotLinkElementName, true, xmlReader) == true)
            {
                DeserializeSlotLink(xmlReader, ref graph, module, inputSlotName, moduleAssemblyPath);
            }

            // Consume the input slot end tag (e.g. </InputSlot>)
            xmlReader.ReadEndElement();
        }

        /// <summary>
        /// Deserializes the data value of an input slot from the specified XML reader.
        /// </summary>
        /// <param name="xmlReader">The XML reader to deserialize from.</param>
        /// <param name="module">The module that the input slot data belongs to.</param>
        /// <param name="inputSlotName">The name of the input slot to deserialize the data value for.</param>
        /// <param name="moduleAssemblyPath">The common path containing the assemblies for all modules in the graph.</param>
        private void DeserializeInputSlotData(XmlReader xmlReader, IModule module, String inputSlotName, String moduleAssemblyPath)
        {
            // Consume the data start tag (e.g. <Data>)
            xmlReader.ReadStartElement(slotDataElementName);

            String dataTypeString = xmlReader.ReadElementString(slotDataTypeElementName);
            Type dataType = null;
            if (xmlReader.IsStartElement(slotDataTypeAssemblyElementName) == true)
            {
                String assemblyName = xmlReader.ReadElementString(slotDataTypeAssemblyElementName);
                Assembly dataTypeAssembly = GetAssemblyFromName(assemblyName, moduleAssemblyPath);
                dataType = dataTypeAssembly.GetType(dataTypeString, true);
            }
            else
            {
                dataType = Type.GetType(dataTypeString);
            }

            // Consume the data value start tag (e.g. <Value>)
            xmlReader.ReadStartElement(slotDataValueElementName);
            try
            {
                Object slotDataValue = dataSerializer.DeserializeDataValue(dataType, xmlReader);
                module.GetInputSlot(inputSlotName).DataValue = slotDataValue;
            }
            catch (Exception e)
            {
                throw new DataDeserializationException("Failed to deserialize data value for input slot '" + inputSlotName + "' on module '" + module.GetType().FullName + "'.", e);
            }
            // Consume the data value end tag (e.g. </Value>)
            xmlReader.ReadEndElement();
            // Consume the data end tag (e.g. </Data>)
            xmlReader.ReadEndElement();
        }

        /// <summary>
        /// Deserializes a slot link from the specified XML reader.
        /// </summary>
        /// <param name="xmlReader">The XML reader to deserialize from.</param>
        /// <param name="graph">The module graph to create the slot link in.</param>
        /// <param name="module">The module that the input slot of the slot link belongs to.</param>
        /// <param name="inputSlotName">The name of the input slot in the link.</param>
        /// <param name="moduleAssemblyPath">The common path containing the assemblies for all modules in the graph.</param>
        private void DeserializeSlotLink(XmlReader xmlReader, ref ModuleGraph graph, IModule module, String inputSlotName, String moduleAssemblyPath)
        {
            // Consume the slot link start tag (e.g. <SlotLink>)
            xmlReader.ReadStartElement(slotLinkElementName);

            String outputSlotName = xmlReader.ReadElementString(outputSlotNameElementName);
            IModule linkedModule = DeserializeModule(xmlReader, ref graph, moduleAssemblyPath);

            // Create the slot link in the graph
            graph.CreateSlotLink(linkedModule.GetOutputSlot(outputSlotName), module.GetInputSlot(inputSlotName));

            // Consume the slot link end tag (e.g. </SlotLink>)
            xmlReader.ReadEndElement();
        }

        /// <summary>
        /// Returns true if the current node in the specified XML reader is a start element and the name of the element matches the name parameter.  Returns false if the node is an end element.
        /// </summary>
        /// <param name="name">The qualified name of the element.</param>
        /// <param name="throwExceptionOnNameMismatch">Whether to throw an exception if the start element name does not match the name parameter.  In such a case, true throws an exception, false returns false.</param>
        /// <param name="xmlReader">The XML reader to read from.</param>
        /// <returns>Indicates whether the current node is a start element (true) or an end element (false).</returns>
        private bool IsStartElement(String name, Boolean throwExceptionOnNameMismatch, XmlReader xmlReader)
        {
            bool returnValue;

            if (xmlReader.NodeType == XmlNodeType.EndElement)
            {
                returnValue = false;
            }
            else if (xmlReader.NodeType == XmlNodeType.Element)
            {
                if (xmlReader.Name != name)
                {
                    if (throwExceptionOnNameMismatch == true)
                    {
                        throw new Exception("Element '" + name + "' was not found.");
                    }
                    else
                    {
                        returnValue = false;
                    }
                }
                else
                {
                    returnValue = true;
                }
            }
            else
            {
                throw new Exception("Encountered node type '" + xmlReader.NodeType.ToString() + "' when expecting either a start or an end element.");
            }

            return returnValue;
        }


        /// <summary>
        /// Adds the specified module to private member 'moduleToIdMap', assigning it and returning a new incremental id number.
        /// </summary>
        /// <param name="module">The module to return the id for.</param>
        /// <returns>The id of the module.</returns>
        private Int32 GetModuleId(IModule module)
        {
            Int32 newId = 0;

            foreach (Int32 currentId in moduleToIdMap.Values)
            {
                if (currentId > newId)
                {
                    newId = currentId;
                }
            }
            newId = newId + 1;
            moduleToIdMap.Add(module, newId);

            return newId;
        }

        /// <summary>
        /// Returns the name of the assembly of the specified object.
        /// </summary>
        /// <param name="inputObject">The object to get the assembly name for.</param>
        /// <returns>The assembly name.</returns>
        private String GetObjectAssemblyName(Object inputObject)
        {
            if (testConstructor == false)
            {
                fileInfo = new FrameworkAbstraction.FileInfo(new System.IO.FileInfo(inputObject.GetType().Assembly.Location));
            }
            return fileInfo.Name;
        }

        /// <summary>
        /// Returns the name of the directory containing the assembly of the specified module.
        /// </summary>
        /// <param name="module">The module to get the assembly directory name for.</param>
        /// <returns>The assembly directory name.</returns>
        private String GetModuleAssemblyDirectoryName(IModule module)
        {
            if (testConstructor == false)
            {
                fileInfo = new FrameworkAbstraction.FileInfo(new System.IO.FileInfo(module.GetType().Assembly.Location));
            }
            return fileInfo.DirectoryName;
        }
        
        /// <summary>
        /// Returns true of the specified type exists in the .NET common object runtime library (mscorlib).
        /// </summary>
        /// <param name="type">Teh data type to check.</param>
        /// <returns>True if the data type exists in the .NET common object runtime library, false otherwise.</returns>
        private Boolean DataTypeExistsInCommonLibrary(Type type)
        {
            // TODO: Find a more robust way to do this than simple string matching.
            //    May fail in different languages or on non-Windows platforms??
            if (type.Assembly.FullName.StartsWith("mscorlib") == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the assembly with the specified name if the assembly already occurs in the list of loaded assemblies, or otherwise loads the assembly with the specified name.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to return.</param>
        /// <param name="assemblyPath">The full path to the assembly file.</param>
        /// <returns>The assembly.</returns>
        private Assembly GetAssemblyFromName(String assemblyName, String assemblyPath)
        {
            Assembly returnAssembly = null;

            if (loadedAssemblies.ContainsKey(assemblyName) == true)
            {
                returnAssembly = loadedAssemblies[assemblyName];
            }
            else
            {
                returnAssembly = System.Reflection.Assembly.LoadFrom(Path.Combine(assemblyPath, assemblyName));
                loadedAssemblies.Add(assemblyName, returnAssembly);
            }

            return returnAssembly;
        }

        #endregion
    }
}
