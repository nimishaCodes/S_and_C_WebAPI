using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiMachine.Models
{
    public class ParameterThresholds
    {
        public string parameter_name { get; set; }

        public string strong_lower { get; set; }
        public string weak_lower { get; set; }
        public string strong_upper { get; set; }
        public string weak_upper { get; set; }
        public string good_mean { get; set; }
        public string good_std { get; set; }

    }
}