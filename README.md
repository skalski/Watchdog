# Watchdog
 Deamon to watch and quit raging tasks on Windows
 
 I got problems with 3rd Party Software that locks my CPU randomly.
 So i decided to use this little tool.
 
 This tools works also with multiple threads with the same name.
 Each thread will be watched by its own thread.
 
 Its written in C# for Windows (not sure if it can be proted to *nix)
 
 Usage:
 watchdog --help    <- for help
 
 Example;
 watchdog -n outlook -c 90
 
 This means every process named "outlook" will be watched. If the average CPU usage hits 90% in a timeframe of 10 minutes,
the task will be killed.

Cheers :-) 
