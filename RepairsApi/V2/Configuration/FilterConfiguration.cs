using System;
using System.Collections.Generic;

namespace RepairsApi.V2.Configuration
{
    public class FilterConfiguration : Dictionary<string, ModelFilterConfiguration>
    {
    }

    public class ModelFilterConfiguration : Dictionary<string, List<FilterOption>>
    {
    }
}
