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
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ghosts.Animator.Models
{
    public class EmploymentProfile
    {
        public IList<EmploymentRecord> EmploymentRecords { get; set; }

        public EmploymentProfile()
        {
            this.EmploymentRecords = new List<EmploymentRecord>();
        }
        
        public class EmploymentRecord
        {
            public string Company { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Department { get; set; }
            public string Organization { get; set; }
            public string JobTitle { get; set; }
            public int Level { get; set; }
            public double Salary { get; set; }
            public Guid Manager { get; set; }
            public string EmailSuffix { get; set; }
            public string Email { get; set; }
            public AddressProfiles.AddressProfile Address { get; set; }
            public string Phone { get; set; }
            public EmploymentStatuses EmploymentStatus { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            public enum EmploymentStatuses
            {
                FullTime = 0,
                PartTime = 1,
                Suspended = 2,
                Temporary = 3,
                Resigned = -5,
                Terminated = -9
            }

            public EmploymentRecord()
            {
                this.Address = new AddressProfiles.AddressProfile();
            }
        }
    }
}