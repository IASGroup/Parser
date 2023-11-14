using FluentValidation;
using TaskManager.ParserTasks.Commands.CreateParserTask.Request;

namespace TaskManager.ParserTasks.Commands.CreateParserTask.Validators;

public class CreateParserTaskCommandValidator : AbstractValidator<CreateParserTaskCommand>
{
	public CreateParserTaskCommandValidator()
	{
		RuleFor(x => x.Url).NotEmpty();
		RuleFor(x => x.TypeId).NotEmpty();
		RuleFor(x => x.Name).NotEmpty();
		RuleFor(x => x.ParserTaskUrlOptions).NotEmpty()
			.ChildRules(x => x.RuleFor(y => y!.RequestMethod).NotEmpty()
		);
	}
}