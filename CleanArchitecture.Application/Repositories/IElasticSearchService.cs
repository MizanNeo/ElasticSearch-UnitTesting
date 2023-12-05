 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Repositories
{
    // Define an interface in your application layer
   
    public interface IElasticSearchService
    {
      public Task<IEnumerable<CleanArchitecture.Domain.Entities.Document>> SearchDocuments(string searchTerm);
        public Task<bool> AddDocument(CleanArchitecture.Domain.Entities.Document request);
    }
}
