/*
GHOSTS ANIMATOR
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon® and CERT® are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0930
*/

using System;
using System.Linq;
using Ghosts.Animator.Extensions;
using Ghosts.Animator.Models;
using Ghosts.Animator.Services;

namespace Ghosts.Animator
{
    public static class Npc
    {
        public static NpcProfile NpcProfile { get; set; }

        public static NpcProfile Generate()
        {
            return Generate(new NpcGenerationConfiguration { Branch = MilitaryUnits.GetServiceBranch()});
        }
        
        public static NpcProfile Generate(Enums.MilitaryBranch branch)
        {
            return Generate(new NpcGenerationConfiguration { Branch = branch});
        }
        
        public static NpcProfile Generate(NpcGenerationConfiguration config)
        {
            if(!config.Branch.HasValue)
                config.Branch = MilitaryUnits.GetServiceBranch();
        
            NpcProfile = new NpcProfile
            {
                Id = Guid.NewGuid()
            };

            NpcProfile.Unit = MilitaryUnits.GetOneByServiceBranch(config.Branch.Value);
            NpcProfile.Rank = MilitaryRanks.GetRankByBranch(config.Branch.Value);
            NpcProfile.BiologicalSex = PhysicalCharacteristics.GetBiologicalSex();

            NpcProfile.Birthdate = PhysicalCharacteristics.GetBirthdate(NpcProfile.Rank.Pay);
            NpcProfile.Health = HealthService.GetHealthProfile();
            
            NpcProfile.Address.Add(Address.GetHomeAddress());

            NpcProfile.Name = Name.GetName();

            NpcProfile.Email = Internet.GetMilEmail(NpcProfile.Name.ToString());
            NpcProfile.Password = Internet.GetPassword();
            NpcProfile.CellPhone = PhoneNumber.GetPhoneNumber();
            NpcProfile.HomePhone = PhoneNumber.GetPhoneNumber();

            NpcProfile.Workstation.Domain = Internet.GetMilDomainName();
            NpcProfile.Workstation.Name = Internet.GetComputerName();
            NpcProfile.Workstation.Password = Internet.GetPassword();
            NpcProfile.Workstation.Username = Internet.GetMilUserName(NpcProfile.Name.ToString());
            NpcProfile.Workstation.IPAddress = $"192.168.{AnimatorRandom.Rand.Next(2, 254)}.{AnimatorRandom.Rand.Next(2, 254)}";

            NpcProfile.Career.Strengths = Career.GetStrengths();
            NpcProfile.Career.Weaknesses = Career.GetWeaknesses();
            NpcProfile.Career.WorkEthic = Career.GetWorkEthic();
            NpcProfile.Career.TeamValue = Career.GetTeamValue();
            NpcProfile.Employment = EmploymentHistory.GetEmployment();

            NpcProfile.Family.Members = Family.GetMembers();

            NpcProfile.Finances.CreditCards = CreditCard.GetCreditCards();
            NpcProfile.Finances.NetWorth = CreditCard.GetNetWorth();
            NpcProfile.Finances.TotalDebt = CreditCard.GetTotalDebt();

            NpcProfile.ForeignTravel.Trips = Travel.GetTrips();

            NpcProfile.MentalHealth = MentalHealth.GetMentalHealth();
            NpcProfile.Accounts = Internet.GetAccounts(NpcProfile.Name.ToString());
            NpcProfile.Education = Education.GetMilEducationProfile(NpcProfile.Rank);

            NpcProfile.PGP = PgpService.Generate(NpcProfile.Email);
            
            NpcProfile.InsiderThreat = InsiderThreat.GetInsiderThreatProfile();

            NpcProfile.PhotoLink = PhysicalCharacteristics.GetPhotoUrl();

            NpcProfile.Attributes = AttributesService.GetAttributes();
                
            return NpcProfile;
        }
    }
}