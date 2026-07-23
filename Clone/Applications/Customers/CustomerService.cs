using Indotalent.Infrastructures.Repositories;
using MWSManagement.DTOs.Customers;

namespace Indotalent.Applications.Customers
{
    public class CustomerService
    {
        private readonly IProcedureRepository _repo;

        public CustomerService(IProcedureRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CustomerLoyaltyDto>> GetCustomersAsync(
            string? accountNumber = null,
            string? cardNumber = null)
        {
            const string sql = @"
                EXEC [crt].[GETCUSTOMERLOYALTYCARDS]
                    @nvc_CustomerAccountNumber = @AccountNum,
                    @nvc_CardNumber = @CardNum";

            var rawList = await _repo.QueryAsync<CustomerLoyaltyRawDto>(
                sql,
                SqlParam.NVarChar("@AccountNum", accountNumber, 30),
                SqlParam.NVarChar("@CardNum", cardNumber, 50)
            );

            return rawList
                .Where(r => !string.IsNullOrEmpty(r.AccountNum))
                .GroupBy(r => r.AccountNum!)
                .Select(g =>
                {
                    var first = g.First();
                    var cardList = g
                        .Where(r => !string.IsNullOrEmpty(r.CardNumber))
                        .Select(r => r.CardNumber!.Trim())
                        .Distinct()
                        .ToList();

                    return new CustomerLoyaltyDto
                    {
                        AccountNum = first.AccountNum,
                        CustomerName = first.CustomerName,
                        IdentificationNumber = first.IdentificationNumber,
                        MobilePhone = first.MobilePhone,
                        Email = first.Email,
                        BirthDate = (first.BirthDate.HasValue && first.BirthDate.Value.Year > 1900)
                            ? first.BirthDate.Value.ToString("yyyy-MM-dd")
                            : null,
                        FullAddress = first.FullAddress,
                        CardCount = cardList.Count,
                        Cards = string.Join(", ", cardList)
                    };
                })
                .OrderBy(x => x.AccountNum)
                .ToList();
        }

        public async Task<CustomerLoyaltyCreateResultDto?> CreateCustomerLoyaltyCardAsync(CustomerLoyaltyCreateDto input)
        {
            const string sql = @"
                EXEC [crt].[USP_CreateCustomerLoyaltyCard]
                    @CardNumber = @CardNumber,
                    @CustomerAccountNum = @CustomerAccountNum,
                    @CustomerName = @CustomerName,
                    @IdentificationNumber = @IdentificationNumber,
                    @MobilePhone = @MobilePhone,
                    @Email = @Email,
                    @BirthDate = @BirthDate,
                    @FullAddress = @FullAddress,
                    @CardTenderType = @CardTenderType";

            return await _repo.QueryFirstOrDefaultAsync<CustomerLoyaltyCreateResultDto>(
                sql,
                SqlParam.NVarChar("@CardNumber", input.CardNumber, 50),
                SqlParam.NVarChar("@CustomerAccountNum", input.CustomerAccountNum, 30),
                SqlParam.NVarChar("@CustomerName", input.CustomerName, 100),
                SqlParam.NVarChar("@IdentificationNumber", input.IdentificationNumber, 30),
                SqlParam.NVarChar("@MobilePhone", input.MobilePhone, 30),
                SqlParam.NVarChar("@Email", input.Email, 100),
                SqlParam.DateTime("@BirthDate", input.BirthDate),
                SqlParam.NVarChar("@FullAddress", input.FullAddress, 250),
                SqlParam.Int("@CardTenderType", input.CardTenderType)
            );
        }

        public async Task<CustomerLoyaltyDto?> GetCustomerByAccountNumAsync(string accountNum)
        {
            var list = await GetCustomersAsync(accountNumber: accountNum);
            return list.FirstOrDefault();
        }

        public async Task<bool> UpdateCustomerLoyaltyCardAsync(CustomerLoyaltyCreateDto input)
        {
            const string sql = @"
                EXEC [crt].[USP_UpdateCustomerLoyaltyCard]
                    @CustomerAccountNum = @CustomerAccountNum,
                    @CardNumber = @CardNumber,
                    @CustomerName = @CustomerName,
                    @IdentificationNumber = @IdentificationNumber,
                    @MobilePhone = @MobilePhone,
                    @Email = @Email,
                    @BirthDate = @BirthDate,
                    @FullAddress = @FullAddress,
                    @CardTenderType = @CardTenderType";

            var rowsAffected = await _repo.ExecuteAsync(
                sql,
                SqlParam.NVarChar("@CustomerAccountNum", input.CustomerAccountNum, 30),
                SqlParam.NVarChar("@CardNumber", input.CardNumber, 50),
                SqlParam.NVarChar("@CustomerName", input.CustomerName, 100),
                SqlParam.NVarChar("@IdentificationNumber", input.IdentificationNumber, 30),
                SqlParam.NVarChar("@MobilePhone", input.MobilePhone, 30),
                SqlParam.NVarChar("@Email", input.Email, 100),
                SqlParam.DateTime("@BirthDate", input.BirthDate),
                SqlParam.NVarChar("@FullAddress", input.FullAddress, 250),
                SqlParam.Int("@CardTenderType", input.CardTenderType)
            );

            return rowsAffected >= 0;
        }

        public async Task<bool> DeleteCustomerLoyaltyCardAsync(string accountNum)
        {
            const string sql = @"
                EXEC [crt].[USP_DeleteCustomerLoyaltyCard]
                    @CustomerAccountNum = @CustomerAccountNum";

            var rowsAffected = await _repo.ExecuteAsync(
                sql,
                SqlParam.NVarChar("@CustomerAccountNum", accountNum, 30)
            );

            return rowsAffected >= 0;
        }
    }
}