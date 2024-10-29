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

        private readonly List<Customer>[] _rewardedCustomers = new List<Customer>[7];

        private const int DailyLimit = 5;

        private const int NumOfDays = 7;

        public CampaignService() {

            for (int i = 0; i < _rewardedCustomers.Length; i++)
            {
                _rewardedCustomers[i] = new List<Customer>();
            }
        }

        public bool RewardCustomer(Customer customer, int day)
        {
            if (_rewardedCustomers[day].Count <= DailyLimit)
            {
                _rewardedCustomers[day].Add(customer);
                return true;
            }

            return false;
        }

        public List<Customer> GetRewardedCustomers()
        {
            return _rewardedCustomers.SelectMany(list => list).ToList(); ;
        }

    }
}
