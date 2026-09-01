using FluentValidation;

namespace OrderManagement.Application.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryValidator
    : AbstractValidator<GetOrdersQuery>
{
    public GetOrdersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}