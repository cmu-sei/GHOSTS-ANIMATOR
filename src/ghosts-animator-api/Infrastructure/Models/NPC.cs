// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using AutoMapper;
using Ghosts.Animator.Models;

namespace Ghosts.Animator.Api.Infrastructure.Models;

public class NPC : NpcProfile
{
    /// <summary>
    /// GCD 2020, GCD 2021
    /// </summary>
    public string Campaign { get; set; }
        
    /// <summary>
    /// We could call this a group but in order
    /// to be more specific we use enclave.
    /// E.g. RCC-C, E, K, P, S, CPT503, etc.
    /// </summary>
    public string Enclave { get; set; }
        
    /// <summary>
    /// A team within an enclave
    /// E.g. RCC-K's HQ team
    /// </summary>
    public string Team { get; set; }

    public static NPC TransformTo(NpcProfile o)
    {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<NpcProfile, NPC>());
        var mapper = new Mapper(config);
        return mapper.Map<NPC>(o);
    }
}