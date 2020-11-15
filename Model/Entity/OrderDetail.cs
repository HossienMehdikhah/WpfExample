using CommonLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfExample.Model.Entity
{
    public class OrderDetail : CommonModel
    {
        public OrderDetail():base()
        {
            ID ??= Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public string ProduceID { get; set; }
        public string OrderID { get; set; }
        public uint Count { get; set; }
        public ulong Price { get; set; }        
        public ulong SumPrice { get; set; }


        public virtual Produce Produce { get; set; }
        public virtual Order Order { get; set; }
    }
}
