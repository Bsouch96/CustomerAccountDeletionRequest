using AutoMapper;
using CustomerAccountDeletionRequest.DomainModels;
using CustomerAccountDeletionRequest.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAccountDeletionRequest.Profiles
{
    public class DeletionRequestProfile : Profile
    {
        public DeletionRequestProfile()
        {
            CreateMap<DeletionRequestModel, DeletionRequestCreateDTO>();
        }
    }
}
