using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Core;
using EPiServer.Validation;
using Foundation.Cms.Attributes;
using Foundation.Cms.Pages;
using Foundation.Find.Cms;

namespace Foundation.Demo.Validation
{
    public class SingleInstancesValidator : IValidate<PageData>
    {
        private readonly ICmsSearchService _searchService;
        public SingleInstancesValidator(ICmsSearchService seaechService)
        {
            _searchService = seaechService;
        }
        public IEnumerable<ValidationError> Validate(PageData instance)
        {
            var singleInstanceAttribute = instance.GetType().GetCustomAttribute<SingleInstancesAttribute>(true);

            if (singleInstanceAttribute == null)
            {
                return Enumerable.Empty<ValidationError>();
            }

            // call search service to get all existing instances of page type

            var existingInstances = _searchService.SearchByPageType<SearchResultPage>().ToList();

            if (existingInstances.Any())
            {
                if (existingInstances.Count > 0)
                {
                    // if we already have a instance of this page in find then check scope of instance
                    if (singleInstanceAttribute.Scope == SingleInstancesAttribute.InstanceScope.Site)
                    {
                        // Error
                        return new[]
                        {
                            new ValidationError
                            {
                                ErrorMessage =
                                    $"Only one instances of this page type can exist.",
                                PropertyName = "PageType",
                                Severity = ValidationErrorSeverity.Error,
                                ValidationType = ValidationErrorType.StorageValidation
                            }
                        };
                    }
                    else if (singleInstanceAttribute.Scope == SingleInstancesAttribute.InstanceScope.SameContentTree)
                    {
                        if (existingInstances.Any(x => x.ParentLink == instance.ParentLink))
                        {
                            //Error
                            return new[]
                            {
                                new ValidationError
                                {
                                    ErrorMessage =
                                        $"Only one instances of this page type can exist at this level",
                                    PropertyName = "PageName",
                                    Severity = ValidationErrorSeverity.Error,
                                    ValidationType = ValidationErrorType.StorageValidation
                                }
                            };
                        }
                    }
                }
            }
            
            return Enumerable.Empty<ValidationError>();
        }
    }
}
