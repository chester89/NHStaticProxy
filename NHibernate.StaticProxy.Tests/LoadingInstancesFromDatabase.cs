// ReSharper disable InconsistentNaming

using NHibernate.StaticProxy.Tests.Config;
using NHibernate.StaticProxy.Tests.Entities;
using Xunit;

namespace NHibernate.StaticProxy.Tests
{
    public class LoadingInstancesFromDatabase : NHTestsBase<HbmMappingProvider>, IUseFixture<CustomerFixture>
    {
        private int customerId;
        private CustomerFixture customerFixture;

        public void SetFixture(CustomerFixture data)
        {
            if (customerFixture != null)
            {
                customerFixture.DeleteCustomers(SessionFactory);
                customerId = -1;
            }

            customerFixture = data;

            if (customerFixture != null)
            {
                customerId = customerFixture.AddCustomer(SessionFactory);
            }
        }
        
        
        [Fact]
        public void CanLoadItemFromDatabase()
        {
            using (var s = SessionFactory.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var customer = s.Load<Customer>(customerId);
                Assert.NotNull(customer);
                tx.Commit();
            }
        }

        [Fact]
        public void CanGetItemIdFromDatabase_WithoutGoingtoDb()
        {
            using (var s = SessionFactory.OpenSession())
            //using (var tx = s.BeginTransaction()) // intentionally removed
            {
                s.Disconnect();// intentional

                var customer = s.Load<Customer>(customerId);
                Assert.Equal(customerId, customer.Id);
            }
        }

        [Fact]
        public void CanCheckNotInitializedStatus()
        {
            using (var s = SessionFactory.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                Assert.False(NHibernateUtil.IsInitialized(customer));
            }
        }

        [Fact]
        public void CanGetIdentifier()
        {
            using (var s = SessionFactory.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                Assert.Equal(
                    customerId,
                    s.GetIdentifier(customer));
            }
        }

        [Fact]
        public void CanSetIdentifier_AndGetNewValueFromSession()
        {
            using (var s = SessionFactory.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                customer.Id = 2;
                Assert.Equal(
                    customerId,
                    s.GetIdentifier(customer));
            }
        }

        [Fact]
        public void CanLazyLoadProperties()
        {
            using (var s = SessionFactory.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                Assert.Equal("Zoubi", customer.Name);
            }
        }

        [Fact]
        public void CanLazyLoadPropertiesWithBackingField()
        {
            using (var s = SessionFactory.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                Assert.Equal("Nick", customer.PropertyWithField);
            }
        }

        [Fact]
        public void LazyLoadingPropertiesWithBackingFieldDoesntExecuteSetter()
        {
            using (var s = SessionFactory.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                Assert.Equal("Nick", customer.PropertyWithField);
                Assert.NotEqual(100, customer.Dummy);
            }
        }

        [Fact]
        public void CanLazyLoadFields()
        {
            using (var s = SessionFactory.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                Assert.Equal("fieldOnly", customer.fieldOnly);
            }
        }

        [Fact]
        public void CanCallMethodsWithParameters()
        {
            using (var s = SessionFactory.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                customer.AddOrder(new Order());
            }
        }

        [Fact]
        public void CallingMethodsWillForceLoading()
        {
            using (var s = SessionFactory.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                Assert.False(NHibernateUtil.IsInitialized(customer));
                customer.AddOrder(new Order());
                Assert.True(NHibernateUtil.IsInitialized(customer));
            }
        }
    }
}

// ReSharper restore InconsistentNaming