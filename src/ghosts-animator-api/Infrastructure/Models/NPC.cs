// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System.Linq;
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

    public static NPC TransformToNpc(NpcProfile o)
    {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<NpcProfile, NPC>());
        var mapper = new Mapper(config);
        return mapper.Map<NPC>(o);
    }
    
    /// <summary>
    /// Summary only copies the first record for many of the lists a profile might have
    /// Often used to submit to a system such as an LLM where a full profile would be too much data
    /// </summary>
    public static NpcProfileSummary TransformToNpcProfileSummary(NpcProfile o)
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<NpcProfile, NpcProfileSummary>()
                .ForMember(dest => dest.Address,
                    opt => opt.MapFrom(src => src.Address.FirstOrDefault()))
                .ForMember(dest => dest.Education.Degrees,
                    opt => opt.MapFrom(src => src.Education.Degrees.FirstOrDefault()))
                .ForMember(dest => dest.Employment.EmploymentRecords,
                    opt => opt.MapFrom(src => src.Employment.EmploymentRecords.FirstOrDefault()))
                .ForMember(dest => dest.ForeignTravel.Trips,
                opt => opt.MapFrom(src => src.ForeignTravel.Trips.FirstOrDefault()));
        });

        var mapper = new Mapper(config);
        return mapper.Map<NpcProfileSummary>(o);
    }
}