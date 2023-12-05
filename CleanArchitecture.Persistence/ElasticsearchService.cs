// ElasticsearchService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Application.Repositories;
using Elasticsearch.Net;
using Nest;
using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Persistence
{
    public class ElasticsearchService : IElasticSearchService
    {
        private readonly IElasticClient _client;

        public ElasticsearchService(IConfiguration configuration)
        {
            var settings = new ConnectionSettings(new Uri(configuration["ElasticsearchSettings:Uri"]))
                .DefaultIndex("cleanarch").CertificateFingerprint("01f1c5163ec7ea565efd31a970c40fe309337908581a2dc8c4abeb8f77a632a4").EnableDebugMode();

            //var settings = new ConnectionSettings(new Uri(mEsQuerySource.Url));
            settings.EnableHttpCompression();
            //settings.en
            settings.EnableApiVersioningHeader();
            settings.BasicAuthentication("elastic", "l_r3CxrE9yegwEZOucco");
            _client = new ElasticClient(settings);
        }

        public async Task<bool> AddDocument(Document request)
        {
            var indexName = "cleanarch"; // Replace with your desired index name
            //used rest api if fullfleged needed then i need to use logstash or FluentD
            var response = await _client.IndexAsync(request, idx => idx.Index(indexName));
            return response.IsValid;
           
        }
        public async Task<IEnumerable<Document>> SearchDocuments(string searchTerm)
        {
            var searchResponse = await _client.SearchAsync<Document>(s => s
                 .Query(q => q
                     .Match(m => m
                         .Field(f => f.Content)
                         .Query(searchTerm)
                     )
                 )
             );

            return (IEnumerable<Document>)searchResponse.Documents;
        }
    }
}
