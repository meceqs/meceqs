using System;
using Sales.Contracts.Commands;

namespace TrafficGenerator
{
    public static class OrderFactory
    {
        public static Random _random = new Random();

        public static PlaceOrderCommand CreateOrder(Guid customerId)
        {
            var order = new PlaceOrderCommand();
            order.CustomerId = customerId;

            int items = _random.Next(6) + 1 /* to make sure we don't get 0 */;
            for (int i = 0; i < items; i++)
            {
                order.Items.Add(new PlaceOrderItem
                {
                    ItemNumber = GetRandomItemNumber(),
                    Quantity = _random.Next(5) + 1 /* to make sure we don't get 0 */,
                    TotalAmount = _random.Next(1000, 100000) / 100m
                });
            }

            return order;
        }

        private static string GetRandomItemNumber()
        {
            const string characters = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            string itemNumber = "";
            for (int i = 0; i < 5; i++)
            {
                var randomIndex = _random.Next(characters.Length);
                itemNumber += characters[randomIndex];
            }

            return itemNumber;
        }
    }
}