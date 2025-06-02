namespace Application.UseCases.Users.Queries;

using FluentValidation;

public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(q => q.Pagination)
            .NotNull()
            .WithMessage("Pagination is required.")
            .DependentRules(() =>
            {
                RuleFor(q => q.Pagination!.PageIndex)
                    .GreaterThan(0)
                    .WithMessage("PageIndex must be greater than 0.");

                RuleFor(q => q.Pagination!.PageSize)
                    .InclusiveBetween(1, 100)
                    .WithMessage("PageSize must be between 1 and 100.");
            });
    }
}
