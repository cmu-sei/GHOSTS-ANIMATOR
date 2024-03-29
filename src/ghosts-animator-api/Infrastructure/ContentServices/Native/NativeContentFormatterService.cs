using System;
using System.Linq;
using Ghosts.Animator.Api.Infrastructure.Models;
using Ghosts.Animator.Extensions;
using Ghosts.Animator.Models;
using NLog;

namespace Ghosts.Animator.Api.Infrastructure.ContentServices.Native;

public static class NativeContentFormatterService
{
    private static readonly Logger _log = LogManager.GetCurrentClassLogger();
    
    public static string GenerateTweet(NPC agent)
    {
        string tweetText;
        
        if (agent.Birthdate.Date.DayOfYear == DateTime.Now.Date.DayOfYear)
        {
            tweetText = ProcessBirthday(agent);
        }
        else
        {
            var i = AnimatorRandom.Rand.Next(0, 15);
            tweetText = i switch
            {
                0 => ProcessAddress(agent),
                1 => ProcessFamily(agent),
                2 => ProcessEmployment(agent),
                3 => ProcessEducation(agent),
                4 => ProcessAccount(agent),
                _ => Faker.Lorem.Sentence() //default is just text, no personal information
            };
        }

        return tweetText;
    }
    
    private static string ProcessAccount(NpcProfile agent)
    {
        try
        {
            if (!agent.Accounts.Any()) return "";
            {
                var o = agent.Accounts.RandomElement();
                var list = new[]
                {
                    $"Check out my new post on {o.Url}",
                    $"Check out my new picture uploaded to {o.Url}",
                    $"New post on {o.Url}",
                    $"Join my newsletter on {o.Url}"
                };
                return list.RandomFromStringArray();
            }
        }
        catch (Exception e)
        {
            _log.Error(e);
        }

        return Faker.Lorem.Sentence();
    }

    private static string ProcessBirthday(NpcProfile agent)
    {
        try
        {
            var list = new[]
            {
                "Happy birthday to me!",
                $"Out for dinner on my {DateTime.Now.Year - agent.Birthdate.Year} birthday!",
                $"I'm {DateTime.Now.Year - agent.Birthdate.Year} today!",
                $"{DateTime.Now.Year - agent.Birthdate.Year} looks good on me."
            };
            return list.RandomFromStringArray();
        }
        catch (Exception e)
        {
            _log.Error(e);
        }

        return Faker.Lorem.Sentence();
    }

    private static string ProcessFamily(NpcProfile agent)
    {
        try
        {
            if (!agent.Family.Members.Any()) return "";
            var o = agent.Family.Members.RandomElement();
            var list = new[]
            {
                $"Visiting my {o.Relationship} {o.Name} today.",
                $"Hanging with {o.Name} my {o.Relationship}.",
                $"{o.Relationship} and I say {o.Name} - ",
                $"My {o.Relationship} {o.Name}."
            };
            return $"{list.RandomFromStringArray()} {Faker.Lorem.Sentence()}";
        }
        catch (Exception e)
        {
            _log.Error(e);
        }
        return Faker.Lorem.Sentence();
    }

    private static string ProcessAddress(NpcProfile agent)
    {
        try
        {
            if (!agent.Address.Any()) return "";
            var o = agent.Address.RandomElement();
            var list = new[]
            {
                $"Visiting the {o.State} capital today. Beautiful!",
                $"Chilling in {o.City} today. Literally freezing.",
                $"Love {o.City} - so beautiful!",
                $"Love {o.City} - so great!"
            };
            return list.RandomFromStringArray();
        }
        catch (Exception e)
        {
            _log.Error(e);
        }
        return Faker.Lorem.Sentence();
    }

    private static string ProcessEmployment(NpcProfile agent)
    {
        try
        {
            if (!agent.Employment.EmploymentRecords.Any()) return "";
            var o = agent.Employment.EmploymentRecords.RandomElement();
            var list = new[]
            {
                $"Love working at {o.Company}",
                $"Hanging with my peeps from {o.Company} at the office today!",
                $"{o.Company} is hiring for my team - DM me for details",
                $"My team at {o.Company} is hiring WFH - DM me for info"
            };
            return list.RandomFromStringArray();
        }
        catch (Exception e)
        {
            _log.Error(e);
        }
        return Faker.Lorem.Sentence();
    }

    private static string ProcessEducation(NpcProfile agent)
    {
        try
        {
            if (!agent.Education.Degrees.Any()) return "";
            var o = agent.Education.Degrees.RandomElement();
            var list = new[]
            {
                $"{o.School.Name} is the best school in the world!",
                $"On campus of {o.School.Name} - great to be back!",
                $"The {o.School.Name} campus is beautiful this time of year!",
                $"GO {o.School.Name}!!!"
            };
            return list.RandomFromStringArray();
        }
        catch (Exception e)
        {
            _log.Error(e);
        }
        return Faker.Lorem.Sentence();
    }
}