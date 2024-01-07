using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace COS210_Project_FoodOrderingSystem_
{
    class ItemClass
    {
        public string Id { get; set; }
        public int quantity { get;set; }
        public ItemClass() { }
        public ItemClass(string name,int qty)
        { 
            Id = name;
            quantity = qty;
        }
    }
}
