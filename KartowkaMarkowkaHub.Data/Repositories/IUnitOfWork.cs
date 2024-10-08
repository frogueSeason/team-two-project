﻿using KartowkaMarkowkaHub.Core.Domain;

namespace KartowkaMarkowkaHub.Data.Repositories
{
    public interface IUnitOfWork
    {
        void Save();

        GenericRepository<Product> ProductRepository { get; }
        GenericRepository<Order> OrderRepository { get; }
        GenericRepository<OrderStatus> OrderStatusRepository { get; }
    }
}