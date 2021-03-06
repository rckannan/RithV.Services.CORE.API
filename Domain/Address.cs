using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RithV.Services.CORE.API.Events;
using RithV.Services.CORE.API.Infra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RithV.Services.CORE.API.Domain
{
    public class Customer
        : Entity, IAggregateRoot
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        private DateTime DateofBirth;

        // Address is a Value Object pattern example persisted as EF Core 2.0 owned entity
        //public Address Address { get; private set; }

        
        private Int64 UserID;
 
        private string Name;

        private string FullName; 
         
        //private bool _isDraft;

        //private readonly List<Address> _addressitems;
        //public IReadOnlyCollection<Address> AddressItems => _addressitems;

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method OrderAggrergateRoot.AddOrderItem() which includes behaviour.

        public static Customer NewDraft()
        {
            var customer = new Customer();
            //customer._isDraft = true;
            return customer;
        }

        protected Customer()
        { 
            //_isDraft = false;
        }

        public Customer(Int64 id, Int64 userId, string userName, string fullName,   DateTime DOB) : this()
        {
            UserID = userId;
            Name = userName;
            FullName = fullName;
            DateofBirth = DOB;
            Id = id;

            // Add the OrderStarterDomainEvent to the domain events collection 
            // to be raised/dispatched when comitting changes into the Database [ After DbContext.SaveChanges() ]
            // AddOrderStartedDomainEvent( userId, userName, fullName, DOB );
        } 

        private void AddOrderStartedDomainEvent( Int64 userId, string userName, string fullName,  DateTime DOB)
        {
            var customerDomainEvent = new CustomerDomainEvent( userId, userName, fullName, DOB );

            this.AddDomainEvent(customerDomainEvent);
        }

        // DDD Patterns comment
        // This Order AggregateRoot's method "AddOrderitem()" should be the only way to add Items to the Order,
        // so any behavior (discounts, etc.) and validations are controlled by the AggregateRoot 
        // in order to maintain consistency between the whole Aggregate. 
        //public void AddAddressItem( string Street,  string City, string State,string Country, string ZipCode)
        //{
        //    var existingCity = _addressitems.Where(o => o.City == City)
        //        .SingleOrDefault(); 

        //    var addressItem = new Address(Street, City, State, Country, ZipCode );
        //    _addressitems.Add(addressItem); 
        //}

    }

    class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> COREAPIConfiguration)
        {
            COREAPIConfiguration.ToTable("tblCustomer", COREAPIContext.DEFAULT_SCHEMA);

            COREAPIConfiguration.HasKey(o => o.Id);

            COREAPIConfiguration.Ignore(b => b.DomainEvents);
            // COREAPIConfiguration.Ignore(b => b.Id);

            //COREAPIConfiguration.Property(o => o.Id)
            //    .UseHiLo("custseq", COREAPIContext.DEFAULT_SCHEMA);

            //Address value object persisted as owned entity type supported since EF Core 2.0
            //COREAPIConfiguration
            //    .OwnsOne(o => o.Address, a =>
            //    {
            //        a.WithOwner();
            //    });

            COREAPIConfiguration
               .Property<Int64>("Id")
               .UsePropertyAccessMode(PropertyAccessMode.Field)
               .HasColumnName("Id")
               .IsRequired(true);

            COREAPIConfiguration
                .Property<Int64>("UserID")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("UserID")
                .IsRequired(true);

            COREAPIConfiguration
                .Property<string>("Name")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Name")
                .IsRequired();

            COREAPIConfiguration
               .Property<string>("FullName")
               .UsePropertyAccessMode(PropertyAccessMode.Field)
               .HasColumnName("FullName")
               .IsRequired();

            COREAPIConfiguration
                .Property<DateTime>("DateofBirth")
                // .HasField("_orderStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("DateofBirth")
                .IsRequired();


          //  var navigation = COREAPIConfiguration.Metadata.FindNavigation(nameof(Customer.AddressItems));

            // DDD Patterns comment:
            //Set as field (New since EF 1.1) to access the OrderItem collection property through its field
            //navigation.SetPropertyAccessMode(PropertyAccessMode.Field); 
        }
    }
    public class Address : ValueObject
    {
        public String Street { get; private set; }
        public String City { get; private set; }
        public String State { get; private set; }
        public String Country { get; private set; }
        public String ZipCode { get; private set; }

        public Address() { }

        public Address(string street, string city, string state, string country, string zipcode)
        {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return ZipCode;
        }
    }
}
