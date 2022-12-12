// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Collections.Generic;
using Ghosts.Animator.Enums;
using Ghosts.Animator.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Ghosts.Animator.Api.Infrastructure.Models;

public class InsiderThreatGenerationConfiguration
{
    // A campaign is the top level of an engagement
    public string Campaign { get; set; }

    // Enclaves are specific subnets of a range, (or a larger number of people) 
    public IList<EnclaveConfiguration> Enclaves { get; set; }
}

public class InsiderThreatGenerationConfigurationExample : IExamplesProvider<InsiderThreatGenerationConfiguration>
{
    public InsiderThreatGenerationConfiguration GetExamples()
    {
        return new InsiderThreatGenerationConfiguration
        {
            Campaign = "GCD 2021",
            Enclaves = new List<EnclaveConfiguration>
            {
                new()
                {
                    Name = "RCC-K",
                    Teams = new List<TeamConfiguration>
                    {
                        new()
                        {
                            Name = "INTEL", DomainTemplate = "intel{machine_number}-rcc-k.disa.mil",
                            MachineNameTemplate = "INTEL{machine_number}",
                            Npcs = new NpcConfiguration
                            {
                                Number = 10,
                                Configuration = new NpcGenerationConfiguration
                                    {Branch = MilitaryBranch.USARMY, Unit = "", RankDistribution = new List<RankDistribution>()}
                            }
                        }
                    }
                }
            }
        };
    }
}