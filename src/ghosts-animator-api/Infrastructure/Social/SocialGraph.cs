// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Collections.Generic;

namespace Ghosts.Animator.Api.Infrastructure.Social;

public class SocialGraph
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IList<SocialConnection> Connections { get; set; }
    public IList<Learning> Knowledge { get; set; }
        
    public long CurrentStep { get; set; }
        

    public SocialGraph()
    {
        this.Connections = new List<SocialConnection>();
        this.Knowledge = new List<Learning>();
    }
        
    public class SocialConnection
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Distance { get; set; }
        public int RelationshipStatus { get; set; }
            
        public IList<Interaction> Interactions { get; set; }

        public SocialConnection()
        {
            this.Interactions = new List<Interaction>();
        }
    }

    public class Interaction
    {
        public long Step { get; set; }
        public int Value { get; set; }
    }

    public class Learning
    {
        public Guid To { get; set; }
        public Guid From { get; set; }
        public string Topic { get; set; }
        public long Step { get; set; }
        public int Value { get; set; }

        public Learning(Guid to, Guid from, string topic, long currentStep, int value)
        {
            this.From = from;
            this.To = to;
            this.Topic = topic;
            this.Step = currentStep;
            this.Value = value;
        }

        public override string ToString()
        {
            return $"{this.To},{this.From},{this.Topic},{this.Step},{this.Value}";
        }
    }
}