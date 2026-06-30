//using Indotalent.Applications.CustomerCategoryAXs;
//using Indotalent.DTOs;
//using Microsoft.AspNetCore.OData.Query;
//using Microsoft.AspNetCore.OData.Routing.Controllers;

//namespace Indotalent.ApiOData
//{
//    public class CustomerCategoryAXController : ODataController
//    {
//        private readonly CustomerCategoryAXService _CustomerCategoryAXService;

//        public CustomerCategoryAXController(CustomerCategoryAXService CustomerCategoryAXService)
//        {
//            _CustomerCategoryAXService = CustomerCategoryAXService;
//        }

//        [EnableQuery]
//        public IQueryable<CustomerCategoryAXDto> Get()
//        {
//            return _CustomerCategoryAXService
//                .GetAll()
//                .Select(rec => new CustomerCategoryAXDto
//                {
//                    Id = rec.Id,
//                    Name = rec.Name,
//                });
//        }


//    }
//}
