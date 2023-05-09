namespace Ghosts.Animator.Api.Infrastructure.Extensions;

public static class StringExtensions
{
    public static string ReplaceDoubleQuotesWithSingleQuotes(this string input)
    {
        return input.Replace("\"", "'");
    }
}