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
    /// Base class containing common properties of a slot, which is a virtual 'container' which can hold a single piece of input or output data either used by or produced by a module.
    /// </summary>
    public abstract class Slot
    {
        /// <summary>The name of the data held by the slot.</summary>
        protected String name;
        /// <summary>A description of the data held by the slot.</summary>
        protected String description;
        /// <summary>The type of the data held by the slot.</summary>
        protected Type dataType;
        /// <summary>The data held by the slot.</summary>
        protected Object dataValue;
        /// <summary>The module that this slot belongs to.</summary>
        protected IModule module;

        /// <summary>
        /// The name of the data held by the slot.
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// A description of the data held by the slot.
        /// </summary>
        public String Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// The type of the data held by the slot.
        /// </summary>
        public Type DataType
        {
            get
            {
                return dataType;
            }
        }

        /// <summary>
        /// The data held by the slot.
        /// </summary>
        public virtual Object DataValue
        {
            set
            {
                dataValue = value;
            }
            get
            {
                return dataValue;
            }
        }

        /// <summary>
        /// The module that this input slot belongs to.
        /// </summary>
        public IModule Module
        {
            get
            {
                return module;
            }
        }

        /// <summary>
        /// Initialises a new instance of the MathematicsModularFramework.Slot class.
        /// </summary>
        /// <param name="name">The name of the data held by the slot.</param>
        /// <param name="description">A description of the data held by the slot.</param>
        /// <param name="dataType">The type of the data held by the slot.</param>
        /// <param name="module">The module that this input slot belongs to.</param>
        protected Slot(String name, String description, Type dataType, IModule module)
        {
            if (name == null || name.Length == 0)
            {
                throw new ArgumentException("Parameter 'name' must be non-null and greater than 0 length.", "name");
            }

            if (description == null || description.Length == 0)
            {
                throw new ArgumentException("Parameter 'description' must be non-null and greater than 0 length.", "description");
            }

            if (dataType == null)
            {
                throw new ArgumentException("Parameter 'dataType' must be non-null.", "dataType");
            }

            if(module == null)
            {
                throw new ArgumentException("Parameter 'module' must be non-null.", "module");
            }

            this.name = name;
            this.description = description;
            this.dataType = dataType;
            dataValue = null;
            this.module = module;
        }
    }
}
