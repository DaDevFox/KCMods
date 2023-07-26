using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsaneDifficultyMod.Events
{
    public class EventManager
    {
        public static List<IDModEvent> events = new List<IDModEvent>()
        {
            new EarthquakeEvent(),
            new OppressionEvent(),
            new DroughtEvent()
        };
        public static void OnYearEnd() 
        {
            foreach (IDModEvent _event in events) 
            {
                if (_event.Test() && Player.inst.CurrYear % _event.testFrequency == 0)
                {
                    TryRun(_event);
                }
            }
        }

        public static void Init()
        {
            foreach (IDModEvent _event in events) 
            {
                _event.Init();
            }

            Broadcast.OnLoadedEvent.Listen(OnLoaded);
            Broadcast.OnSaveEvent.Listen(OnSave);
        }

        public static void TriggerEvent(Type _event)
        {
            foreach (IDModEvent __event in events)
            {
                if (__event.GetType() == _event)
                {
                    TryRun(__event);
                }
            }
        }

        public static void OnLoaded(object sender, OnLoadedEvent loadedEvent)
        {
            foreach (IDModEvent _event in events) {
                Type saveObjType = _event.saveObject;
                if (LoadSave.ReadDataGeneric("insaneDifficultyMod", _event.saveID) != null){
                    _event.OnLoaded(Newtonsoft.Json.JsonConvert.DeserializeObject(LoadSave.ReadDataGeneric("insaneDifficultyMod", _event.saveID), saveObjType));
                }
            }
        }

        public static void OnSave(object sender, OnSaveEvent saveEvent)
        {
            foreach (IDModEvent _event in events)
            {
                LoadSave.SaveDataGeneric("insaneDifficultyMod", _event.saveID,Newtonsoft.Json.JsonConvert.SerializeObject(_event.OnSave()));
            }
        }

        

        private static void TryRun(IDModEvent _event)
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
