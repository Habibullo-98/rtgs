using Microsoft.EntityFrameworkCore;
using RTGS.Models.DB;
using RTGS.Utils;

namespace RTGS.Services.DB;

public class Db(DbContextOptions<Db> options) : DbContext(options)
{
    private DbSet<Transaction> Transactions { get; set; }
    private DbSet<Partner> Partners { get; set; }
    
    public async Task<Partner?> GetPartner(int id)
    {
        return await Partners.FirstOrDefaultAsync(t => t.Id == id);
    }
    
    public bool TransactionExists(int id, int partnerId) => Transactions.Any(t => t.Id == id && t.PartnerId == partnerId);
    
    public async Task<int> AddTransaction(Transaction transaction)
    {
        await Transactions.AddAsync(transaction);
        await SaveChangesAsync();
        return transaction.Id;
    }

    public async Task<Transaction?> GetTransaction(string id)
    {
        return await Transactions.FirstOrDefaultAsync(t => t.TransactionId == id);
    }

    public async Task<List<Transaction>> GetTransactions(Status status)
    {
        return await Transactions.Where(t => t.Status == status).ToListAsync();
    }
    
    public async Task<Transaction?> GetTransaction(int id)
    {
        return await Transactions.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task UpdateTransaction(Transaction transaction)
    {
        Transactions.Update(transaction);
        await SaveChangesAsync();
    }
}