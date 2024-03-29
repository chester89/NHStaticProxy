﻿using Iesi.Collections.Generic;

namespace NHibernate.StaticProxy.Tests.Entities
{
    [StaticProxy]
    public class Customer
    {
        private string propertyWithField;
        
        public Customer()
        {
            Orders = new HashedSet<Order>();
        }

        public int Id { get; set; }
        
        public string Name { get; set; }

        public string NotMapped { get; set; }

        public string PropertyWithField
        {
            get
            {
                return propertyWithField;
            }
            set
            {
                propertyWithField = value;

                Dummy = 100;
            }
        }

        public string fieldOnly;

        public int Dummy { get; set; }

        public ISet<Order> Orders { get; set; }
        
        public void AddOrder(Order order)
        {
            Orders.Add(order);
            order.Customer = this;
        }
    }
}
