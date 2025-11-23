using AutoMapper;
using DMS.Core.Dto;
using DMS.Core.Entities;
using Microsoft.Extensions.Configuration;

namespace DMS.Services.Helper
{
    public class DocumentUrlResolver : IValueResolver<Document, DocumentGetDto, string>
    {
        private readonly IConfiguration _config;
        public DocumentUrlResolver(IConfiguration config)
        {
            _config = config;
        }

        public string Resolve(Document source, DocumentGetDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.DocumentContent))
            {
                return source.DocumentContent;
            }
            return null;
        }
    }

}
