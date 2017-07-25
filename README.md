# LenseRayTracer
Create light sources and setup lenses and watch ray tracing happen real time! Intended for educational purposes only. 
![alt text](https://github.com/sk8ermeb/LenseRayTracer/blob/master/Interface.png)

<h2>How To Use This Program</h2>
This is written in C# .net universal. So you likely need visual studio and the latest .net to run it. I am using 4.6.1. First thing to note is all fields in this program are dimensionless as they are relative (except for ange/degrees). If it helps you visualize just pick whatever unit is appropraiate for your experiment. Mathmatically it will all come out sense everything is relative. Also all colors are specified in R-G-B each on a scale from 0 to 255, so "255-0-0" is entirely red.  
<h3>Lense</h3>
First add the lenses you want. To do this go to Simulation ->Lenses. Positive radius is convex and Negative in concave. There is a horizontal and depth radius for the top and the bottom so that you can do unique things like focus light in only one dimension while diverging light in another.
The Granulatrity is very important, and the smaller the granularity the more difficult it is to render but the better it looks. this is ray tracing so everything is numerical approximation. 
Lastly index of refraction is just that. The air has an index of refraction of 1, so it is relative. 
<h3>Light Source</h3>
There are two kinds of light, and infinite light, and a spotlight. The infinite light is for modeling the sun. The direction is in calissic spherical coordinates. So PHI and Theta. 
</strong>The granularity is entirely determined by the ground plane, so you must ad a groundplane to have an infinite light. One ray is aimed toward each piece to give the simulation reasonable bounds.</strong>
For the spot light the direction it is aimed provides and X, Y, and Z. -Y is directly down. You can spcify granularity by approximately how many degrees between each ray. 

<h3>Lense Encountering</h3>
There are lots of shortcuts taken to speed up processing time. The most important is that the program is not aware of the side of the lense that is actually the first encounter, rather it always incounters the "top" radius first. So if the lense has been flipped 180 such that the light is hitting the bottom first your simulation will not be rendered accuratly. 
Once a ray has encountered the groundplane, or has nothing else to encounter it will be killed. 

If the tracing is taking way to long uncheck Simulation -> Draw Rays Real Time. This option updates the rendering after each ray has been traced to help visualize what is going on. 

The Reset Light option removes all rendered light to get a fresh rendering. Leacing it there gives the user to see iterative tracings.

The color of all light gets normalized by the brightness, so color 100-100-100 with brightness 100 should be rendered similaraly to 200-200-200 brightness 100. 

If the radius is smaller then the lense size wierd things will start happening. You willl see the rendering not converge. I havn't bothered adjusting for that case so keep that in mind. 

Enjoy! I am just one guy who had need for a ray tracing program for a project I was doing because I didn't have the money for custom lenses being build so I created this. The code is not pretty nor commenented. But its a free 3d ray tracing program in C#. Feel free to contribute as well if you want.
![alt text](https://github.com/sk8ermeb/LenseRayTracer/blob/master/two-lights.png)
