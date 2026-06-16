using AutoMapper;
using Indotalent.DTOs;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.ApiOData
{

    public class SalesOrderItemChildController : ODataController
    {

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
            }
        }

        private readonly IMapper _mapper;




    }
}
