namespace BookLibrary.Api.Extensions;

internal static class ResultExtensions
{
    public static IResult Html(this IResultExtensions extensions, string html)
    {
        return new HtmlResult(html);
    }
}
