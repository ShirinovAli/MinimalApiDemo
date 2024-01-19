using Microsoft.EntityFrameworkCore;
using MinimalApiDemo.Data;
using MinimalApiDemo.Models;
using MinimalApiDemo.Repositories.Abstract;

namespace MinimalApiDemo.Repositories.Concrete
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _context;

        public CouponRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Coupon coupon)
        {
            await _context.Coupons.AddAsync(coupon);
        }

        public async Task<ICollection<Coupon>> GetAllAsync()
        {
            return await _context.Coupons.AsNoTracking().ToListAsync();
        }

        public async Task<Coupon> GetAsync(int id)
        {
            return await _context.Coupons.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<Coupon> GetAsync(string couponName)
        {
            return _context.Coupons.AsNoTracking().FirstOrDefaultAsync(x => x.Name.ToLower() == couponName.ToLower());
        }

        public async Task RemoveAsync(Coupon coupon)
        {
            await Task.Run(() => { _context.Coupons.Remove(coupon); });
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Coupon coupon)
        {
            await Task.Run(() => { _context.Coupons.Update(coupon); });
        }
    }
}
