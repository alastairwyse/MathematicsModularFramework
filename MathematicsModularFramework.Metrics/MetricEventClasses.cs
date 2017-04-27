/*
 * Copyright 2017 Alastair Wyse (http://www.oraclepermissiongenerator.net/mathematicsmodularframework/)
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
using ApplicationMetrics;

namespace MathematicsModularFramework.Metrics
{
    public class ModuleProcessed : CountMetric
    {
        public ModuleProcessed()
        {
            base.name = "ModuleProcessed";
            base.description = "The number of modules processed";
        }
    }

    public class ModuleProcessingTime : IntervalMetric
    {
        public ModuleProcessingTime()
        {
            base.name = "ModuleProcessingTime";
            base.description = "The time taken to process a module";
        }
    }

    public class ModuleGraphProcessed : CountMetric
    {
        public ModuleGraphProcessed()
        {
            base.name = "ModuleGraphProcessed";
            base.description = "The number of module graphs processed";
        }
    }

    public class ModuleGraphProcessingTime : IntervalMetric
    {
        public ModuleGraphProcessingTime()
        {
            base.name = "ModuleGraphProcessingTime";
            base.description = "The time taken to process a module graph";
        }
    }

    public class ModuleGraphProcessingCancelled : CountMetric
    {
        public ModuleGraphProcessingCancelled()
        {
            base.name = "ModuleGraphProcessingCancelled";
            base.description = "The number of times processing of a module graph was cancelled";
        }
    }

    public class ModuleGraphCopied : CountMetric
    {
        public ModuleGraphCopied()
        {
            base.name = "ModuleGraphCopied";
            base.description = "The number of module graphs copied";
        }
    }

    public class ModuleGraphCopyingTime : IntervalMetric
    {
        public ModuleGraphCopyingTime()
        {
            base.name = "ModuleGraphCopyingTime";
            base.description = "The time taken to copy a module graph";
        }
    }
}
