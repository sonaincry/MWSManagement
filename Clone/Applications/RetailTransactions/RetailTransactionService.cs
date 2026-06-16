using Indotalent.Data;
using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class RetailTransactionService
{
    private readonly ApplicationDbContext _context;

    public RetailTransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<RetailTransaction> GetAll()
    {
        return _context.RetailTransaction.AsNoTracking();
    }
}