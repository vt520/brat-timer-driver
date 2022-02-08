# Brat TimerDriver

A Simple Oneshot Timer for Brat / Bratwurst

## Sample Usage
```javascript
ConnectDriver("TimerDriver");
TimerResolution(1000); // Each tick will be 1 second

const timer_name = 'A_TIMER';
const timer_ticks = 10;
let timer_data = {
	count: 0
}

var timer_handle = CreateTimer(
	timer_name,
	timer_ticks,
	timer_data);

Procssors = [
	{
		event: `TIMER_${timer_name}`,
		include: [
			driver: "TimerDriver"
		],
		action: TimerAction
	}
];

function TimerAction(event) {
	if(++event.count.Value <= 5)
		CreateTimer(timer_name, timer_ticks, event);
}
```
