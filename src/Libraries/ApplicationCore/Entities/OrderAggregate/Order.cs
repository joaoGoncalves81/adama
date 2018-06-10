﻿using ApplicationCore.Interfaces;
using ApplicationCore.Entities;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Entities.OrderAggregate
{
    public class Order : BaseEntity, IAggregateRoot
    {
        private Order()
        {
        }

        public Order(string buyerId, int? taxNumber, Address shipToAddress, Address billingAddress, bool useBillingSameAsShipping, List<OrderItem> items, decimal shippingCost)
        {
            ShipToAddress = shipToAddress;
            BillingToAddress = billingAddress;
            UseBillingSameAsShipping = useBillingSameAsShipping;
            ShippingCost = shippingCost;
            _orderItems = items;
            BuyerId = buyerId;
            TaxNumber = taxNumber;
        }
        public string BuyerId { get; private set; }
        public int? TaxNumber { get; private set; }
        public DateTimeOffset OrderDate { get; private set; } = DateTimeOffset.Now;
        public Address ShipToAddress { get; private set; }
        public Address BillingToAddress { get; private set; }
        public decimal ShippingCost { get; private set; }
        public Boolean UseBillingSameAsShipping { get; private set; }
        public OrderStateType OrderState { get; set; }

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method Order.AddOrderItem() which includes behavior.
        private readonly List<OrderItem> _orderItems = new List<OrderItem>();

        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
        // Using List<>.AsReadOnly() 
        // This will create a read only wrapper around the private list so is protected against "external updates".
        // It's much cheaper than .ToList() because it will not have to copy all items in a new collection. (Just one heap alloc for the wrapper instance)
        //https://msdn.microsoft.com/en-us/library/e78dcd75(v=vs.110).aspx 

        public decimal Total()
        {
            var total = 0m;
            foreach (var item in _orderItems)
            {
                total += item.UnitPrice * item.Units;
            }
            return total + ShippingCost;
        }

    }
}