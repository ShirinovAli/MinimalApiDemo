using FluentValidation;
using MinimalApiDemo.DTOs;

namespace MinimalApiDemo.Validations
{
    public class CouponUpdateValidation : AbstractValidator<CouponUpdateDto>
    {
        public CouponUpdateValidation()
        {
            RuleFor(model => model.Id).NotEmpty().GreaterThan(0);
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).NotEmpty().InclusiveBetween(1, 100);
        }
    }
}
