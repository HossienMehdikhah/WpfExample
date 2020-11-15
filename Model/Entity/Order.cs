using CommonLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfExample.Model.Entity
{
    public class Order : CommonModel
    {
        public Order() : base()
        {
            ID ??= Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public string PersonalID { get; set; }
        public ushort Number { get; set; } = 0;
        public DateTime Date { get; set; }


        public virtual Personal Personal { get; set; }
    }
}
