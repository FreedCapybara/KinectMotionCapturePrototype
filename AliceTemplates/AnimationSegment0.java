package edu.calvin.cs.alicekinect;

import org.lgna.story.SBiped;
import org.lgna.story.SBox;
import org.lgna.story.SScene;
import org.lgna.story.SetOpacity;
import org.lgna.story.Duration;
import org.lgna.story.DurationAnimationStyleArgumentFactory;
import org.lgna.story.Move;
import org.lgna.story.MoveDirection;
import org.lgna.story.MoveTo;
import org.lgna.story.PointAt;
import org.lgna.story.Position;

/**
 * A section of an animation created with the Kinect.
 * @author andrew
 */
public class AnimationSegment{0} implements IAnimator {

	SBox box = new SBox();
	SBox root = new SBox();

	public AnimationSegment{0}(SScene scene) {
		box.setName("wand");
		box.setOpacity(0, SetOpacity.duration(0));
		root.setName("root");
		root.setOpacity(0, SetOpacity.duration(0));
		root.setVehicle(scene);
	}

	public void animate(SBiped biped) {
		box.setVehicle(biped);

		{1}
	}
}
