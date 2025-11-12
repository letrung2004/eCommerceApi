using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Application.Interfaces.IRepository;

namespace OrderService.Application.Features.Order.Queries.OrderDetailById.GetOrderItemById
{
    public class GetOrderItemByIdQueryHandler : IRequestHandler<GetOrderItemByIdQuery, List<OrderItemResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;
        public GetOrderItemByIdQueryHandler(IUnitOfWork unitOfWork,
            IOrderItemRepository orderItemRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderItemResponse>> Handle(GetOrderItemByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.GetRepository<Domain.Entities.Order>()
                    .GetByIdAsync(request.OrderId);

            if (order == null)
                throw new ArgumentException("Order not found");

            var orderItems = await _orderItemRepository.GetOrderItemsByOrderId(request.OrderId);
            return orderItems;

        }
    }
}
