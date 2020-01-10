using System;

using Capibara.Domain.Models;

using Prism.Navigation;

namespace Capibara.Presentation.Navigation
{
    public class NavigationParameterBuilder
    {
        public IModel Model { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public NavigationParameters Build()
        {
            var result = new NavigationParameters();

            if (this.Model.IsPresent())
            {
                result.Add(ParameterNames.Model, this.Model);
            }

            if (this.Title.IsPresent())
            {
                result.Add(ParameterNames.Title, this.Title);
            }

            if (this.Url.IsPresent())
            {
                result.Add(ParameterNames.Url, this.Url);
            }

            return result;
        }
    }
}
