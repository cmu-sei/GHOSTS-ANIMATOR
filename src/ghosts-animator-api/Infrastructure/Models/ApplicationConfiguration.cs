// Copyright 2020 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

namespace Ghosts.Animator.Api.Infrastructure.Models;

public class ApplicationConfiguration
{
    public string GhostsApiUrl { get; set; }
    public DatabaseSettings.ApplicationDatabaseSettings DatabaseSettings { get; set; }
    public string Proxy { get; set; }
    public AnimationsSettings Animations { get; set; }
    
    public class AnimationsSettings
    {
        public bool IsEnabled { get; set; }
        public SocialGraphSettings SocialGraph { get; set; }
        public SocialBeliefSettings SocialBelief { get; set; }
        public SocialSharingSettings SocialSharing { get; set; }
        public FullAutonomySettings FullAutonomy { get; set; }
        public ChatSettings Chat { get; set; }
        
        public class SocialGraphSettings
        {
            public bool IsEnabled { get; set; }
            public bool IsMultiThreaded { get; set; }
            public bool IsInteracting { get; set; }
            public int TurnLength { get; set; }

            public int MaximumSteps { get; set; }

            public double ChanceOfKnowledgeTransfer { get; set; }

            public DecaySettings Decay { get; set; }

            public class DecaySettings
            {
                public int StepsTo { get; set; }
                public double ChanceOf { get; set; }
            }
        }
        
        public class SocialBeliefSettings
        {
            public bool IsEnabled { get; set; }
            public bool IsMultiThreaded { get; set; }
            public bool IsInteracting { get; set; }
            public int TurnLength { get; set; }
            public int MaximumSteps { get; set; }
        }
        
        public class ChatSettings
        {
            public bool IsEnabled { get; set; }
            public bool IsMultiThreaded { get; set; }
            public bool IsInteracting { get; set; }
            public int TurnLength { get; set; }
            public int MaximumSteps { get; set; }
            public bool IsSendingTimelinesToGhostsApi { get; set; }
            public string PostUrl { get; set; }
            public ContentEngineSettings ContentEngine { get; set; }
        }

        public class SocialSharingSettings
        {
            public bool IsEnabled { get; set; }
            public bool IsMultiThreaded { get; set; }
            public bool IsInteracting { get; set; }
            public bool IsSendingTimelinesToGhostsApi { get; set; }
            public bool IsSendingTimelinesDirectToSocializer { get; set; }
            public string PostUrl { get; set; }
            public int TurnLength { get; set; }
            public int MaximumSteps { get; set; }
            public ContentEngineSettings ContentEngine { get; set; }
        }

        public class FullAutonomySettings
        {
            public bool IsEnabled { get; set; }
            public bool IsMultiThreaded { get; set; }
            public bool IsInteracting { get; set; }
            public bool IsSendingTimelinesToGhostsApi { get; set; }
            public int TurnLength { get; set; }
            public int MaximumSteps { get; set; }
            public ContentEngineSettings ContentEngine { get; set; }
        }
    }
    
    public class ContentEngineSettings
    {
        public string Source { get; set; }
        public string Model { get; set; }
        public string Host { get; set; }
    }
}