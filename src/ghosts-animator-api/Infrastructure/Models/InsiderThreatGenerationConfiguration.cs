/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

using System.Collections.Generic;
using Ghosts.Animator.Enums;
using Ghosts.Animator.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Ghosts.Animator.Api.Infrastructure.Models
{
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
                    new EnclaveConfiguration
                    {
                        Name = "RCC-K",
                        Teams = new List<TeamConfiguration>
                        {
                            new TeamConfiguration
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
}