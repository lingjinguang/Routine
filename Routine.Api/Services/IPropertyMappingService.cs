using System;
using System.Collections.Generic;

namespace Routine.Api.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSoure,TDestination>();
        bool ValidMappingExistsFor<TSoure, TDestination>(string orderBy);
    }
}
