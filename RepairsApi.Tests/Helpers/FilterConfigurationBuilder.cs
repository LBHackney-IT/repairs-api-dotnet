using RepairsApi.V2.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairsApi.Tests.Helpers
{
    public class FilterConfigurationBuilder
    {
        private FilterConfiguration _filters = new FilterConfiguration();

        public FilterConfigurationBuilder AddModel(string modelKey, Action<ModelFilterConfigurationBuilder> setup)
        {
            var modelBuilder = new ModelFilterConfigurationBuilder();
            setup(modelBuilder);
            _filters.Add(modelKey, modelBuilder.Build());
            return this;
        }

        public FilterConfiguration Build() => _filters;
    }

    public class ModelFilterConfigurationBuilder
    {
        private ModelFilterConfiguration _filters = new ModelFilterConfiguration();

        public ModelFilterConfigurationBuilder AddFilter(string filterName, Action<FilterOptionBuilder> setup)
        {
            var optionBuilder = new FilterOptionBuilder();
            setup(optionBuilder);
            _filters.Add(filterName, optionBuilder.Build());
            return this;
        }

        internal ModelFilterConfiguration Build() => _filters;
    }

    public class FilterOptionBuilder
    {
        private List<FilterOption> _options = new List<FilterOption>();

        public FilterOptionBuilder AddOption(string optionKey, string optionDescription)
        {
            _options.Add(new FilterOption()
            {
                Description = optionDescription,
                Key = optionKey
            });
            return this;
        }

        internal List<FilterOption> Build() => _options;
    }
}
