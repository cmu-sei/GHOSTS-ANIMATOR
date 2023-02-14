// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;

namespace Ghosts.Animator.Api.Infrastructure.Models;

public class NPCIpAddress
{
    public Guid NpcId { get; set; }
    public string IpAddress { get; set; }
    public DateTime CreatedUTC { get; set; }
        
    public string Enclave { get; set; }

    public NPCIpAddress()
    {
        this.CreatedUTC = DateTime.UtcNow;
    }
}