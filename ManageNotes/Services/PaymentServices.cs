using System.Threading.Tasks;
using ManageNotes.Data;
using Microsoft.EntityFrameworkCore;

namespace ManageNotes.Services
{
    public class PaymentServices
    {
        private ApplicationContext _applicationContext;

        public PaymentServices(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _applicationContext
                .Payments
                .AddAsync(payment);
        }

        public async Task<Payment> GetPaymentAsync(string sys_code)
        {
            return await _applicationContext
                .Payments
                .FirstOrDefaultAsync(x => x.Syst_Code == sys_code);
        }
    }
}