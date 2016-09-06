using System;
using System.Collections.Generic;

namespace Sales.Contracts.Commands
{
    public class PlaceOrderCommand
    {
        public Guid CustomerId { get; set; }

        public List<PlaceOrderItem> Items { get; set; } = new List<PlaceOrderItem>();
    }

    public class PlaceOrderItem
    {
        public string ItemNumber { get; set; }

        public int Quantity { get; set; }

        public decimal TotalAmount { get; set; }
    }
}