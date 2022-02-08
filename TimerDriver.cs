using System;
using System.Collections.Generic;
using System.Timers;
using Brat.Drivers;
using Brat;
using Newtonsoft.Json.Linq;

namespace Brat.Drivers {
    public class TimerDriver : Driver {
        public TimerDriver(string name) : base(name) { }
        public override DriverAPI GetAPI(ProcessorHost processor) {
            return new TimerAPI(processor);
        }
    }
    public class TimerAPI : DriverAPI {
        public Timer timer {
            get;
            protected set;
        } = new Timer() { 
            AutoReset = false
        };
        private Dictionary<string, TimedEvent> events = new Dictionary<string, TimedEvent> { };
        protected double Resolution {
            get => timer.Interval;
            set {
                if (value < 1000) value = 1000;
                timer.Interval = value;
            }
        }
        public bool TimerResolution(dynamic ms_per_tick) {
            Resolution = double.Parse(Utility.Unwrap(ms_per_tick));
            return true;
        }
        public TimerAPI(ProcessorHost processor) : base(processor) {
            Resolution = 1000;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            Dictionary<string, TimedEvent> current_events = new Dictionary<string, TimedEvent>(events);

            foreach(KeyValuePair<string, TimedEvent> item in current_events) {
                if(item.Value.Tick() is JObject eventSource) {
                    string event_name = $"TIMER_{item.Key}".ToUpper();
                    
                    // add standard event metas
                    eventSource["driver"] = JValue.CreateString("TimerDriver");
                    eventSource["event_id"] = JValue.CreateString(item.Key);
                    eventSource["event_type"] = JValue.CreateString(event_name);

                    Context.AddEvent(new Event(
                        eventSource,
                        event_name
                    ));
                    events.Remove(item.Key);
                }
            }
            timer.Enabled = (events.Count > 0);
        }

        public bool CreateTimer(dynamic name, dynamic modulus = null, dynamic event_value = null) {
            name = Utility.Unwrap(name);
            event_value = Utility.Unwrap(event_value);
            modulus = (int)Utility.Unwrap(modulus);
            if (modulus < 1) modulus = 1;

            events.Add(name, new TimedEvent { 
                modulus = (int)modulus, 
                eventData = JObject.FromObject(event_value) });
            timer.Enabled = true;
            return true;
        }
        public bool CancelTimer(dynamic name) {
            name = Utility.Unwrap(name);
            return events.Remove(name);
        }
        
    }
    class TimedEvent {
        public int modulus = 1;
        public JObject eventData = null;
        private int count = 0;
        
        public TimedEvent(int modulus = 1, JObject data = null) {

        }
        public JObject Tick() {            
            if ((count = (++count % modulus)) == 0) {
                return eventData;
            }
            return null;
        }
    }
}
