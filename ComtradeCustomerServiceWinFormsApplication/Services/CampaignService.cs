using ComtradeCustomerServiceWinFormsApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeCustomerServiceWinFormsApplication.Services
{
    class CampaignService
    {

        private readonly List<Customer> _rewardedCustomers = new List<Customer>();

        private const int DailyLimit = 5;

        public bool RewardCustomer(Customer customer)
        {
            if (_rewardedCustomers.Count < DailyLimit && !customer.IsRewarded)
            {
                customer.IsRewarded = true;
                _rewardedCustomers.Add(customer);
                return true;
            }
            return false;
        }

        public List<Customer> GetRewardedCustomers()
        {
            return _rewardedCustomers;
        }

    }
}
