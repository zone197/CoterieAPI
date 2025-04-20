using FluentValidation;

namespace Coterie.Api.Models.MiniRater
{
    public class MiniRaterRequestModel
    {
        public string Business { get; set; }
        public double Revenue { get; set; }
        public List<string> States { get; set; }
    }

    public class MiniRaterRequestModelValidator : AbstractValidator<MiniRaterRequestModel>
    {
        public MiniRaterRequestModelValidator()
        {
            RuleFor(x => x.Business)
                .NotEmpty().WithMessage("Business is required.");

            RuleFor(x => x.Revenue)
                .GreaterThan(0).WithMessage("Revenue must be greater than zero.");

            RuleFor(x => x.States)
                .NotEmpty().WithMessage("At least one state is required.")
                .Must(states => states.All(s => !string.IsNullOrWhiteSpace(s)))
                .WithMessage("State names cannot be empty.");
        }
    }
}
