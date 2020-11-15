using System;
using System.Collections.Generic;
using System.Text;

namespace WpfExample.Model.Dto
{
    public class OrderDetailDto
    {
        public string ID { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Price { get; set; }       
        public string Count { get; set; }       
        public string SumPrice { get; set; }
    }
}
