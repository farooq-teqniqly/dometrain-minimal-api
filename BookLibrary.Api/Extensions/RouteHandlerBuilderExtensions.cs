using System.Net;
using FluentValidation.Results;

namespace BookLibrary.Api.Extensions;

internal static class RouteHandlerBuilderExtensions
{
    internal static RouteHandlerBuilder ProducesUnauthorizedResponse(this RouteHandlerBuilder builder)
    {
        builder.Produces((int)HttpStatusCode.Unauthorized);
        return builder;
    }

    internal static RouteHandlerBuilder ProducesNotFoundResponse(this RouteHandlerBuilder builder)
    {
        builder.Produces((int)HttpStatusCode.NotFound);
        return builder;
    }

    internal static RouteHandlerBuilder ProducesNoContentResponse(this RouteHandlerBuilder builder)
    {
        builder.Produces((int)HttpStatusCode.NoContent);
        return builder;
    }

    internal static RouteHandlerBuilder ProducesBadRequestResponse(this RouteHandlerBuilder builder)
    {
        builder.Produces<IEnumerable<ValidationFailure>>((int)HttpStatusCode.BadRequest);
        return builder;
    }

    internal static RouteHandlerBuilder ProducesOkResponse<TResponse>(this RouteHandlerBuilder builder)
    {
        builder.Produces<TResponse>();
        return builder;
    }

    internal static RouteHandlerBuilder ProducesCreatedResponse<TResponse>(this RouteHandlerBuilder builder)
    {
        builder.Produces<TResponse>((int)HttpStatusCode.Created);
        return builder;
    }

    internal static RouteHandlerBuilder WithDefaultTags(this RouteHandlerBuilder builder)
    {
        builder.WithTags("Books");
        return builder;
    }

    internal static RouteHandlerBuilder AcceptsJson<TResponse>(this RouteHandlerBuilder builder) where TResponse : notnull
    {
        builder.Accepts<TResponse>("application/json");
        return builder;
    }
}
