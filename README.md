KinectMotionCapturePrototype
============================

Simple motion capture program for the XBox Kinect that generates Java code for animating Alice characters.

Usage:

1. Click "Start" in the bottom right corner to begin recording, and "Stop" to end the recording.
2. Once a movement has been recorded, click "Save" to generate a Jar file (%PROJECT_DIR%\bin\Debug\build\KinectAnimation.jar)
which can be referenced in a NetBeans Alice project.
3. To reference the jar in NetBeans, right-click `Libraries` under your project tree, select `Add JAR/Folder`, navigate to %PROJECT_DIR%\bin\Debug\build\, and select KinectAnimation.jar.
4. With the library referenced you should be able to add lines like this to your Alice project to use it:
```java
KinectAnimation myAnimation = new KinectAnimation(this); // sets up the animation
myAnimation.animate(myBiped); // runs the animation on a given Biped object
```

Demo on YouTube: https://www.youtube.com/watch?v=V-aHkEUg5ps

(The NetBeans plugin for Alice is available at http://www.alice.org/index.php?page=downloads/download_alice3.1)
