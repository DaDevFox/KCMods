using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalDisastersMod.Events
{
    class EventManager
    {
        public static List<ModEvent> events = new List<ModEvent>()
        {
            new EarthquakeEvent(),
            new DroughtEvent(),
            new TornadoEvent()
        };
        public static void OnYearEnd() 
        {
            foreach (ModEvent _event in events) 
            {
                if (_event.Test() && Player.inst.CurrYear % _event.testFrequency == 0)
                {
                    TryRun(_event);
                }
            }
        }

        public static void Init()
        {
            foreach (ModEvent _event in events) 
            {
                _event.Init();
            }

            Broadcast.OnLoadedEvent.Listen(OnLoaded);
            Broadcast.OnSaveEvent.Listen(OnSave);
        }

        public static void TriggerEvent(Type _event)
        {
            foreach (ModEvent __event in events)
            {
                if (__event.GetType() == _event)
                {
                    TryRun(__event);
                }
            }
        }

        public static void OnLoaded(object sender, OnLoadedEvent loadedEvent)
        {
            foreach (ModEvent _event in events) {
                Type saveObjType = _event.saveObject;
                if (LoadSave.ReadDataGeneric(Mod.modID, _event.saveID) != null){
                    _event.OnLoaded(Newtonsoft.Json.JsonConvert.DeserializeObject(LoadSave.ReadDataGeneric("insaneDifficultyMod", _event.saveID), saveObjType));
                }
            }
        }

        public static void OnSave(object sender, OnSaveEvent saveEvent)
        {
            foreach (ModEvent _event in events)
            {
                LoadSave.SaveDataGeneric(Mod.modID, _event.saveID,Newtonsoft.Json.JsonConvert.SerializeObject(_event.OnSave()));
            }
        }

        

        private static void TryRun(ModEvent _event)
        {
            try
            {
                _event.Run();
            }
            catch (Exception ex)
            {
                Mod.helper.Log("Error on Event " + _event.saveID + "\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

    }
}
