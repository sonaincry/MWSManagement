using Indotalent.DTOs;
using Indotalent.Models.DTOs;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Indotalent.ApiOData
{
    public class RetailTransactionController : ODataController
    {
        private readonly RetailTransactionService _service;

        public RetailTransactionController(
            RetailTransactionService service)
        {
            _service = service;
        }

        [EnableQuery]
        public IQueryable<RetailTransactionDTO> Get()
        {
            var test = _service.GetAll().Take(1000).ToList();

            return test.Select(x => new RetailTransactionDTO
            {
                TransactionId = x.TransactionId,    
                Type = x.Type,
                ReceiptId = x.ReceiptId,
                Store = x.Store,                                           
                Terminal = x.Terminal,
                Staff = x.Staff,
                TransDate = x.TransDate,
                NetAmount = x.NetAmount,
                GrossAmount = x.GrossAmount,
                PaymentAmount = x.PaymentAmount,


                TransDateSearch = x.TransDate.ToString("dd-MM-yyyy")
            }).AsQueryable();
        }
    }
}