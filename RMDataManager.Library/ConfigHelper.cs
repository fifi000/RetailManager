﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library
{
    public class ConfigHelper
    {
        public static double GetTaxRate()
        {
            string taxRateText = ConfigurationManager.AppSettings["taxRate"];

            bool IsValidTax = double.TryParse(taxRateText, out double tax);

            if (IsValidTax == false || tax < 0)
            {
                throw new ConfigurationErrorsException("The tax rate is not a valid number");
            }

            return tax;
        }
    }
}
