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

namespace Ghosts.Animator.Models.InsiderThreat
{
    public class InsiderThreatProfile
    {
        public AccessProfile Access { get; set; }
        public CriminalViolentOrAbusiveConductProfile CriminalViolentOrAbusiveConduct { get; set; }
        public FinancialConsiderationsProfile FinancialConsiderations { get; set; }
        public ForeignConsiderationsProfile ForeignConsiderations { get; set; }
        public JudgementCharacterAndPsychologicalConditionsProfile JudgementCharacterAndPsychologicalConditions { get; set; }
        public ProfessionalLifecycleAndPerformanceProfile ProfessionalLifecycleAndPerformance { get; set; }
        public SecurityAndComplianceIncidentsProfile SecurityAndComplianceIncidents { get; set; }
        public SubstanceAbuseAndAddictiveBehaviorsProfile SubstanceAbuseAndAddictiveBehaviors { get; set; }
        public TechnicalActivityProfile TechnicalActivity { get; set; }
        public bool IsBackgroundCheckStatusClear { get; set; }

        public InsiderThreatProfile()
        {
            this.Access = new AccessProfile();
            this.CriminalViolentOrAbusiveConduct = new CriminalViolentOrAbusiveConductProfile();
            this.FinancialConsiderations = new FinancialConsiderationsProfile();
            this.ForeignConsiderations = new ForeignConsiderationsProfile();
            this.JudgementCharacterAndPsychologicalConditions = new JudgementCharacterAndPsychologicalConditionsProfile();
            this.ProfessionalLifecycleAndPerformance = new ProfessionalLifecycleAndPerformanceProfile();
            this.SecurityAndComplianceIncidents = new SecurityAndComplianceIncidentsProfile();
            this.SubstanceAbuseAndAddictiveBehaviors = new SubstanceAbuseAndAddictiveBehaviorsProfile();
            this.TechnicalActivity = new TechnicalActivityProfile();
        }

        public IEnumerable<RelatedEvent> GetAllEvents()
        {
            var events = new List<RelatedEvent>();
            events.AddRange(this.Access.RelatedEvents);
            events.AddRange(this.FinancialConsiderations.RelatedEvents);
            events.AddRange(this.ForeignConsiderations.RelatedEvents);
            events.AddRange(this.TechnicalActivity.RelatedEvents);
            events.AddRange(this.ProfessionalLifecycleAndPerformance.RelatedEvents);
            events.AddRange(this.SecurityAndComplianceIncidents.RelatedEvents);
            events.AddRange(this.CriminalViolentOrAbusiveConduct.RelatedEvents);
            events.AddRange(this.JudgementCharacterAndPsychologicalConditions.RelatedEvents);
            events.AddRange(this.SubstanceAbuseAndAddictiveBehaviors.RelatedEvents);
            return events;
        }
    }
    
    public class ForeignConsiderationsProfile : InsiderThreatBaseProfile {}
    public class TechnicalActivityProfile : InsiderThreatBaseProfile {}
    public class ProfessionalLifecycleAndPerformanceProfile : InsiderThreatBaseProfile {}
    public class SecurityAndComplianceIncidentsProfile : InsiderThreatBaseProfile { }
    public class CriminalViolentOrAbusiveConductProfile : InsiderThreatBaseProfile { }
    public class JudgementCharacterAndPsychologicalConditionsProfile : InsiderThreatBaseProfile { }
    public class SubstanceAbuseAndAddictiveBehaviorsProfile : InsiderThreatBaseProfile  { }
    public class FinancialConsiderationsProfile : InsiderThreatBaseProfile { }
    
    public class AccessProfile : InsiderThreatBaseProfile
    {
        public string SecurityClearance { get; set; }
        public string PhysicalAccess { get; set; }
        public string SystemsAccess { get; set; }
        public bool? IsDoDSystemsPrivilegedUser { get; set; }
        public string ExplosivesAccess { get; set; }
        public string CBRNAccess { get; set; }
    }
}