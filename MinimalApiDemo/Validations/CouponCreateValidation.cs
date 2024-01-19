using FluentValidation;
using MinimalApiDemo.DTOs;

namespace MinimalApiDemo.Validations
{
    public class CouponCreateValidation : AbstractValidator<CouponCreateDto>
    {
        public CouponCreateValidation()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).NotEmpty().InclusiveBetween(1, 100);
        }
    }
}
