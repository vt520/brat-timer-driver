/** provided by: brat-timer-driver */
var timer = ConnectDriver("TimerDriver");
TimerResolution(1000);

const TIMER = {
    minute: 60,
    hour: 3600
}

function Remind(callback, seconds, event = null) {
    if (!event) event = {};
    let timer_name = UUID();
    let timer_info = Persist.Initialize(timer_name, {});
    timer_info.relay = AddRelay(CompileFilter({
        event_id: `@${timer_name}`,
        driver: "TimerDriver"
    }), callback);
    timer_info.active = CreateTimer(timer_name, seconds, event);
    return timer_name;

}

function CancelRemind(timer_name) {
    CancelTimer(timer_name);
    CancelRelay(Persist[timer_name].relay);
}