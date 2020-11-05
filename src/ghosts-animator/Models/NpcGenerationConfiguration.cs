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

namespace Ghosts.Animator.Models
{
    /// <summary>
    /// Pass this class into the npc generator for fine-tuned configuration,
    /// such as service branch, rank distribution, etc.
    /// </summary>
    public class NpcGenerationConfiguration
    {
        /// <summary>
        /// Set this if a specific service branch is needed
        /// </summary>
        public MilitaryBranch? Branch { get; set; }
        /// <summary>
        /// Set this to ensure specific ranks and their number are generated
        /// </summary>
        public IList<RankDistribution> RankDistribution { get; set; }
        /// <summary>
        /// Set this to generate NPCs that are all part of the same service branch unit
        /// </summary>
        public string Unit { get; set; }
    }

    public class RankDistribution
    {
        /// <summary>
        /// Corresponds to pay grade indications for each US service branch
        /// and their corresponding ranks 
        /// </summary>
        public string PayGrade { get; set; }
        /// <summary>
        /// The probability of generating this rank based on overall numbers provided
        /// </summary>
        public double Probability { get; set; }
        /// <summary>
        /// For larger distributions, set this to the number of minimum that must be generated for a scenario
        /// </summary>
        public int Minimum { get; set; }
    }
}