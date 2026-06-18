//using AutoMapper;
//using Indotalent.Infrastructures.Countries;
//using Indotalent.Infrastructures.Extensions;
//using Indotalent.Models.Entities;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using System.ComponentModel;

//namespace Indotalent.Pages.Customers
//{
//    [Authorize]
//    public class CustomerFormModel : PageModel
//    {
//        private readonly IMapper _mapper;
//        private readonly ICountryService _countrySevice;
//        public CustomerFormModel(
//            IMapper mapper,

//            ICountryService countrySevice
//            )
//        {
//            _mapper = mapper;
//            _countrySevice = countrySevice;
//        }

//        [TempData]
//        public string StatusMessage { get; set; } = string.Empty;
//        public string? Action { get; set; } = string.Empty;
//        public string Number { get; set; } = string.Empty;

//        [BindProperty]
//        public CustomerModel CustomerForm { get; set; } = default!;

//        public class CustomerModel
//        {
//            public Guid? RowGuid { get; set; }
//            public int? Id { get; set; }

//            [DisplayName("Name")]
//            public string Name { get; set; } = string.Empty;

//            [DisplayName("Description")]
//            public string? Description { get; set; }

//            [DisplayName("Group")]
//            public int CustomerGroupId { get; set; }

//            [DisplayName("Category")]
//            public int CustomerCategoryId { get; set; }

//            [DisplayName("Street")]
//            public string? Street { get; set; }

//            [DisplayName("City")]
//            public string? City { get; set; }

//            [DisplayName("State")]
//            public string? State { get; set; }

//            [DisplayName("Zip Code")]
//            public string? ZipCode { get; set; }

//            [DisplayName("Country")]
//            public string? Country { get; set; }

//            [DisplayName("Phone Number")]
//            public string? PhoneNumber { get; set; }

//            [DisplayName("Fax Number")]
//            public string? FaxNumber { get; set; }

//            [DisplayName("Email Address")]
//            public string? EmailAddress { get; set; }

//            [DisplayName("Website")]
//            public string? Website { get; set; }

//            [DisplayName("WhatsApp")]
//            public string? WhatsApp { get; set; }

//            [DisplayName("LinkedIn")]
//            public string? LinkedIn { get; set; }

//            [DisplayName("Facebook")]
//            public string? Facebook { get; set; }

//            [DisplayName("Instagram")]
//            public string? Instagram { get; set; }

//            [DisplayName("TwitterX")]
//            public string? TwitterX { get; set; }

//            [DisplayName("TikTok")]
//            public string? TikTok { get; set; }
//        }

//        public class MappingProfile : Profile
//        {
//            public MappingProfile()
//            {

//            }
//        }

//        public ICollection<SelectListItem> CustomerGroupLookup { get; set; } = default!;
//        public ICollection<SelectListItem> CustomerCategoryLookup { get; set; } = default!;
//        public ICollection<SelectListItem> CountryLookup { get; set; } = default!;
//        private void BindLookup()
//        {



//            CountryLookup = _countrySevice.GetCountries();

//        }

//        public async Task OnGetAsync(Guid? rowGuid)
//        {

//            this.SetupViewDataTitleFromUrl();
//            this.SetupStatusMessage();
//            StatusMessage = this.ReadStatusMessage();

//            var action = Request.Query["action"];
//            Action = action;

//            BindLookup();

//            if (rowGuid.HasValue)
//            {

//            }
//            else
//            {
//                CustomerForm = new CustomerModel
//                {
//                    RowGuid = Guid.Empty,
//                    Id = 0
//                };
//            }
//        }

       

//    }
//}
