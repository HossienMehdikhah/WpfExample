using CommonLibrary.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace WpfExample.Model.Entity
{
    public class Personal : CommonModel
    {
        public Personal() : base()
        {
            ID ??= Guid.NewGuid().ToString();
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        [Phone]
        public string Tell { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
