using CommonLibrary.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfExample.Model.Entity
{
    public class Produce : CommonModel
    {
        public Produce() : base()
        {
            ID ??= Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public ulong Price { get; set; }
        public string Code { get; set; }
    }
}
