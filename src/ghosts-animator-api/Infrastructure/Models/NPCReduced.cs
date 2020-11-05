using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ghosts.Animator.Api.Infrastructure.Models
{
    public class NPCReduced
    {
        public Dictionary<string, string> PropertySelection { get; set; }
        private object NpcObject { get; set; }

        private Type ObjectType { get; set; }
        
        public NPCReduced()
        {
            this.PropertySelection = new Dictionary<string, string>();
        }

        public NPCReduced(string[] fieldsToReturn, NPC npc)
        {
            NpcObject = npc;
            
            //what we'll return
            this.PropertySelection = new Dictionary<string, string>();

            //get an npc to "rip apart"
            ObjectType = typeof(NPC);

            //for each field we want to return
            foreach (var fieldToReturn in fieldsToReturn)
            {
                var fieldArray = fieldToReturn.Split(".");

                //get that field on the npc object
                
                object currentObject = npc;
                
                foreach (var f in fieldArray)
                {
                    if (currentObject != null && currentObject.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    {
                        var collection = (IList) currentObject;
                        currentObject = collection[0];
                    }

                    currentObject = currentObject?.GetType().GetProperty(f)?.GetValue(currentObject, null);
                }

                if (!(currentObject is null))
                {
                    if(currentObject.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))) {}
                    this.PropertySelection.Add(fieldToReturn, currentObject.ToString());
                }


                //other potential datas to look at 
                //type.GetProperties()
                //foreach (var method in type.GetMethods())
            }
        }

        public PropertyInfo GetPropertyInfo(object src, string name)
        {
            if (src.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var collection = (IList) src;
                src = collection[0];
            }
        
            return src.GetType().GetProperties().FirstOrDefault(x => x.Name == name);
        }
    }
}