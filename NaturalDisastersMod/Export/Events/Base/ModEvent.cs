using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalDisastersMod.Events
{
    class ModEvent
    {
        public String saveID = "";
        public Type saveObject;

        public int testFrequency = 1;

        public virtual bool Test() 
        {
            return false;
        }

        public virtual void Init() 
        {

        }

        public virtual void Run() 
        {

        }

        public virtual object OnSave()
        {
            return null;
        }

        public virtual void OnLoaded(object saveData)
        {

        }


    }
}
