KinectMotionCapturePrototype
============================

Simple motion capture program for the XBox Kinect that generates Alice 3 classes (.a3c files).

##Usage

1. Run the program with an Xbox Kinect connected to your machine (only tested with Kinect v1.8).
2. Click `Start` in the bottom right corner to begin recording, and `Stop` to end the recording.
3. Once a movement has been recorded, click `Save` to generate an Alice 3 class in the desired location.
4. In Alice, go to `Setup Scene `>` 'My Classes' tab `>` new ... from KinectAnimation.a3c`
5. With the library referenced you should be able to add lines like this to your Alice project in NetBeans to use it:
```java
myBiped.animate(scene); // runs the animation
```

Demo on YouTube: https://www.youtube.com/watch?v=V-aHkEUg5ps (shows the old NetBeans implementation, but the animation looks the same)

##Setup

Requires the [Kinect SDK v1.8](https://www.microsoft.com/en-us/download/details.aspx?id=40278) (and [IKVM v8.1.5561](http://www.frijters.net/ikvmbin-8.1.5561.zip), which is already included in `packages/`)

Otherwise, just build and run!
