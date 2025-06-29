using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.Application.DTOs;
using Shop.Application.Interfaces;
using Shop.Domain.Interfaces;
using Shop.Infrastructure;


namespace Shop.Application.Services
{


    public class OrderService : IOrderService
    {
            private readonly IRepository<Order> _orderRepo;
            private readonly IRepository<OrderDetail> _orderDetailRepo;
            private readonly IRepository<Product> _productRepo;

            public OrderService(
                IRepository<Order> orderRepo,
                IRepository<OrderDetail> orderDetailRepo,
                IRepository<Product> productRepo)
            {
                _orderRepo = orderRepo;
                _orderDetailRepo = orderDetailRepo;
                _productRepo = productRepo;
            }

        public async Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = (await _orderRepo.GetAllAsync())
                .Where(o => o.UserId == userId)
                .ToList();

            var orderDetails = await _orderDetailRepo.GetAllAsync();
            var products = await _productRepo.GetAllAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = orderDetails
                    .Where(d => d.OrderId == order.OrderId)
                    .Select(d => new OrderDetailDto
                    {
                        OrderDetailId = d.OrderDetailId,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        ProductName = products.FirstOrDefault(p => p.ProductId == d.ProductId)?.ProductName,
                        Status = order.Status
                    }).ToList()
            }).ToList();
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return null;

            var orderDetails = await _orderDetailRepo.GetAllAsync();
            var products = await _productRepo.GetAllAsync();

            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = orderDetails
                    .Where(d => d.OrderId == order.OrderId)
                    .Select(d => new OrderDetailDto
                    {
                        Status = order.Status,
                        OrderDetailId = d.OrderDetailId,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        ProductName = products.FirstOrDefault(p => p.ProductId == d.ProductId)?.ProductName
                    }).ToList()
            };
        }
        public async Task UpdateAsync(OrderDto orderDto)
        {
            var order = await _orderRepo.GetByIdAsync(orderDto.OrderId);
            if (order == null) return;
            order.Status = orderDto.Status;
            order.TotalAmount = orderDto.TotalAmount;
            order.OrderDate = orderDto.OrderDate;
            _orderRepo.Update(order);
            // Cập nhật chi tiết đơn hàng
            foreach (var item in orderDto.Items)
            {
                var orderDetail = await _orderDetailRepo.GetByIdAsync(item.OrderDetailId);
                if (orderDetail != null)
                {
                    orderDetail.Quantity = item.Quantity;
                    orderDetail.UnitPrice = item.UnitPrice;
                    _orderDetailRepo.Update(orderDetail);
                }
            }
            await _orderRepo.SaveChangesAsync();

        }
        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepo.GetAllAsync();
            var orderDetails = await _orderDetailRepo.GetAllAsync();
            var products = await _productRepo.GetAllAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,       
                Items = orderDetails
                    .Where(d => d.OrderId == order.OrderId)
                    .Select(d => new OrderDetailDto
                    {
                        OrderDetailId = d.OrderDetailId,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        ProductName = products.FirstOrDefault(p => p.ProductId == d.ProductId)?.ProductName,
                        Status = order.Status
                    }).ToList()
            }).ToList();
        }
        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return;

            order.Status = status;
            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();
        }




    }


}
