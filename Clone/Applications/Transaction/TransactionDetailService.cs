using Indotalent.Data;
using Indotalent.Models.Entities;
using Microsoft.EntityFrameworkCore;
using MWSManagement.DTOs;

public class TransactionDetailService
{
    private readonly ApplicationDbContext _context;

    public TransactionDetailService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<TransDetailDto> GetAll()
    {
        return _context.TransDetailDto.AsNoTracking();
    }
}